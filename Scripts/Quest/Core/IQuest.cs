using System;
using System.Collections.Generic;

///<summary>
///すべてのクエストクラスはこのインタフェースを実装しなければならない。
///</summary>
public interface IQuest
{
    //UIがアクセスしなければならないデータ
    public string Title { get; }
    public string Description { get; }
    public string TargetID { get; }
    public int GoalCount { get; }
    public int CurrentCount { get; }
    public HashSet<string> TargetHash { get; }
    public ActionData CompleteAction { get; }

    public event Action OnUpdateQuestAction;
    public event Action OnCompleteQuestAction;

    public void InitQuest(QuestData questData);

    public void UpdateQuest();

    public void CompleteQuest();
    public void Release();

    public string GetAdditionalText();

    public void ImportProgress(QuestProgressData questProgressData);
    public QuestProgressData ExportProgress();
}