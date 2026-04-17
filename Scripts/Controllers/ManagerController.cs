using System.Collections.Generic;
using UnityEngine;

///<summary>
///マネージャたちを管理
///マネージャを生成したり、アップデートやデストロイなどを管理する
///</summary>
public class ManagerController : MonoBehaviour
{
    public static ManagerController Instance { get; private set; }
    private List<BaseManager> m_managerList;

    public void SetManagerControllers()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        m_managerList = new List<BaseManager>();

        CreateManagers(); //create instance
        InitManagers(); //setup variable
        BindEvent();
    }

    private void BindEvent()
    {
        GameSceneManager.OnMidLoadSceneAction += RefreshManagers;
    }

    private void UnBindEvent()
    {
        GameSceneManager.OnMidLoadSceneAction -= RefreshManagers;
    }

    public void RefreshManagers()
    {
        if (m_managerList.Count == 0)
            return;

        foreach (var manager in m_managerList)
        {
            manager.RefreshManager();
        }
    }

    private void ClearManagers()
    {
        if (m_managerList.Count == 0)
            return;

        foreach (var manager in m_managerList)
        {
            manager.ClearManager();
            Destroy(manager.gameObject);
        }
    }

    private void CreateManagers()
    {
        CreateManager<ObjectManager>(nameof(ObjectManager));
        CreateManager<InputManager>(nameof(InputManager));
        CreateManager<CameraManager>(nameof(CameraManager));
        CreateManager<UISystemManager>(nameof(UISystemManager));
        CreateManager<DialogManager>(nameof(DialogManager));
        CreateManager<QuestManager>(nameof(QuestManager));
        CreateManager<ActionManager>(nameof(ActionManager));
        CreateManager<ClueManager>(nameof(ClueManager));
        CreateManager<HUDManager>(nameof(HUDManager));
        CreateManager<GameSceneManager>(nameof(GameSceneManager));
        CreateManager<LoadingManager>(nameof(LoadingManager));
        CreateManager<ObservationManager>(nameof(ObservationManager));
        CreateManager<PlayerManager>(nameof(PlayerManager));
        CreateManager<CutsceneManager>(nameof(CutsceneManager));
        CreateManager<SaveManager>(nameof(SaveManager));
        CreateManager<MainMenuManager>(nameof(MainMenuManager));
        CreateManager<CommonPopupManager>(nameof(CommonPopupManager));
        CreateManager<PauseManager>(nameof(PauseManager));
        CreateManager<GameOverManager>(nameof(GameOverManager));
        CreateManager<ScreenEffectManager>(nameof(ScreenEffectManager));
        CreateManager<CombatManager>(nameof(CombatManager));
        CreateManager<EffectManager>(nameof(EffectManager));
        CreateManager<EnemyManager>(nameof(EnemyManager));
        CreateManager<SoundManager>(nameof(SoundManager));
        CreateManager<EndingManager>(nameof(EndingManager));
    }

    private void CreateManager<T>(string managerName, bool dontDestroy = false) where T : BaseManager
    {
        string managerObjName = "@" + managerName;
        SetManagerObject<T>(managerObjName, dontDestroy, out T manager);
        if (manager == null)
            return;

        manager.CreateManager();
        m_managerList.Add(manager);
    }

    private void InitManagers()
    {
        //setup internal variable
        foreach (var manager in m_managerList)
        {
            manager.InternalInitManager();
        }

        foreach (var manager in m_managerList)
        {
            manager.ExternalInitManager();
        }
    }

    private void SetChildManager(GameObject managerObj)
    {
        managerObj.transform.SetParent(gameObject.transform);
    }

    private void SetManagerObject<T>(string objName, bool dontDestroy, out T manager) where T : UnityEngine.Component
    {
        GameObject go = new GameObject(objName);
        manager = go.AddComponent<T>();

        SetChildManager(go);

        if (dontDestroy)
            DontDestroyOnLoad(go);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        Release();
    }

    public void Release()
    {
        UnBindEvent();
        ClearManagers();
    }
}
