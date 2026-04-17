using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// save 関連クラス
/// </summary>

[JsonObject]
public class SaveData
{
    [JsonProperty("playerData")]
    public PlayerSaveData PlayerData;

    [JsonProperty("clueData")]
    public ClueSaveData ClueData;

    [JsonProperty("questData")]
    public QuestSaveData QuestData;

    [JsonProperty("sceneData")]
    public SceneSaveData SceneData;

    [JsonProperty("objectData")] 
    public ObjectSaveData ObjectData;
}

public class ObjectSaveData
{
    [JsonProperty("inactiveHash")] 
    public HashSet<string> InactiveObjectHash;
}

[JsonObject]
public class PlayerSaveData
{
    // maxHP
    [JsonProperty("MaxHP")]
    public int MaxHP;

    // pos
    [JsonProperty("pos")]
    public Vector3 Pos;

    // angle
    [JsonProperty("angle")]
    public Vector3 Angle;

    // JudgementBullet
    [JsonProperty("judgementBullet")]
    public int JudgementBullet;

    [JsonProperty("triggerInfo")] 
    public HashSet<string> TriggerInfoHash;
}

[JsonObject]
public class ClueSaveData
{
    // clue
    [JsonProperty("clueDic")]
    public Dictionary<string, ClueData> ClueDic;
}

[JsonObject]
public class QuestSaveData
{
    // quest
    [JsonProperty("completeQuestDic")]
    public Dictionary<string, QuestStateEnum> CompleteQuestDic;

    [JsonProperty("currentQuestID")]
    public string CurrentQuestID;

    [JsonProperty("questProgress")]
    public QuestProgressData QuestProgress;

    [JsonProperty("questState")]
    public QuestStateEnum QuestState;
}

[JsonObject]
public class QuestProgressData
{
    [JsonProperty("questID")]
    public string QuestID;

    [JsonProperty("currentCount")]
    public int CurrentCount;
}

[JsonObject]
public class SceneSaveData
{
    [JsonProperty("currentSceneType")]
    public SceneTypeEnum CurrentSceneType;

    [JsonProperty("currentSceneName")]
    public string CurrentSceneName;
}
