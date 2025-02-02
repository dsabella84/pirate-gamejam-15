extends Level

const NOISE_SHAKE_SPEED: float = 30.0
const NOISE_SWAY_SPEED: float = 1.0
const NOISE_SHAKE_STRENGTH: float = 60.0
const NOISE_SWAY_STRENGTH: float = 10.0
const RANDOM_SHAKE_STRENGTH: float = 30.0
const SHAKE_DECAY_RATE: float = 1.1
const EARTHQUAKE_DURATION: float = 6.0
const DELAY_BEFORE_EARTHQUAKE: float = 2.0

var dialog_resource: DialogueResource = preload ('res://scenes/dialogue/LabInterior.dialogue')
var lantern_first_time_used: bool = false
var noise_i: float = 0.0
var shake_type: int = ShakeType.Random
var shake_strength: float = 0.0
var earthquake_active: bool = false

@onready var noise = FastNoiseLite.new()
@onready var rand = RandomNumberGenerator.new()
@onready var lantern = $"Future/Lantern"
@onready var rubble: Node2D = $"Future/EntrywayRubble"
@onready var earthquake_timer = Timer.new()

enum ShakeType {
	Random,
	Noise,
	Sway
}

func _ready() -> void:
	super._ready();
	lanter_locked = true;

	DialogState.start_dialog(load('res://scenes/dialogue/LabInterior.dialogue'), 'start_of_game_text')
	rand.randomize()
	noise.seed = rand.randi()
	noise.frequency = 0.5
	
	add_child(earthquake_timer)
	earthquake_timer.wait_time = DELAY_BEFORE_EARTHQUAKE
	earthquake_timer.one_shot = true
	earthquake_timer.connect("timeout", Callable(self, "_on_earthquake_timer_timeout"))

	WorldState.time_changed.connect(_on_time_changed)

func _on_lantern_interacted(_initiator):
	WorldState.disable_movement = true;

	lantern.visible = false
	lantern.queue_free()
	
	await DialogState.start_dialog(load('res://scenes/dialogue/LabInterior.dialogue'), 'lantern_picked_up')

	WorldState.player.direction = Vector2.UP;
	$"EarthquakeSound".play()
	earthquake_timer.start()

func _on_earthquake_timer_timeout():
	shake_strength = RANDOM_SHAKE_STRENGTH
	shake_type = ShakeType.Random
	earthquake_timer = EARTHQUAKE_DURATION
	earthquake_active = true

func _process(delta: float) -> void:
	if !earthquake_active: return
	
	earthquake_timer -= delta
	if earthquake_timer <= 2:
		rubble.visible = true
	if earthquake_timer <= 0:
		WorldState.disable_movement = false;
		lanter_locked = false;
		earthquake_active = false
		shake_strength = 0.0
	else:
		shake_strength = lerp(shake_strength, 0.0, SHAKE_DECAY_RATE * delta)
		var shake_offset: Vector2
		match shake_type:
			ShakeType.Random:
				shake_offset = get_random_offset()
			ShakeType.Noise:
				shake_offset = get_noise_offset(delta, NOISE_SHAKE_SPEED, shake_strength)
			ShakeType.Sway:
				shake_offset = get_noise_offset(delta, NOISE_SWAY_SPEED, NOISE_SWAY_STRENGTH)
		WorldState.player.camera.offset = shake_offset

func get_noise_offset(delta: float, speed: float, strength: float) -> Vector2:
	noise_i += delta * speed
	return Vector2(
		noise.get_noise_2d(1, noise_i) * strength,
		noise.get_noise_2d(100, noise_i) * strength
	)

func get_random_offset() -> Vector2:
	return Vector2(
		rand.randf_range( - shake_strength, shake_strength),
		rand.randf_range( - shake_strength, shake_strength)
	)

func _on_entryway_text_body_entered(_body):
	if WorldState.in_future:
		if !WorldState.lantern_unlocked:
			DialogState.start_dialog(dialog_resource, 'try_leave_lab_without_lantern')
		else:
			DialogState.start_dialog(dialog_resource, 'interact_rubble')

func _on_time_changed(to_future: bool):
	if to_future||lantern_first_time_used: return

	DialogState.start_dialog(dialog_resource, 'lantern_first_use')
	lantern_first_time_used = true
