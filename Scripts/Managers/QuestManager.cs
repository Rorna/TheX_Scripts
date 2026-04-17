using System;
using System.Collections.Generic;

/// <summary>
/// クエストの受注と更新通知を担うマネージャー
/// 
/// 設計方針と役割
/// クエストの「受注」のみを行い、状態管理や進行ロジックはすべて各クエスト自身に任せる
/// 本クラスは各クエストの具体的な詳細について気にしない
/// クエスト側からのリクエストに応じて `OnQuestUpdateAction` を呼び出し、
/// システム全体へクエスト更新の通知を行う
/// </summary>
public class QuestManager : BaseManager, ISavable
{
    public static QuestManager Instance { get; private set; }

    private Dictionary<string, QuestData> m_questDic;
    public Dictionary<string, QuestStateEnum> CompleteQuestDic { get; private set; }

    private QuestController m_controller;
    public IQuest CurrentQuest => m_controller.CurrentQuest;
    public QuestStateEnum QuestState => m_controller.QuestState;
    public bool CanGetQuest => CurrentQuest == null;

    public static event Action<IQuest> OnQuestUpdateAction; //ui update

    public override void CreateManager()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }

        Instance = this;
    }

    public override void InternalInitManager()
    {
        InitQuestData();

        m_controller = new QuestController();
        m_controller.Init(this);
        CompleteQuestDic = new Dictionary<string, QuestStateEnum>();
    }

    public override void ExternalInitManager()
    {
        Facade.Instance.RequestRegisterSaveData(this);
    }

    private void InitQuestData()
    {
        m_questDic = new Dictionary<string, QuestData>();

        string jsonPath = DefineString.JsonQuestPath + DefineString.JsonQuest;
        string jsonText = JsonUtil.LoadJson(jsonPath);
        var loadedData = JsonUtil.ParseJson<Dictionary<string, List<QuestData>>>(jsonText);

        foreach (var quest in loadedData["quests"])
        {
            m_questDic.TryAdd(quest.ID, quest);
        }
    }

    public void TryGetQuest(string questID)
    {
        if (m_questDic.TryGetValue(questID, out QuestData questData) == false)
        {
            return;
        }

        m_controller.GetQuest(questData);

        HandleUpdateQuestInfo();
    }

    public void HandleUpdateQuestInfo()
    {
        OnQuestUpdateAction?.Invoke(CurrentQuest);
    }

    public void CompleteQuest()
    {
        HandleUpdateQuestInfo();
        CompleteQuestDic.TryAdd(m_controller.CurrentQuestID, QuestStateEnum.Complete);
        m_controller.ClearQuest();
    }

    public void SetCompleteQuestDic(Dictionary<string, QuestStateEnum> dic)
    {
        CompleteQuestDic = dic;
    }


    public bool IsQuestComplete(string questID)
    {
        if (m_questDic.TryGetValue(questID, out QuestData questData) == false)
            return false;

        if(CompleteQuestDic.ContainsKey(questID) ==false)
            return false;

        return true;
    }

    public override void RefreshManager()
    {
        m_controller.RefreshController();
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

    public QuestData GetQuestData(string questID)
    {
        if (m_questDic.TryGetValue(questID, out var questData) == false)
            return null;

        return questData;
    }
}
