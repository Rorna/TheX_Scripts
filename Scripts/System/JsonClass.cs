using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// json クラス
/// </summary>

[JsonObject]
public class DialogData
{
    [JsonProperty("startNodeID")]
    public string StartNodeID { get; private set; }

    [JsonProperty("nodes")]
    public Dictionary<string, DialogNode> Nodes { get; private set; }
}

[JsonObject]
public class DialogNode
{
    [JsonProperty("text")]
    public string Text { get; private set; }

    [JsonProperty("speaker")]
    public string Speaker { get; private set; }

    [JsonProperty("emotion")]
    public string Emotion { get; private set; }

    [JsonProperty("sound")]
    public string Sound { get; private set; }

    [JsonProperty("action")]
    public ActionData Action { get; private set; }

    // flow control
    [JsonProperty("nextNodeID")]
    public string NextNodeID { get; private set; }

    [JsonProperty("choices")]
    public List<DialogChoice> Choices { get; private set; }


    // branch
    [JsonProperty("branchType")]
    public DialogBranchTypeEnum BranchType { get; private set; }
    
    [JsonProperty("branchValue")]
    public string BranchValue { get; private set; }

    [JsonProperty("failTargetNodeID")]
    public string FailTargetNodeID { get; private set; }

    // talker
    [JsonProperty("talker")]
    public string Talker { get; private set; }
}

[JsonObject]
public class DialogChoice
{
    [JsonProperty("text")]
    public string Text { get; private set; }

    [JsonProperty("targetNodeID")]
    public string TargetNodeID { get; private set; }


    // branch
    [JsonProperty("branchType")]
    public DialogBranchTypeEnum BranchType { get; private set; }

    [JsonProperty("branchValue")]
    public string BranchValue { get; private set; }

    [JsonProperty("failTargetNodeID")]
    public string FailTargetNodeID { get; private set; }
}

[JsonObject]
public class QuestData
{
    [JsonProperty("id")]
    public string ID { get; private set; }

    [JsonProperty("questType")]
    public QuestTypeEnum QuestType { get; private set; }

    [JsonProperty("title")]
    public string Title { get; private set; }

    [JsonProperty("description")]
    public string Description { get; private set; }

    // Target
    [JsonProperty("targetID")]
    public string TargetID { get; private set; }

    [JsonProperty("goalCount")]
    public int GoalCount { get; private set; }

    [JsonProperty("targetList")]
    public HashSet<string> TargetHash { get; private set; }

    [JsonProperty("completeAction")]
    public ActionData CompleteActionData { get; private set; }
}

[JsonObject]
public class ActionData
{
    [JsonProperty("actionType")]
    public ActionTypeEnum ActionType { get; private set; }

    [JsonProperty("targetID")]
    public string TargetID { get; private set; }

    public void SetActionType(ActionTypeEnum actionType)
    {
        ActionType = actionType;
    }

    public void SetTargetID(string targetID)
    {
        TargetID = targetID;
    }
}

[JsonObject]
public class SituationActionInfo
{
    [JsonProperty("situationActionData")]
    public Dictionary<SituationTypeEnum, ActionData> SituationActionDic { get; private set; }
}

[JsonObject]
public class ActionInfo
{
    [JsonProperty("actionInfo")]
    public HashSet<ActionTypeEnum> ActionHash { get; private set; } //重複防止のため hash.
}

[JsonObject]
public class ClueData
{
    [JsonProperty("id")]
    public string ID { get; private set; }

    [JsonProperty("title")]
    public string Title { get; private set; }

    [JsonProperty("description")]
    public string Description { get; private set; }

    // Target
    [JsonProperty("spriteID")]
    public string SpriteID { get; private set; }
}

[JsonObject]
public class SceneInfo
{
    [JsonProperty("sceneInfo")]
    public Dictionary<string, SceneData> SceneInfoDic { get; private set; } // 重複防止のため hash
}

[JsonObject]
public class SceneData
{
    [JsonProperty("sceneType")]
    public SceneTypeEnum SceneType { get; private set; }
}

[JsonObject]
public class PlayerInfo
{
    [JsonProperty("speed")]
    public float Speed { get; private set; }

    [JsonProperty("runSpeed")]
    public float RunSpeed { get; private set; }

    [JsonProperty("jumpHeight")]
    public float JumpHeight { get; private set; }

    [JsonProperty("gravity")]
    public float Gravity { get; private set; }

    [JsonProperty("HP")]
    public int HP { get; private set; }

    [JsonProperty("firstPersonReach")]
    public float FirstPersonReach { get; private set; }

    [JsonProperty("thirdPersonRange")]
    public float ThirdPersonRange { get; private set; }

    [JsonProperty("interactionLayer")]
    public string[] InteractionLayer { get; private set; }

    [JsonProperty("interactInterval")]
    public float InteractInterval { get; private set; }

    [JsonProperty("judgementBullet")]
    public int JudgementBullet { get; private set; }
}

[JsonObject]
public class SceneObjectData
{
    [JsonProperty("objectList")]
    public Dictionary<string, ObjectData> SceneObjDic { get; private set; }
}

[JsonObject]
public class ObjectInfo
{
    [JsonProperty("objectInfo")]
    public Dictionary<string, ObjectData> ObjInfoDic { get; private set; }
}

[JsonObject]
public class ObjectData
{
    [JsonProperty("objectType")]
    public CreateObjectTypeEnum ObjectType { get; private set; }

    [JsonProperty("prefabName")]
    public string PrefabName { get; private set; }

