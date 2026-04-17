using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : BaseManager, ISavable
{
    public static GameSceneManager Instance { get; private set; }

    private Dictionary<string, SceneData> m_sceneInfoDic;
    private GameSceneController m_controller;
    public SceneTypeEnum CurrentSceneType => m_controller.CurrentSceneType;
    public string CurrentSceneName => m_controller.CurrentSceneName;
    
    public static event Action<SceneTypeEnum> OnSceneTypeChangeAction;
    public static event Action OnStartLoadSceneAction; // フェードインとか...
    public static event Action OnMidLoadSceneAction; // マネージャーのアップデートとか...
    public static event Action OnCompleteLoadSceneAction; // hud の変更とか...

    private bool m_isFirstLoad = true;

    public override void CreateManager()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public override void InternalInitManager()
    {
        InitSceneHash();
        InitController();
    }

    public override void ExternalInitManager()
    {
        Facade.Instance.RequestRegisterSaveData(this);
    }

    private void InitSceneHash()
    {
        m_sceneInfoDic = new Dictionary<string, SceneData>();

        // path
        string jsonPath = DefineString.JsonScenePath + DefineString.JsonScene;
        string jsonText = JsonUtil.LoadJson(jsonPath);

        var loadedData = JsonUtil.ParseJson<SceneInfo>(jsonText);

        m_sceneInfoDic = loadedData.SceneInfoDic;
    }

    private void InitController()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (m_sceneInfoDic.TryGetValue(sceneName, out SceneData sceneData) == false)
        {
            return;
        }

        m_controller = new GameSceneController();
        m_controller.Init(sceneName, sceneData);
    }

    public void TryMoveScene(string sceneName)
    {
        if (m_sceneInfoDic.TryGetValue(sceneName, out SceneData sceneData) == false)
            return;

        m_controller.MoveScene(sceneName, sceneData);
    }

    public void HandleStartLoadScene()
    {
        OnStartLoadSceneAction?.Invoke();
    }
    public void HandleMidLoadScene()
    {
        OnMidLoadSceneAction?.Invoke();
    }

    public void HandleCompleteLoadScene()
    {
        Time.timeScale = 1f;

        OnCompleteLoadSceneAction?.Invoke();
    }

    public void HandleSceneTypeChange()
    {
        OnSceneTypeChangeAction?.Invoke(CurrentSceneType);
    }

    public override void RefreshManager()
    {
        if (m_isFirstLoad)
        {
            m_isFirstLoad = false;
            HandleCompleteLoadScene();
        }
    }

    public override void ClearManager()
    {
        m_controller.Release();
    }

    public void SaveGameData(SaveData saveData)
    {
        m_controller.SaveGame(saveData);
    }

    public void LoadGameData(SaveData saveData)
    {
        m_controller.LoadGame(saveData);
    }
}
