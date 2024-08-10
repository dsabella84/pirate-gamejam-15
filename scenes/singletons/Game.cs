using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Godot;

public partial class Game : Node
{
    private readonly CancellationTokenSource _gameExitCts = new();

    public static Game Instance { get; private set; } = null!;

    public PlayerData? LoadedSaveFile { get; private set; }
    public LevelBase? CurrentLevel { get; private set; }
    public Player? PlayerInstance { get; set; }

    /// <inheritdoc />
    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest || (what == NotificationWMGoBackRequest && !(LoadedSaveFile != null))) { _ = ExitGameAsync(false); }
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
        throw new NotImplementedException();
    }

    private void LoadMainMenu()
    {
        throw new NotImplementedException();
    }

    public void SwitchTime()
    {
        throw new NotImplementedException();
    }
}
