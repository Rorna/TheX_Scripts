using System;
using System.Collections.Generic;

/// <summary>
/// 手がかりクエスト
/// </summary>
public class ClueQuest : IQuest
{
    public string Title { get; private set; }
    public string Description { get; private set; }
    public string TargetID { get; private set; }
    public int GoalCount { get; private set; }
    public int CurrentCount { get; private set; }
    public HashSet<string> TargetHash { get; private set; }
    public ActionData CompleteAction { get; private set; }

    public event Action OnUpdateQuestAction;
    public event Action OnCompleteQuestAction;
    public void InitQuest(QuestData questData)
    {
        Title = questData.Title;
        Description = questData.Description;
        TargetID = questData.TargetID;

        TargetHash = new HashSet<string>();
        TargetHash = questData.TargetHash;

        GoalCount = TargetHash.Count;

        CompleteAction = new ActionData();
        CompleteAction = questData.CompleteActionData;

        //手がかり獲得イベントの登録
        ClueManager.OnUpdateClueAction += HandleUpdateClueAction;
    }

    private void HandleUpdateClueAction(Dictionary<string, ClueData> clueDic)
    {
        int count = 0;
        foreach (var clueInfo  in clueDic)
        {
            string clueID = clueInfo.Key;
            if(TargetHash.Contains(clueID) == false)
                continue;

            count++;
        }

        CurrentCount = count;
        UpdateQuest();
    }

    public void UpdateQuest()
    {
        if (CurrentCount >= GoalCount)
        {
            CompleteQuest();
            return;
        }

        OnUpdateQuestAction?.Invoke();
    }

    public void CompleteQuest()
    {
        Release();

        OnCompleteQuestAction?.Invoke();

        // 完了 action あった実行行
        if (CompleteAction != null)
        {
            Facade.Instance.RequestExecuteAction(CompleteAction);
        }
    }

    public void Release()
    {
        ClueManager.OnUpdateClueAction -= HandleUpdateClueAction;
    }

    public string GetAdditionalText()
    {
        return $"{CurrentCount} / {GoalCount}";
    }

    public void ImportProgress(QuestProgressData questProgressData)
    {
        CurrentCount = questProgressData.CurrentCount;
    }

    public QuestProgressData ExportProgress()
    {
        QuestProgressData progressData = new QuestProgressData();
        progressData.CurrentCount = CurrentCount;
        return progressData;
    }
}
