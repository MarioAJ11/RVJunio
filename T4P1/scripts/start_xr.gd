class_name StartXR
extends Node3D

signal focus_lost
signal focus_gained
signal pose_recentered

@export var maximum_refresh_rate := 90

var xr_interface: OpenXRInterface
var xr_is_focussed := false
var xr_active := false


func _ready() -> void:
	if Engine.is_editor_hint():
		return
	_try_start_xr()


func is_xr_active() -> bool:
	return xr_active


func _try_start_xr() -> void:
	xr_interface = XRServer.find_interface("OpenXR")
	if not xr_interface:
		push_warning("OpenXR no encontrado. Usando vista previa en escritorio.")
		return

	if not xr_interface.is_initialized():
		if not xr_interface.initialize():
			push_warning(
				"OpenXR no disponible en este PC. Vista previa en escritorio; "
				+ "en Quest funcionara con el APK."
			)
			return

	xr_active = true
	print("OpenXR inicializado correctamente.")

	var vp := get_viewport()
	vp.use_xr = true
	DisplayServer.window_set_vsync_mode(DisplayServer.VSYNC_DISABLED)

	if RenderingServer.get_rendering_device():
		vp.vrs_mode = Viewport.VRS_XR

	xr_interface.session_begun.connect(_on_openxr_session_begun)
	xr_interface.session_visible.connect(_on_openxr_visible_state)
	xr_interface.session_focussed.connect(_on_openxr_focused_state)
	xr_interface.session_stopping.connect(_on_openxr_stopping)
	xr_interface.pose_recentered.connect(_on_openxr_pose_recentered)

	if xr_interface.get_environment_blend_mode() == XRInterface.XR_ENV_BLEND_MODE_ALPHA_BLEND:
		vp.transparent_bg = true


func _on_openxr_session_begun() -> void:
	var current_refresh_rate := xr_interface.get_display_refresh_rate()
	var new_rate := current_refresh_rate
	var available_rates: Array = xr_interface.get_available_display_refresh_rates()

	for rate in available_rates:
		if rate > new_rate and rate <= maximum_refresh_rate:
			new_rate = rate

	if current_refresh_rate > 0 and current_refresh_rate != new_rate:
		xr_interface.set_display_refresh_rate(new_rate)
		current_refresh_rate = new_rate

	if current_refresh_rate > 0:
		Engine.physics_ticks_per_second = int(current_refresh_rate)


func _on_openxr_visible_state() -> void:
	if xr_is_focussed:
		xr_is_focussed = false
		process_mode = Node.PROCESS_MODE_DISABLED
		focus_lost.emit()


func _on_openxr_focused_state() -> void:
	xr_is_focussed = true
	process_mode = Node.PROCESS_MODE_INHERIT
	focus_gained.emit()


func _on_openxr_stopping() -> void:
	print("OpenXR se está cerrando.")


func _on_openxr_pose_recentered() -> void:
	pose_recentered.emit()
