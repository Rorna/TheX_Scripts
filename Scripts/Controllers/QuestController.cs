using UnityEngine;

public class QuestController
{
    private QuestManager m_manager;
    public string CurrentQuestID { get; private set; }
    public IQuest CurrentQuest { get; private set; }
    public QuestStateEnum QuestState { get; private set; }

    public void Init(QuestManager manager)
    {
        m_manager = manager;
    }

    public void GetQuest(QuestData data)
    {
        ClearQuest();
        CurrentQuestID = data.ID;
        CreateQuest(data);
    }

    private void CreateQuest(QuestData data)
    {
        IQuest quest = null;
        switch (data.QuestType)
        {
            case QuestTypeEnum.Count:
                quest = new CountQuest();
                break;

            case QuestTypeEnum.Target:
                quest = new TargetQuest();
                break;
            case QuestTypeEnum.Clue:
                quest = new ClueQuest();
                break;
        }

        if (quest == null)
            return;

        quest.InitQuest(data);
        quest.OnUpdateQuestAction += UpdateQuest;
        quest.OnCompleteQuestAction += CompleteQuest;

        CurrentQuest = quest;
        QuestState = QuestStateEnum.Progress;
    }

    private void UpdateQuest()
    {
        m_manager.HandleUpdateQuestInfo();
    }

    public void RefreshController()
    {
        if (GameSceneManager.Instance.CurrentSceneType == SceneTypeEnum.MainMenu)
        {
            ClearQuest();
        }
    }

    private void CompleteQuest()
    {
        Facade.Instance.RequestPlaySFX(DefineString.SFX_ClearQuest, Vector3.zero, 0.4f, false);

        QuestState = QuestStateEnum.Complete;
        m_manager.CompleteQuest();
    }

    public void ClearQuest()
    {
        if (CurrentQuest != null)
        {
            CurrentQuest.OnUpdateQuestAction -= UpdateQuest;
            CurrentQuest.OnCompleteQuestAction -= CompleteQuest;
            CurrentQuest.Release();
        }
            
        CurrentQuest = null;
        CurrentQuestID = null;
    }

    public void SaveGame(SaveData saveData)
    {
        QuestSaveData questData = new QuestSaveData();

        //現在のクエストデータ設定
        if (CurrentQuest != null)
        {
            QuestProgressData questProgress = CurrentQuest.ExportProgress();
            questData.QuestProgress = questProgress;
            questData.QuestProgress.QuestID = CurrentQuestID;
        }

        questData.CurrentQuestID = CurrentQuestID;
        questData.QuestState = QuestState;
        questData.CompleteQuestDic = QuestManager.Instance.CompleteQuestDic;

        saveData.QuestData = questData;
    }

    public void LoadGame(SaveData saveData)
    {
        //クエストの生成
        QuestSaveData questData = saveData.QuestData;
        if (questData.QuestProgress == null && 
            (questData.CompleteQuestDic == null || questData.CompleteQuestDic.Count == 0) && 
            questData.QuestState == QuestStateEnum.Inactive)
            return;

        ClearQuest();

        m_manager.SetCompleteQuestDic(questData.CompleteQuestDic);

        //進行中のクエストがあればクエスト設定
        CurrentQuestID = questData.CurrentQuestID;
        if (CurrentQuestID == null)
            return;

        QuestData data = m_manager.GetQuestData(CurrentQuestID);
        if (data == null)
            return;

        CreateQuest(data);

        //現在のクエスト進捗情報の設定
        QuestProgressData questProgress = questData.QuestProgress;
        CurrentQuest.ImportProgress(questProgress);

        QuestState = questData.QuestState;
        m_manager.HandleUpdateQuestInfo();
    }

    public void Release()
    {

    }

    
}
