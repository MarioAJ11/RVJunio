class_name JugglingBall
extends XRToolsPickable

signal juggle_catch
signal juggle_toss
signal juggle_fell

@export var ball_color: Color = Color(0.9, 0.2, 0.15):
	set(value):
		ball_color = value
		_apply_color()

@export var reset_delay: float = 1.2

var _home: Vector3
var _in_air := false
var _reset_timer := 0.0

@onready var _mesh: MeshInstance3D = $Mesh


func _ready() -> void:
	release_mode = ReleaseMode.UNFROZEN
	mass = 0.1
	_home = global_position

	var bounce := PhysicsMaterial.new()
	bounce.bounce = 0.5
	bounce.friction = 0.45
	physics_material_override = bounce

	picked_up.connect(_on_picked_up)
	dropped.connect(_on_dropped)
	grabbed.connect(_on_grabbed)
	_apply_color()


func set_home(pos: Vector3) -> void:
	_home = pos


func _apply_color() -> void:
	if not is_inside_tree():
		return
	if not _mesh:
		_mesh = get_node_or_null("Mesh") as MeshInstance3D
	if not _mesh:
		return

	var mat := StandardMaterial3D.new()
	mat.albedo_color = ball_color
	mat.roughness = 0.2
	mat.metallic = 0.08
	_mesh.material_override = mat


func _physics_process(delta: float) -> void:
	if Engine.is_editor_hint():
		return

	if is_picked_up() or freeze:
		_in_air = false
		_reset_timer = 0.0
		return

	if global_position.y > _home.y + 0.1:
		_in_air = true
		_reset_timer = 0.0
		return

	var fell := global_position.y < 0.45
	fell = fell or global_position.distance_to(_home) > 1.25
	if fell:
		_reset_timer += delta
		if _reset_timer >= reset_delay:
			_reset_to_home()
	else:
		_reset_timer = 0.0


func _reset_to_home() -> void:
	global_position = _home
	linear_velocity = Vector3.ZERO
	angular_velocity = Vector3.ZERO
	sleeping = true
	_in_air = false
	_reset_timer = 0.0
	juggle_fell.emit()


func _on_picked_up(_pickable: XRToolsPickable) -> void:
	_in_air = false


func _on_dropped(_pickable: XRToolsPickable) -> void:
	if linear_velocity.length() > 0.7:
		juggle_toss.emit()


func _on_grabbed(_pickable: XRToolsPickable, _by: Node3D) -> void:
	if _in_air:
		juggle_catch.emit()
	_in_air = false


func grab_desktop() -> void:
	if _in_air:
		juggle_catch.emit()
	_in_air = false
	freeze = true


func release_desktop(velocity: Vector3) -> void:
	freeze = false
	linear_velocity = velocity
	if velocity.length() > 0.7:
		juggle_toss.emit()
