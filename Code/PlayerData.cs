using System;
using System.Text.Json.Serialization;

public class PlayerData
{
    public PlayerData(string saveName)
    {
        SaveName = saveName;
    }

    [JsonConstructor]
    public PlayerData(string saveName, Guid saveFileGuid)
    {
        SaveName = saveName;
        SaveFileGuid = saveFileGuid;
    }

#if DEBUG

    [JsonIgnore] public static PlayerData Debug { get; } = new("DEBUG");
#endif

    [JsonIgnore] public static PlayerData? Current => Game.Instance.LoadedSaveFile;

    #region Headers

    public Guid SaveFileGuid { get; } = Guid.NewGuid();
    [JsonIgnore] public string SaveFileName => $"{SaveFileGuid:N}.json";
    public string SaveName { get; set; }
    public bool SoftDeleted { get; set; }

    #endregion

    #region Data

    public bool InFuture { get; set; } = true;
    public bool NewspaperPickedUp { get; set; }
    public bool PolicemanQuestActive { get; set; }
    public bool PartygoerPasswordSeen { get; set; }

    #endregion
}
