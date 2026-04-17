using System.Collections.Generic;

public class ClueController
{
    public Dictionary<string, ClueData> CurrentClueDic { get; private set; }

    private const int MaxAmount = 3;

    public void Init()
    {
        CurrentClueDic = new Dictionary<string, ClueData>();
    }

    public void GetClue(string clueID, ClueData clueData)
    {
        CurrentClueDic.Add(clueID, clueData);

        ClueManager.Instance.UpdateClueAction();
    }

    public bool CanGetClue(string clueID)
    {
        if (CurrentClueDic.Count == MaxAmount)
            return false;

        if (CurrentClueDic.ContainsKey(clueID))
            return false;

        return true;
    }
    public void SaveGame(SaveData saveData)
    {
        ClueSaveData clueData = new ClueSaveData();
        clueData.ClueDic = CurrentClueDic;

        saveData.ClueData = clueData;
    }

    public void LoadGame(SaveData saveData)
    {
        ClueSaveData clueData = saveData.ClueData;
        if (clueData.ClueDic == null)
            return;

        CurrentClueDic.Clear();
        CurrentClueDic = clueData.ClueDic;

        ClueManager.Instance.UpdateClueAction();
    }

    public void ClearClue()
    {
        CurrentClueDic.Clear();
        ClueManager.Instance.UpdateClueAction();
    }

    public void Release()
    {
        CurrentClueDic.Clear();
    }

}
