using System;
using System.Collections.Generic;

public class DialogManager : BaseManager
{
    public static DialogManager Instance { get; private set; }
    public static event Action<bool> OnDialogStateChangedAction;

    private Dictionary<string, DialogData> m_dialogDataDic;

    private DialogController m_controller;

    public string CurrentDialogObjName => m_controller.CurrentDialogObjName;
    public bool IsOnDialog => m_controller.IsOnDialog;

    #region Init

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
        InitDialogData();
    }

    public override void ExternalInitManager()
    {
        InitController();
    }

    private void InitDialogData()
    {
        Dictionary<string, string> allJsonDic = JsonUtil.LoadAllJsonFiles(DefineString.JsonDialogPath);

        m_dialogDataDic = new Dictionary<string, DialogData>();
        foreach (var json in allJsonDic)
        {
            string fileName = json.Key;
            string jsonText = json.Value;

            DialogData dialogFile = JsonUtil.ParseJson<DialogData>(jsonText);
            if (dialogFile != null && m_dialogDataDic.ContainsKey(fileName) == false)
            {
                m_dialogDataDic.Add(fileName, dialogFile);
            }
        }

    }

    #endregion Init

    public void InitController()
    {
        m_controller = new DialogController();
        var uiInterface = UISystemManager.Instance.GetUIInterface<IDialog>();
        m_controller.Init(uiInterface);
    }

    public void TryStartDialog(string dialogID)
    {
        if (m_dialogDataDic.TryGetValue(dialogID, out DialogData dialogData) == false)
        {
            return;
        }

        m_controller.StartDialog(dialogData);
    }

    public void HandleDialogStateChange(bool isDialog)
    {
        OnDialogStateChangedAction?.Invoke(isDialog);
    }

    public override void RefreshManager()
    {
        m_controller.RefreshController();
    }

    public override void ClearManager()
    {
        m_controller.Release();
    }
}