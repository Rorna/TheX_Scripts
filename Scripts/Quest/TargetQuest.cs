using System;
using System.Collections.Generic;

///<summary>
///クリア条件がターゲット ID の場合。
///ターゲットIDが削除された場合はクリア
///</summary>
public class TargetQuest : IQuest
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

        CompleteAction = new ActionData();
        CompleteAction = questData.CompleteActionData;

        EnemyManager.OnEnemyDeadAction += HandleUpdateDeadEnemyAction;
    }

    private void HandleUpdateDeadEnemyAction(EnemyController enemy)
    {
        if (enemy.name != TargetID)
            return;

        CompleteQuest();
    }

    public void UpdateQuest()
    {
    }

    public void CompleteQuest()
    {
        Release();

        OnCompleteQuestAction?.Invoke();

        //完了アクションあったら実行。
        if (CompleteAction != null)
        {
            Facade.Instance.RequestExecuteAction(CompleteAction);
        }
    }

    public void Release()
    {
        EnemyManager.OnEnemyDeadAction -= HandleUpdateDeadEnemyAction;
    }

    public string GetAdditionalText()
    {
        return string.Empty;
    }

    public void ImportProgress(QuestProgressData questProgressData)
    {
        
    }

    public QuestProgressData ExportProgress()
    {
        QuestProgressData progressData = new QuestProgressData();
        progressData.CurrentCount = CurrentCount;
        return progressData;
    }
}
