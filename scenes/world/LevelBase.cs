using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;

public partial class LevelBase : Node2D
{
    private static PackedScene? _playerPacked;

    #region Autospawn configuration

    [Export, ExportCategory("Autospawn")] public bool AutospawnEnabled { get; set; } = true;
    [Export] public SpawnPoint? AutospawnSpawnPoint { get; set; }

    #endregion

    #region "Past" configuration

    [Export, ExportCategory("Past")] public Color PastBackgroundColor { get; set; } = Color.Color8(0, 0, 0);
    [Export] public Node2D? PastNodesGroup { get; set; }
    [Export] public AudioStream? PastMusic { get; set; }

    #endregion

    #region "Future" configuration

    [Export, ExportCategory("Future")] public Color FutureBackgroundColor { get; set; } = Color.Color8(255, 255, 255);
    [Export] public Node2D? FutureNodesGroup { get; set; }
    [Export] public AudioStream? FutureMusic { get; set; }

    #endregion

    #region "Shared" configuration

    [Export, ExportCategory("Shared")] public Node2D? GroupShared { get; set; }

    #endregion

    public Player? Player { get; private set; }

    /// <inheritdoc />
    public override void _Ready()
    {
        _playerPacked ??= GD.Load<PackedScene>("res://scenes/characters/player/Player.tscn");

        if (AutospawnEnabled && Game.Instance.PlayerInstance == null)
        {
            SpawnPoint spawnPoint = AutospawnSpawnPoint ?? throw new InvalidOperationException("Autospawn spawn point is not set");
            SpawnPlayerAtSpawnPoint(spawnPoint);
        }

        // todo: music

        SwitchNodeGroups(PlayerData.Current!.InFuture);
    }

    public void SpawnPlayerAtSpawnPoint(int spawnPointId)
    {
        SpawnPoint spawnPoint = GetChildren().OfType<SpawnPoint>().Single(sp => sp.Id == spawnPointId);
        SpawnPlayerAtSpawnPoint(spawnPoint);
    }

    public void SpawnPlayerAtSpawnPoint(SpawnPoint spawnPoint)
    {
        var position = spawnPoint.GlobalPosition;
        var direction = spawnPoint.GetDirectionVector();

        Game.Instance.PlayerInstance?.QueueFree();
        Player = _playerPacked!.Instantiate<Player>();

        Player.GlobalPosition = position;
        Player.FacingDirection = direction;

        CallDeferred("add_child", Player);
    }

    public void SwitchNodeGroups(bool toFuture)
    {
        if (PastNodesGroup != null) PastNodesGroup.Visible = !toFuture;
        if (FutureNodesGroup != null) FutureNodesGroup.Visible = toFuture;
    }

    public async Task FadeToColorAsync(Color color, bool withPlayer, TimeSpan duration)
    {
        // todo
    }

    public void SetFadeColorInstantly(Color color)
    {
        // todo
    }
}
