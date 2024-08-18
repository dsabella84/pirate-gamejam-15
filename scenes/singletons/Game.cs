using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Godot;

public partial class Game : Node
{
    private readonly CancellationTokenSource _gameExitCts = new();

    public static Game Instance { get; private set; } = null!;

    [Export] public float LevelTransitionDurationSeconds { get; set; } = 2f;
    [Export] public float LanternTransitionDurationSeconds { get; set; } = 1f;

    public TimeSpan LevelTransitionDuration
    {
        get => TimeSpan.FromSeconds(LevelTransitionDurationSeconds);
        set => LevelTransitionDurationSeconds = (float)value.TotalSeconds;
    }

    public TimeSpan LanternTransitionDuration
    {
        get => TimeSpan.FromSeconds(LanternTransitionDurationSeconds);
        set => LanternTransitionDurationSeconds = (float)value.TotalSeconds;
    }

    public PlayerData? LoadedSaveFile { get; private set; } = PlayerData.Debug;
    public LevelBase? CurrentLevel { get; private set; }
    public Player? PlayerInstance { get; set; }
    public bool MovementDisabled { get; set; }

    /// <inheritdoc />
    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest || (what == NotificationWMGoBackRequest && LoadedSaveFile == null)) { _ = ExitGameAsync(false); }
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing) _gameExitCts.Dispose();
    }

    /// <inheritdoc />
    public override void _Ready()
    {
        Instance = this;

        GetTree().QuitOnGoBack = false;
        GetTree().AutoAcceptQuit = false;

        LoadMainMenu();
    }

    public async Task ExitGameAsync(bool saveCurrentGame = true)
    {
        if (LoadedSaveFile != null && saveCurrentGame) await GameFiles.SavePlayerData(LoadedSaveFile, _gameExitCts.Token);

        await _gameExitCts.CancelAsync().ConfigureAwait(false);
        GetTree().Quit();
    }

    public async Task ExitToMainMenuAsync(bool saveCurrentGame = true)
    {
        if (LoadedSaveFile != null && saveCurrentGame)
        {
            await GameFiles.SavePlayerData(LoadedSaveFile, _gameExitCts.Token);
            UnloadLevel();
        }

        LoadMainMenu();
    }

    private void UnloadLevel()
    {
        LoadedSaveFile = null;
        throw new NotImplementedException();
    }

    private void LoadLevel(PlayerData saveFile)
    {
        LoadedSaveFile = saveFile;
        throw new NotImplementedException();
    }

    private void UnloadMainMenu()
    {
        // TODO
    }

    private void LoadMainMenu()
    {
        // TODO
    }

    public void SwitchTime()
    {
        // TODO: fade-out

        // TODO: fade-in
    }

    public async Task TransitToLevelAsync(LevelBase nextLevel, int spawnId)
    {
        MovementDisabled = true;

        bool inFuture = PlayerData.Current?.InFuture ?? true;

        Color fromColor = SelectColorFromLevel(inFuture, CurrentLevel);
        Color toColor = SelectColorFromLevel(inFuture, nextLevel);
        Color middleColor = fromColor.Lerp(toColor, 0.5f);
        TimeSpan halfDuration = LevelTransitionDuration / 2;

        if (CurrentLevel != null)
        {
            await CurrentLevel.FadeToColorAsync(middleColor, true, halfDuration);
            CurrentLevel.QueueFree();
        }

        nextLevel.SpawnPlayerAtSpawnPoint(spawnId);
        nextLevel.SetFadeColorInstantly(middleColor);

        Callable.From(() => GetTree().Root.AddChild(nextLevel)).CallDeferred();
        await nextLevel.FadeToColorAsync(toColor, true, halfDuration);

        MovementDisabled = false;

        return;

        static Color SelectColorFromLevel(bool inFut, LevelBase? levelBase)
        {
            if (inFut) return levelBase?.FutureBackgroundColor ?? Colors.White;

            return levelBase?.PastBackgroundColor ?? Colors.Black;
        }
    }
}
