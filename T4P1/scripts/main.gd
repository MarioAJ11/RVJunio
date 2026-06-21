extends StartXR

@onready var world_environment: WorldEnvironment = $WorldEnvironment
@onready var xr_origin: XROrigin3D = $XROrigin3D
@onready var desktop_camera: Camera3D = $DesktopCamera
@onready var game_ui_3d: Node3D = $GameUI
@onready var desktop_hud: CanvasLayer = $DesktopHUD
@onready var desktop_simulator: Node = $DesktopSimulator


func _ready() -> void:
	super._ready()
	if is_xr_active():
		_setup_mixed_reality()
		call_deferred("_recenter_view")
	else:
		_setup_desktop_preview()


func _setup_mixed_reality() -> void:
	get_viewport().transparent_bg = true

	if world_environment and world_environment.environment:
		world_environment.environment.background_mode = Environment.BG_CLEAR_COLOR

	if xr_interface:
		xr_interface.environment_blend_mode = XRInterface.XR_ENV_BLEND_MODE_ALPHA_BLEND

	if game_ui_3d:
		game_ui_3d.visible = true
	if desktop_hud:
		desktop_hud.visible = false
	if desktop_simulator:
		desktop_simulator.set_process(false)
		desktop_simulator.set_physics_process(false)
		desktop_simulator.set_process_input(false)
		desktop_simulator.set_process_unhandled_input(false)


func _setup_desktop_preview() -> void:
	get_viewport().use_xr = false
	get_viewport().transparent_bg = false

	if world_environment and world_environment.environment:
		world_environment.environment.background_mode = Environment.BG_COLOR
		world_environment.environment.background_color = Color(0.14, 0.16, 0.2)

	if desktop_camera:
		desktop_camera.current = true

	if xr_origin:
		xr_origin.visible = false

	if game_ui_3d:
		game_ui_3d.visible = false
	if desktop_hud:
		desktop_hud.visible = true


func _recenter_view() -> void:
	if not is_xr_active():
		return
	await get_tree().create_timer(0.5).timeout
	XRServer.center_on_hmd(XRServer.RESET_BUT_KEEP_TILT, true)
