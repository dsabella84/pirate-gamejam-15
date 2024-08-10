using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

public static partial class GameFiles
{
    public const string GAME_NAME = "Sunfall";
    public const string STUDIO_NAME = "RemotePeopleGames";

    static GameFiles()
    {
        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        GameDataPath = Path.Combine(appDataPath, STUDIO_NAME, GAME_NAME);
        PlayerSaveFolderPath = Path.Combine(GameDataPath, "Saves");
    }

    public static string GameDataPath { get; }
    public static string PlayerSaveFolderPath { get; }

    public static async Task<PlayerData?> LoadPlayerData(string filePath, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(filePath)) return null;

        FileStream readStream = File.OpenRead(filePath);
        await using ConfiguredAsyncDisposable stream = readStream.ConfigureAwait(false);
        PlayerData? data = await JsonSerializer
            .DeserializeAsync(readStream, ConfiguredJsonSerializedContext.Default.PlayerData, cancellationToken)
            .ConfigureAwait(false);

        return data;
    }

    public static async Task<GameSettings> LoadSettings(CancellationToken cancellationToken = default)
    {
        string filePath = Path.Combine(GameDataPath, "settings.json");
        if (!File.Exists(filePath)) return new GameSettings();

        FileStream readStream = File.OpenRead(filePath);
        await using ConfiguredAsyncDisposable stream = readStream.ConfigureAwait(false);
        GameSettings? settings = await JsonSerializer
            .DeserializeAsync(readStream,
                ConfiguredJsonSerializedContext.Default.GameSettings,
                cancellationToken)
            .ConfigureAwait(false);

        return settings ?? new GameSettings();
    }

    public static async Task SavePlayerData(PlayerData data, CancellationToken cancellationToken = default)
    {
        string filePath = Path.Combine(PlayerSaveFolderPath, data.SaveFileName);

        FileStream writeStream = File.OpenWrite(filePath);
        await using ConfiguredAsyncDisposable stream = writeStream.ConfigureAwait(false);
        await JsonSerializer
            .SerializeAsync(writeStream, data, ConfiguredJsonSerializedContext.Default.GameSettings, cancellationToken)
            .ConfigureAwait(false);
    }

    public static async Task SaveSettings(GameSettings settings, CancellationToken cancellationToken = default)
    {
        string filePath = Path.Combine(GameDataPath, "settings.json");

        FileStream writeStream = File.OpenWrite(filePath);
        await using ConfiguredAsyncDisposable stream = writeStream.ConfigureAwait(false);
        await JsonSerializer
            .SerializeAsync(writeStream, settings, ConfiguredJsonSerializedContext.Default.GameSettings, cancellationToken)
            .ConfigureAwait(false);
    }

    public static async IAsyncEnumerable<PlayerData> LoadAllSaveFiles([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(PlayerSaveFolderPath)) yield break;

        string[] files = Directory.GetFiles(PlayerSaveFolderPath);
        foreach (string filePath in files)
        {
            PlayerData? data = await LoadPlayerData(filePath, cancellationToken);
            if (data != null) yield return data;
        }
    }

    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(GameSettings))]
    [JsonSerializable(typeof(PlayerData))]
    private partial class ConfiguredJsonSerializedContext : JsonSerializerContext;
}