    [JsonProperty("path")]
    public string Path { get; private set; }

    [JsonProperty("spawnPos")]
    public Vector3 SpawnPos { get; private set; }

    [JsonProperty("rotation")]
    public Vector3 Rotation { get; private set; }

    [JsonProperty("tag")]
    public string Tag { get; private set; }

    [JsonProperty("layer")]
    public string Layer { get; private set; }

    public void SetPrefabName(string prefabName)
    {
        if (PrefabName != string.Empty)
            return;

        PrefabName = prefabName;
    }
}

[JsonObject]
public class DialogPathInfo // All dialog json files
{
    [JsonProperty("dialogID")]
    public string DialogID { get; set; }

    [JsonProperty("path")]
    public string Path { get; set; }
}

[JsonObject]
public class CutsceneInfo
{
    [JsonProperty("cutsceneInfo")]
    public Dictionary<string, CutsceneData> cutsceneInfoDic { get; private set; }
}

[JsonObject]
public class CutsceneData
{
    [JsonProperty("path")]
    public string Path { get; private set; }

    [JsonProperty("trackDynamicBindings")] 
    public Dictionary<string, string> TrackDynamicBindingDic { get; private set; } //key: trackName, value: prefabName
}

[JsonObject]
public class EnemyInfo
{
    [JsonProperty("EnemyInfo")]
    public Dictionary<string, EnemyData> EnemyInfoDic { get; private set; }
}

[JsonObject]
public class EnemyData
{
    [JsonProperty("HP")]
    public int HP { get; private set; }

    [JsonProperty("Attack")]
    public int Attack { get; private set; }

    [JsonProperty("Speed")]
    public float Speed { get; private set; }

    [JsonProperty("JumpHeight")]
    public float JumpHeight { get; private set; }

    [JsonProperty("Gravity")]
    public float Gravity { get; private set; }

    [JsonProperty("MaxActionInterval")]
    public float MaxActionInterval { get; private set; }

    [JsonProperty("MinActionInterval")]
    public float MinActionInterval { get; private set; }

    // move values
    [JsonProperty("BaseMoveDistance")]
    public float BaseMoveDistance { get; private set; }

    [JsonProperty("MaxMoveDistance")]
    public float MaxMoveDistance { get; private set; }

    [JsonProperty("MinMoveDistance")]
    public float MinMoveDistance { get; private set; }

    [JsonProperty("MaxMoveAngle")]
    public float MaxMoveAngle { get; private set; }

    [JsonProperty("MinMoveAngle")]
    public float MinMoveAngle { get; private set; }

    [JsonProperty("MaxMoveDuration")]
    public float MaxMoveDuration { get; private set; }

    [JsonProperty("MinMoveDuration")]
    public float MinMoveDuration { get; private set; }

    [JsonProperty("Action")]
    public List<EnemyActionEnum> ActionList { get; private set; }

    [JsonProperty("Skill")]
    public List<SkillEnum> SkillList { get; private set; }
}

[JsonObject]
public class SkillInfo
{
    [JsonProperty("SkillInfo")] 
    public Dictionary<SkillEnum, SkillData> SkillInfoDic { get; private set; }
}

[JsonObject]
public class SkillData
{
    [JsonProperty("SkillType")]
    public SkillTypeEnum SkillType { get; private set; }

    [JsonProperty("SkillCastType")]
    public SkillCastTypeEnum SkillCastType { get; private set; }

    [JsonProperty("Damage")]
    public int Damage { get; private set; }

    [JsonProperty("DelayTime")]
    public float DelayTime { get; private set; }

    // RushAttack Values
    [JsonProperty("RushDuration")]
    public float RushDuration { get; private set; }

    [JsonProperty("RushForce")]
    public float RushForce { get; private set; }

    [JsonProperty("AttackCount")]
    public int AttackCount { get; private set; }

    // ShockWave Values
    [JsonProperty("ScaleUpTime")]
    public float ScaleUpTime { get; private set; } // スケールの更新間隔

    [JsonProperty("ScaleUpParam")]
    public float ScaleUpParam { get; private set; } // スケールの増加量

    [JsonProperty("LifeTime")]
    public float LifeTime { get; private set; }

    [JsonProperty("ParticlePath")]
    public string ParticlePath { get; private set; }
}

[JsonObject]
public class SoundInfo
{
    [JsonProperty("SoundInfo")] 
    public Dictionary<string, SoundData> SoundInfoDic { get; private set; }
}

[JsonObject]
public class SoundData
{
    [JsonProperty("SoundID")]
    public string SoundID { get; private set; }

    [JsonProperty("Path")]
    public string Path { get; private set; }

    [JsonProperty("SoundType")]
    public SoundTypeEnum SoundType { get; private set; }

    [JsonProperty("Loop")]
    public bool Loop { get; private set; }

    [JsonProperty("DefaultVolume")]
    public float DefaultVolume { get; private set; }

    [JsonProperty("SpatialBlendRatio")]
    public float SpatialBlendRatio { get; private set; }
}

[JsonObject]
public class SceneSoundInfo
{
    [JsonProperty("SceneSoundInfo")]
    public Dictionary<string, Dictionary<SceneBGMTypeEnum, SceneBGMData>> SceneSoundDic { get; private set; }
}


[JsonObject]
public class SceneBGMData
{
    [JsonProperty("SoundID")]
    public string SoundID { get; private set; }

    [JsonProperty("Volume")]
    public float Volume { get; private set; }
}

