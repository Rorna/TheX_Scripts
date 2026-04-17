
public static class DefineString
{
    #region Path

    //prefab
    public static string MainCameraPath = "Camera/MainCamera";
    public static string FirstCameraPath = "Camera/FirstPersonCamera";
    public static string ThirdCameraPath = "Camera/ThirdPersonCamera";

    public static string UIPath = "UI/";
    public static string UIPopUpPath = "UI/PopUp/";
    public static string ChoiceButtonPath = "UI/ChoiceButton";
    public static string PlayerPath = "Player/Player";
    
    public static string DefaultGun = "DefaultGun";
    public static string ObjectPath = "Object/";


    //json path
    //国籍によってパスを動的に変更
    private static string CurrentLangPath => GameDirector.Instance != null ?
        GameDirector.Instance.CurrentLanguageType.ToString() : "KOR";

    public static string JsonPath => $"Jsons/{CurrentLangPath}";

    //Localization
    public static string JsonDialogPath = $"{JsonPath}/Dialog";
    public static string JsonQuestPath = $"{JsonPath}/Quest"; 
    public static string JsonCluePath = $"{JsonPath}/Clue"; 

    //Common
    public static string JsonActionPath = "Jsons/Common/Action";
    public static string JsonScenePath = "Jsons/Common/Scene";
    public static string JsonObjPath = "Jsons/Common/Object";
    public static string JsonCutscenePath = "Jsons/Common/Cutscene";
    public static string JsonSkillPath = "Jsons/Common/Skill";
    public static string JsonSoundPath = "Jsons/Common/Sound";

    public static string JsonPlayerValuePath = "Jsons/Common/Object/PlayerValue";
    public static string JsonSceneObjectPath = "Jsons/Common/Object/SceneObject";

    //sprite path
    public static string ClueSpritePath = "Sprites/Clue/";


    #endregion Path

    #region file name
    //json
    public static string JsonQuest = "/QuestInfo";
    public static string JsonSituationActionInfo = "/SituationActionInfo";
    public static string JsonAction = "/ActionInfo";
    public static string JsonClue = "/ClueInfo";
    public static string JsonScene = "/SceneInfo";
    public static string JsonPrefabInfo = "/PrefabInfo";
    public static string JsonEnemySpawnInfo = "/EnemySpawnInfo";
    public static string JsonCutsceneInfo = "/CutsceneInfo";
    public static string JsonEnemyInfo = "/EnemyInfo";
    public static string JsonSkillInfo = "/SkillInfo";
    public static string JsonSoundInfo = "/SoundInfo";
    public static string JsonSceneSoundInfo = "/SceneSoundInfo";

    public static string SaveFileName= "SaveData.json";

    //sprite
    public static string ClueEmpty = "Clue_Empty";
    public static string CursorMode = "Mode_Cursor";
    public static string SightMode = "Mode_Sight";


    #endregion

    public static string _Observe = "_Observe";
    public static string StartScene = "Stage1";
    public static string Player = "Player";
    public static string HeadCamera = "HeadCamera";
    public static string _Obj = "_Obj";
    public static string _Enemy = "_Enemy";


    #region keyword

    public static string Observe = "Observe";
    public static string CinemachineBrain = "CinemachineBrain";
    public static string CurrentTarget = "CurrentTarget";
    public static string DummyCamera = "DummyCamera";
    public static string MainMenu = "MainMenu";
    public static string WeaponMountPoint = "WeaponMountPoint";

    //cutscene キーワード
    public static string Scene = "Scene";
    public static string Start = "Start";
    public static string End = "End";
    public static string Effect = "Effect";
    public static string Fade = "Fade";
    public static string Dialog = "Dialog";

    public static string JudgementBullet = "JudgementBullet";
    public static string SaveData = "SaveData";

    public static string ReturnObjectName = "TeleportMachine";
    #endregion


    public static string NewGameConfirmKor = "기존 데이터는 삭제 됩니다. 계속하시겠습니까?";
    public static string NewGameConfirmJpn = "既存のデータは削除されますが、続行しますか？";

    //sfx
    public static string SFX_SummonEnemy = "SFX_SummonEnemy";
    public static string SFX_Damage = "SFX_Damage";
    public static string SFX_GunReload = "SFX_GunReload";
    public static string SFX_ScifiHurt = "SFX_ScifiHurt";
    public static string SFX_RushAttack = "SFX_RushAttack";
    public static string SFX_ShockWave = "SFX_ShockWave";
    public static string SFX_Jump = "SFX_Jump";
    public static string SFX_CluePopup = "SFX_CluePopup";
    public static string SFX_Click = "SFX_Click";
    public static string SFX_PlayerShot = "SFX_PlayerShot";

    public static string SFX_ObservationModeOn = "SFX_ObservationModeOn";
    public static string SFX_ObservationModeOff = "SFX_ObservationModeOff";

    public static string SFX_UISound = "SFX_UISound";
    public static string SFX_ClearQuest = "SFX_ClearQuest";
    public static string SFX_EnemyDead = "SFX_EnemyDead";

    public static string SFX_GameOver = "SFX_GameOver";


}