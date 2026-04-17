using System;
using UnityEngine;

/// <summary>
/// システム全体の「窓口」として機能するクラス。
/// 外部からの機能要求は、必ず本クラスを経由させる設計となっている（Facadeパターン）。
/// 
/// 外部からリクエストを受け取ると、本クラスが仲介役となり、
/// そのリクエストを処理すべき適切なマネージャーへ渡す
/// 
/// 命名規則
/// Request ~~~
/// </summary>
public class Facade : MonoBehaviour
{
    public static Facade Instance { get; private set; }

    public void Init()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void RequestGetQuest(string questID)
    {
        QuestManager.Instance.TryGetQuest(questID);
    }

    public void RequestStartDialog(string dialogID)
    {
        DialogManager.Instance.TryStartDialog(dialogID);
    }

    public void RequestGetClue(string clueID)
    {
        ClueManager.Instance.TryGetClue(clueID);
    }

    public bool RequestCanGetClue(string clueID)
    {
        return ClueManager.Instance.TryCanGetClue(clueID);
    }

    public void RequestExecuteAction(ActionData actionData)
    {
        ActionManager.Instance.TryExecuteAction(actionData);
    }

    public void RequestMoveScene(string sceneName)
    {
        GameSceneManager.Instance.TryMoveScene(sceneName);
    }

    public Interactable RequestGetCurrentTarget()
    {
        return PlayerManager.Instance.TryGetCurrentTarget();
    }

    public void RequestSetPlayerCamera(GameObject player)
    {
        CameraManager.Instance.TrySetPlayerTarget(player);
    }

    public void RequestSwitchInputMap(InputTypeEnum inputType)
    {
        InputManager.Instance.TrySwitchInputMap(inputType);
    }

    public void RequestRunSituationAction(SituationTypeEnum type)
    {
        ActionManager.Instance.TryExecuteSituationAction(type);
    }

    public void RequestJudgement(string xName)
    {
        ObservationManager.Instance.TryJudgement(xName);
    }

    public void RequestPlayCutscene(string cutsceneName)
    {
        CutsceneManager.Instance.TryPlayCutscene(cutsceneName);
    }

    public void RequestHandleCutsceneSignal(string key, string value)
    {
        CutsceneManager.Instance.TryHandleSignal(key, value);
    }

    public GameObject RequestSceneObject(string objName)
    {
        return ObjectManager.Instance.TryGetSceneGameObject(objName);
    }

    public void RequestRefreshJudgementBullet(int count)
    {
        PlayerManager.Instance.TryRefreshJudgementBullet(count);
    }

    public void RequestRegisterSaveData(ISavable save)
    {
        SaveManager.Instance.TryRegisterSaveData(save);
    }

    public void RequestSaveGame()
    {
        SaveManager.Instance.TrySaveGame();
    }

    public void RequestLoadGame()
    {
        SaveManager.Instance.TryLoadGame();
    }

    public string RequestGetPrefabName(string objName)
    {
        return ObjectManager.Instance.TryGetPrefabName(objName);
    }

    public void RequestControlPopupSortOrder(UIPopup popup, bool isShow)
    {
        UISystemManager.Instance.TryControlPopupSortOrder(popup, isShow);
    }

    public void RequestShowConfirmPopup(string text, Action confirmAction)
    {
        CommonPopupManager.Instance.TryShowConfirmPopup(text, confirmAction);
    }

    public void RequestRunGameOver()
    {
        GameOverManager.Instance.TryGameOver();
    }

    public void RequestNoticePopup(string text)
    {
        CommonPopupManager.Instance.TryNoticePopup(text);
    }

    public void RequestScreenFade(float duration = 0.5f)
    {
        ScreenEffectManager.Instance.TryFadeSequence(duration);
    }

    public void RequestScreenFadeIn(float duration, Action onComplete = null)
    {
        ScreenEffectManager.Instance.TryFadeIn(duration, onComplete);
    }

    public void RequestScreenFadeOut(float duration, Action onComplete = null)
    {
        ScreenEffectManager.Instance.TryFadeOut(duration, onComplete);
    }

    public void RequestShowCluePopup(string clueID)
    {
        ClueManager.Instance.TryShowCluePopup(clueID);
    }

    public void RequestHideCluePopup()
    {
        ClueManager.Instance.TryHideCluePopup();
    }

    public void RequestActiveObject(string objName, bool active)
    {
        ObjectManager.Instance.TryActiveObject(objName, active);
    }

    public void RequestPlayEffect<T>(T prefab, Vector3 createPos, Vector3 createDir,
        bool usePool, int poolCapacity = 20, Action<T> InitAction = null, Transform parent = null) where T : Component
    {
        EffectManager.Instance.TryPlayEffect(prefab, createPos, 
            createDir, usePool, poolCapacity, InitAction, parent);
    }

    public void RequestPlayTrailEffect(BulletTrail prefab, Vector3 startPos, 
        Vector3 endPos, bool usePool, int poolCapacity = 20)
    {
        EffectManager.Instance.TryPlayBulletTrail(prefab, startPos, 
            endPos, usePool, poolCapacity);
    }

    public void RequestSetupEnemy(GameObject obj)
    {
        EnemyManager.Instance.TrySetupEnemy(obj);
    }

    public void RequestDestroyParticle(ParticleSystem particle)
    {
        EffectManager.Instance.TryDestroyParticle(particle);
    }

    public void RequestPlayBGM(string soundID, float volume, bool fadeIn, float duration = 0f)
    {
        SoundManager.Instance.TryPlayBGM(soundID, volume, fadeIn, duration);
    }

    public void RequestPlaySceneBGM(SceneBGMTypeEnum bgmType,
        float volume, bool fadeIn, float fadeDuration = 0f)
    {
        SoundManager.Instance.TryPlaySceneBGM(bgmType, volume, fadeIn, fadeDuration);
    }

    public void RequestPlaySFX(string soundID, Vector3? position, float volume, 
        bool fadeOut, float duration = 0f)
    {
        SoundManager.Instance.TryPlaySFX(soundID, position, volume, fadeOut, duration);
    }

    public void RequestStopBGM(bool fadeOut, float fadeDuration)
    {
        SoundManager.Instance.TryStopBGM(fadeOut, fadeDuration);
    }

    public void RequestPlayDamageFeedback()
    {
        HUDManager.Instance.TryPlayDamageFeedback();
    }

    public void RequestStartCombat(string targetName)
    {
        CombatManager.Instance.TryStartCombat(targetName);
    }

    public GameObject RequestSpawnEnemy(string enemyName)
    {
        GameObject enemy = ObjectManager.Instance.TrySpawnEnemy(enemyName);
        return enemy;
    }

    public void RequestSaveTrigger(string triggerName)
    {
        PlayerManager.Instance.TrySaveTrigger(triggerName);
    }

    public void RequestDeleteSaveData()
    {
        SaveManager.Instance.TryDeleteSaveData();
    }

    public void RequestRegisterInactive(string objName)
    {
        ObjectManager.Instance.TryRegisterInactive(objName);
    }

    public void RequestEnding()
    {
        EndingManager.Instance.TryEnding();
    }

    public void RequestCameraShake()
    {
        CameraManager.Instance.TryGenerateCameraShake();
    }

    public void RequestActiveInput(bool active)
    {
        InputManager.Instance.TryActiveInput(active);
    }
}
