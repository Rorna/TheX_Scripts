public class SaveController
{
    private bool m_loadRequest;
    private SaveData m_cachedSaveData;
    private SaveManager m_manager;

    public void Init(SaveManager manager)
    {
        m_manager = manager;
        BindEvent();
    }

    public void SaveGame(SaveData saveData)
    {
        string fileName = DefineString.SaveFileName;
        JsonUtil.SaveToJson(saveData, fileName);

        m_manager.SetSaveData();
    }

    public void LoadGame(SaveData saveData)
    {
        //セーブデータシーンリクエスト
        m_cachedSaveData = saveData;

        string savedSceneName = m_cachedSaveData.SceneData.CurrentSceneName;
        Facade.Instance.RequestMoveScene(savedSceneName);

        m_loadRequest = true;
    }

    private void SetLoadData()
    {
        if (m_loadRequest == false || m_cachedSaveData == null)
            return;

        var savableList = SaveManager.Instance.SavableList;
        foreach (var savable  in savableList)
        {
            savable.LoadGameData(m_cachedSaveData);
        }

        m_loadRequest = false;
    }

    public void DeleteGame()
    {
        string fileName = DefineString.SaveFileName;
        JsonUtil.DeleteSaveDataJson(fileName);

        m_cachedSaveData = null;
    }

    public void RefreshController()
    {
        if (GameSceneManager.Instance.CurrentSceneType == SceneTypeEnum.MainMenu)
        {
            m_cachedSaveData = null;

        }
    }

    private void BindEvent()
    {
        GameSceneManager.OnCompleteLoadSceneAction += SetLoadData;
    }

    private void UnBindEvent()
    {
        GameSceneManager.OnCompleteLoadSceneAction -= SetLoadData;
    }

    public void Release()
    {
        UnBindEvent();
    }

}
