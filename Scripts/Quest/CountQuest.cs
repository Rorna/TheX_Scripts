using System;
using System.Collections.Generic;

/// <summary>
/// クリア条件が数の場合
/// </summary>
public class CountQuest : IQuest
{
    public string Title { get; private set; }
    public string Description { get; private set; }
    public string TargetID { get; private set; }
    public int GoalCount { get; private set; }
    public int CurrentCount { get; private set; }
    public HashSet<string> TargetHash { get; }
    public ActionData CompleteAction { get; }

    public event Action OnUpdateQuestAction;
    public event Action OnCompleteQuestAction;

    public void InitQuest(QuestData questData)
    {
        // set data
        Title = questData.Title;
        Description = questData.Description;
        TargetID = questData.TargetID;
        GoalCount = questData.GoalCount;
        CurrentCount = 0;

        DialogController.OnDialogTargetAction += OnUpdateQuest;
    }

    private void OnUpdateQuest(string objName)
    {
        string prefabName = Facade.Instance.RequestGetPrefabName(objName);
        if (prefabName != TargetID)
            return;

        UpdateQuest();
    }

    public void UpdateQuest()
    {
        CurrentCount++;
        if (CurrentCount >= GoalCount)
        {
            CompleteQuest();
            return;
        }

        OnUpdateQuestAction?.Invoke();
    }

    public void CompleteQuest()
    {
        OnCompleteQuestAction?.Invoke();

        Release();
    }

    public void Release()
    {
        DialogController.OnDialogTargetAction -= OnUpdateQuest;
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