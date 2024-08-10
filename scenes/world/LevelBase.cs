using System;
using System.Linq;
using Godot;

public partial class LevelBase : Node2D
{
    private static PackedScene? _playerPacked;

    #region Autospawn configuration

    [ExportGroup("Autospawn"), Export] public bool AutospawnEnabled { get; set; } = true;
    [ExportGroup("Autospawn"), Export] public SpawnPoint? AutospawnSpawnPoint { get; set; }

    #endregion

    #region "Past" configuration

    [ExportGroup("\"Past\" configuration"), Export] public Color BackgroundPast { get; set; }
    [ExportGroup("\"Past\" configuration"), Export] public Node2D? GroupPast { get; set; }
    [ExportGroup("\"Past\" configuration"), Export] public AudioStream? MusicPast { get; set; }

    #endregion

    #region "Future" configuration

    [ExportGroup("\"Future\" configuration"), Export] public Color BackgroundFuture { get; set; }
    [ExportGroup("\"Future\" configuration"), Export] public Node2D? GroupFuture { get; set; }
    [ExportGroup("\"Future\" configuration"), Export] public AudioStream? MusicFuture { get; set; }

    #endregion

    #region "Shared" configuration

    [ExportGroup("\"Shared\" configuration"), Export] public Node2D? GroupShared { get; set; }

    #endregion

    public Player? Player { get; private set; }

    /// <inheritdoc />
    public override void _Ready()
    {
        _playerPacked ??= GD.Load<PackedScene>("res://scenes/characters/player/Player.tscn");

        if (AutospawnEnabled && Game.AutospawnEnabledGlobal)
        {
            SpawnPoint spawnPoint = AutospawnSpawnPoint ?? throw new InvalidOperationException("Autospawn spawn point is not set");
            SpawnPlayerAtSpawnPoint(spawnPoint);
        }

        // todo: music

        SwitchNodeGroups(Game.IsWorldInFuture);
    }

    public void SpawnPlayerAtSpawnPoint(int spawnPointId)
    {
        SpawnPoint spawnPoint = GetChildren().OfType<SpawnPoint>().Single(sp => sp.Id == spawnPointId);
        SpawnPlayerAtSpawnPoint(spawnPoint);
    }

    public void SpawnPlayerAtSpawnPoint(SpawnPoint spawnPoint)
    {
        var position = spawnPoint.GlobalPosition;
        var direction = spawnPoint.FacingDirection;

        Game.Player?.QueueFree();
        Player = _playerPacked!.Instantiate<Player>();

        Player.GlobalPosition = position;
        Player.FacingDirection = direction;

        CallDeferred("add_child", Player);
    }

    public void SwitchNodeGroups(bool toFuture)
    {
        if (GroupPast != null) GroupPast.Visible = !toFuture;
        if (GroupFuture != null) GroupFuture.Visible = toFuture;
    }
}
