using Godot;

public partial class SceneTransition : Area2D
{
    private PackedScene _preloaded = null!;

    [Export(PropertyHint.File, "*.tscn")] public string? NextLevelPath { get; set; }
    [Export] public int SpawnId { get; set; }

    /// <inheritdoc />
    public override void _Ready()
    {
        if (NextLevelPath == null)
        {
            GD.PushWarning($"SceneTransition '{Name}' has unset target level, SceneTransition will not work");
            return;
        }

        _preloaded = GD.Load<PackedScene>(NextLevelPath);
    }

    private void OnBodyEntered(Node2D body)
    {
        if (NextLevelPath == null || body is not Player) return;

        var instance = _preloaded.Instantiate<LevelBase>();
        _ = Game.Instance.TransitToLevelAsync(instance, SpawnId);
    }
}
