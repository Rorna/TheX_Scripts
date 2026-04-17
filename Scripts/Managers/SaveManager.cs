using System.Collections.Generic;

public class SaveManager : BaseManager
{
    public static SaveManager Instance { get; private set; }

    public List<ISavable> SavableList { get; private set; }

    private SaveData m_gameSaveData;
    private SaveController m_controller;

    public bool HasSaveData => (m_gameSaveData != null);

    public SaveData GameSaveData
    {
        get
        {
            SetSaveData();
            return m_gameSaveData;
        }
    }

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
        InitController();
        SavableList = new List<ISavable>();
    }

    public override void ExternalInitManager()
    {
    }

    private void InitController()
    {
        m_controller = new SaveController();
        m_controller.Init(this);
    }

    public void SetSaveData()
    {
        SaveData data = JsonUtil.LoadSaveDataFromJson<SaveData>(DefineString.SaveFileName);
        if(data != null)
            m_gameSaveData = data;
    }

    public void TrySaveGame()
    {
        SaveData saveData = new SaveData();

        // 登録済みの各セーブ対象に対して、セーブ処理をリクエストする。
        foreach (var savable in SavableList)
        {
            savable.SaveGameData(saveData);
        }

        m_controller.SaveGame(saveData);
    }

    public void TryLoadGame()
    {
        // save data をロードして設定する
        SetSaveData();
        if (m_gameSaveData == null)
        {
            UnityUtil.ShowNoticePopUp("There is No Save Data!!!");
            return;
        }

        // コントローラーにロードゲームリクエストをする
        m_controller.LoadGame(m_gameSaveData);
    }

    public void TryRegisterSaveData(ISavable save)
    {
        SavableList.Add(save);
    }

    public void TryDeleteSaveData()
    {
        m_controller.DeleteGame();
        m_gameSaveData = null;
    }

    public override void RefreshManager()
    {
        if (GameSceneManager.Instance.CurrentSceneType == SceneTypeEnum.MainMenu)
            m_gameSaveData = null;

        m_controller.RefreshController();
    }

    public override void ClearManager()
    {
        m_controller.Release();
    }
}
