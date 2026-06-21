extends Node3D

@onready var _stats_label_3d: Label3D = %StatsLabel
@onready var _stats_label_2d: Label = %DesktopStatsLabel

var _catches := 0
var _tosses := 0
var _streak := 0
var _best_streak := 0


func _ready() -> void:
	var balls_parent := get_parent().get_node_or_null("InteractiveObjects")
	if not balls_parent:
		return

	for child in balls_parent.get_children():
		if child is JugglingBall:
			var ball := child as JugglingBall
			ball.set_home(ball.global_position)
			ball.juggle_catch.connect(_on_catch)
			ball.juggle_toss.connect(_on_toss)
			ball.juggle_fell.connect(_on_fell)

	_update_label()


func _on_catch() -> void:
	_catches += 1
	_streak += 1
	_best_streak = max(_best_streak, _streak)
	_update_label()


func _on_toss() -> void:
	_tosses += 1
	_update_label()


func _on_fell() -> void:
	_streak = 0
	_update_label()


func _update_label() -> void:
	var text := (
		"Atrapadas: %d   Lanzadas: %d\nRacha: %d   Mejor: %d"
		% [_catches, _tosses, _streak, _best_streak]
	)

	if _stats_label_3d:
		_stats_label_3d.text = text
	if _stats_label_2d:
		_stats_label_2d.text = text
