extends Node

## Simula agarre y lanzamiento en PC (sin gafas) para probar malabares y grabar video.

@export var camera: Camera3D
@export var hold_distance: float = 0.55
@export var gentle_toss_up: float = 1.55
@export var high_toss_up: float = 2.45
@export var gentle_toss_forward: float = 0.12
@export var flick_multiplier: float = 0.045
@export var max_throw_speed: float = 2.1
@export var high_throw_speed: float = 2.8

var _held_ball: JugglingBall
var _recent_deltas: Array[Vector2] = []
var _webcam_enabled := false
var _base_environment: Environment

@onready var _world_environment: WorldEnvironment = $"../WorldEnvironment"


func _ready() -> void:
	var main := get_parent()
	if main and main.has_method("is_xr_active") and main.is_xr_active():
		set_process(false)
		set_physics_process(false)
		set_process_input(false)
		set_process_unhandled_input(false)
		return

	if not camera:
		camera = $"../DesktopCamera"
	if _world_environment:
		_base_environment = _world_environment.environment
	set_process_unhandled_input(true)


func _unhandled_input(event: InputEvent) -> void:
	if event is InputEventMouseButton:
		var mb := event as InputEventMouseButton
		if mb.button_index == MOUSE_BUTTON_LEFT:
			if mb.pressed:
				_try_grab_ball(mb.position)
			else:
				_release_ball(false)
	elif event is InputEventMouseMotion:
		if not _held_ball:
			return
		var motion := event as InputEventMouseMotion
		_recent_deltas.append(motion.relative)
		if _recent_deltas.size() > 5:
			_recent_deltas.pop_front()


func _input(event: InputEvent) -> void:
	if event is InputEventKey:
		var key := event as InputEventKey
		if not key.pressed or key.echo:
			return
		if key.keycode == KEY_C:
			toggle_webcam_passthrough()
		elif key.keycode == KEY_SPACE and _held_ball:
			_release_ball(true)


func _physics_process(_delta: float) -> void:
	if not _held_ball or not camera:
		return

	var hold_pos := (
		camera.global_position
		+ (-camera.global_transform.basis.z * hold_distance)
	)
	_held_ball.global_position = hold_pos
	_held_ball.linear_velocity = Vector3.ZERO
	_held_ball.angular_velocity = Vector3.ZERO


func _try_grab_ball(screen_pos: Vector2) -> void:
	var ball := _ray_pick_ball(screen_pos)
	if ball:
		_held_ball = ball
		_recent_deltas.clear()
		_held_ball.grab_desktop()


func _release_ball(high_toss: bool) -> void:
	if not _held_ball or not camera:
		return

	var velocity := _compute_throw_velocity(high_toss)
	_held_ball.release_desktop(velocity)
	_held_ball = null
	_recent_deltas.clear()


func _compute_throw_velocity(high_toss: bool) -> Vector3:
	var forward := -camera.global_transform.basis.z
	forward.y = 0.0
	if forward.length_squared() > 0.001:
		forward = forward.normalized()
	else:
		forward = Vector3(0, 0, -1)

	var flick := _average_flick()
	var flick_strength := flick.length()
	var up_speed := high_toss_up if high_toss else gentle_toss_up
	var speed_cap := high_throw_speed if high_toss else max_throw_speed

	if not high_toss and flick_strength < 2.5:
		return Vector3(
			forward.x * gentle_toss_forward,
			up_speed,
			forward.z * gentle_toss_forward
		).limit_length(speed_cap)

	var side := camera.global_transform.basis.x
	side.y = 0.0
	side = side.normalized()

	var velocity := Vector3(
		forward.x * gentle_toss_forward + side.x * flick.x * flick_multiplier,
		up_speed + flick.y * flick_multiplier * 0.35,
		forward.z * gentle_toss_forward + side.z * flick.x * flick_multiplier
	)
	return velocity.limit_length(speed_cap)


func _average_flick() -> Vector2:
	if _recent_deltas.is_empty():
		return Vector2.ZERO

	var sum := Vector2.ZERO
	for delta in _recent_deltas:
		sum += delta
	return sum / _recent_deltas.size()


func _ray_pick_ball(screen_pos: Vector2) -> JugglingBall:
	if not camera:
		return null

	var from := camera.project_ray_origin(screen_pos)
	var to := from + camera.project_ray_normal(screen_pos) * 12.0
	var query := PhysicsRayQueryParameters3D.create(from, to)
	query.collide_with_areas = false
	query.collide_with_bodies = true

	var hit := camera.get_world_3d().direct_space_state.intersect_ray(query)
	if hit.is_empty():
		return null

	var collider: Object = hit.collider
	if collider is JugglingBall:
		return collider as JugglingBall

	return null


func toggle_webcam_passthrough() -> void:
	if not _world_environment or not _base_environment:
		return

	if _webcam_enabled:
		_world_environment.environment = _base_environment
		_webcam_enabled = false
		print("Passthrough simulado: OFF")
		return

	if CameraServer.get_feed_count() == 0:
		push_warning("No hay webcam disponible.")
		return

	var feed := CameraServer.get_feed(0)
	feed.set_active(true)

	var env := _base_environment.duplicate()
	env.background_mode = Environment.BG_CAMERA_FEED
	env.camera_feed_id = 0
	_world_environment.environment = env
	_webcam_enabled = true
	print("Passthrough simulado: ON (webcam como fondo)")


func is_webcam_enabled() -> bool:
	return _webcam_enabled
