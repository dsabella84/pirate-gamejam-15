using System;
using Godot;

public partial class SpawnPoint : Node2D
{
    [Export] public int Id { get; set; }
    [Export] public Direction FacingDirection { get; set; } = Direction.Down;

    public Vector2 GetDirectionVector()
    {
        return FacingDirection switch
        {
            Direction.Up => Vector2.Up,
            Direction.Down => Vector2.Down,
            Direction.Left => Vector2.Left,
            Direction.Right => Vector2.Right,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}

public enum Direction
{
    Up,
    Down,
    Left,
    Right,
}
