extends Marker2D

@export var is_destination: bool = false

func _draw():
	draw_circle(Vector2.ZERO, 35, Color.TRANSPARENT)
	
func select():
	for child in get_tree().get_nodes_in_group("zone"):
		child.deselect()
	modulate=Color.TRANSPARENT
	
func deselect():
	modulate = Color.TRANSPARENT
	
