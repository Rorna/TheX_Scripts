using System;
using System.Collections.Generic;

/// <summary>
/// 手がかりの管理
/// 追加、照会、更新、セーブ・ロード
/// </summary>
public class ClueManager : BaseManager, ISavable
{
    public static ClueManager Instance { get; private set; }
    private Dictionary<string, ClueData> m_clueDataDic;

    public Dictionary<string, ClueData> CurrentClueDic => m_controller.CurrentClueDic;

    public static event Action<Dictionary<string, ClueData>> OnUpdateClueAction; //ui 更新など

    private ClueController m_controller;
    private CluePopupController m_cluePopupController;

    public override void CreateManager()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        Instance = this;
    }

    public override void InternalInitManager()
    {
        m_controller = new ClueController();
        m_controller.Init();

        InitClueData();
    }

    public override void ExternalInitManager()
    {
        Facade.Instance.RequestRegisterSaveData(this);
    }

    private void InitClueData()
    {
        m_clueDataDic = new Dictionary<string, ClueData>();

        string jsonPath = DefineString.JsonCluePath + DefineString.JsonClue;
        string jsonText = JsonUtil.LoadJson(jsonPath);
        var loadedData = JsonUtil.ParseJson<Dictionary<string, List<ClueData>>>(jsonText);

        foreach (var clue in loadedData["clues"])
        {
            m_clueDataDic.TryAdd(clue.ID, clue);
        }
    }

    private void InitCluePopupController()
    {
        m_cluePopupController = new CluePopupController();
        ICluePopup popupInterface = UISystemManager.Instance.GetUIInterface<ICluePopup>();
        m_cluePopupController.Init(popupInterface);
    }

    public void TryGetClue(string clueID)
    {
        if (m_clueDataDic.ContainsKey(clueID) == false)
        {
            return;
        }

        var clueData = m_clueDataDic[clueID];
        m_controller.GetClue(clueID, clueData);
    }

    public bool TryCanGetClue(string clueID)
    {
        if (m_clueDataDic.ContainsKey(clueID) == false)
        {
            return false;
        }

        return m_controller.CanGetClue(clueID);
    }

    public void UpdateClueAction()
    {
        //更新アクション呼び出し
        OnUpdateClueAction?.Invoke(CurrentClueDic);
    }

    public override void RefreshManager()
    {
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

    public void TryShowCluePopup(string clueID)
    {
        //clueid があるか確認
        if (m_clueDataDic.TryGetValue(clueID, out ClueData clueData) == false)
            return;

        //コントローラーがあるか確認
        if (m_cluePopupController == null)
            InitCluePopupController();

        // 手がかりデータをコントローラーに渡す
        m_cluePopupController.ShowPopup(clueData);
    }

    public void TryHideCluePopup()
    {
        if (m_cluePopupController != null)
        {
            m_cluePopupController.HidePopup();
        }
    }
}