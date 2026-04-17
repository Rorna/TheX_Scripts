///<summary>
///enum
///</summary>

public enum InteractTypeEnum
{
    None,
    NPC,
    Stuff
}

public enum InputTypeEnum
{
    None,
    Field,
    Dialog,
    Observe,
    ObservationMode,
    UI,
    Combat,
    Ending
}


public enum CameraTypeEnum
{
    MainCamera,
    FirstCamera,
    ThirdCamera
}

public enum PlayerStateEnum
{
    MovementIdle,
    ActionIdle,
    Move,
    Jump,
    Dialog,
    Fire,
    Reload
}

public enum EnemyStateEnum
{
    Idle,
    Move,
    Jump,
    Chase,
    Attack
}

///<summary>
///エネミーの状態
///</summary>
public enum EnemyActionEnum
{
    Idle,
    Move,
    Jump,
    Skill
}

///<summary>
///スキルの種類
///</summary>
public enum SkillEnum
{
    MeleeAttack,
    JumpAttack,
    RushAttack,
    ChaseAttack,
    ShockWaveAttack,
    Heal
}

public enum SkillStateEnum
{
    Ready,
    Running,
    Completed,
    Canceled
}

public enum SkillTypeEnum
{
    Damage,
    Heal
}

public enum SkillCastTypeEnum
{
    Melee,
    Range
}


public enum QuestStateEnum
{
    Inactive,
    Progress,
    Complete,
}

public enum QuestTypeEnum
{
    Clue,
    Target,
    Count
}

public enum ActionTypeEnum
{
    GetClue,
    GetQuest,
    GetItem,
    MoveScene,
    RunDialog,
    PlayCutScene,
    RunJudgement,
    RunSave,
    RunLoad,
    GameOver,
    ShowNotify,
    RunEnding,
    Delete
}

public enum SituationTypeEnum
{
    Judgement,
    ReturnToField,
    PlayJudgementCutsceneSuccess,
    PlayJudgementCutsceneFail,
    GameOverZeroBullet
}

public enum SceneTypeEnum
{
    MainMenu,
    Field,
    Observe
}

public enum CreateObjectTypeEnum
{
    Player,
    Field,
    Observe,
    Enemy
}

public enum CutsceneStateEnum
{
    NonPlay,
    Play,
    Pause,
    Resume,
    End
}

public enum SoundTypeEnum
{
    BGM,
    SFX
}

public enum SceneBGMTypeEnum
{
    DefaultBGM,
    CombatBGM
}

public enum DialogBranchTypeEnum
{
    None,
    Quest,
    Save,
    Clue,
    Count
}

public enum TriggerActiveTypeEnum
{
    None,
    Quest,
    Save,
    Clue,
    Count
}

public enum LanguageTypeEnum
{
    None,
    KOR,
    JPN
}

