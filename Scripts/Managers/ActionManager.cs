using System.Collections.Generic;

public class ActionManager : BaseManager
{
    public static ActionManager Instance { get; private set; }
    private HashSet<ActionTypeEnum> m_actionHash; //すべてのアクションが含まれているハッシュ

    private ActionController m_controller;

    private Dictionary<SituationTypeEnum, ActionData> m_situationActionDic;

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
        InitActionHash();
        InitSituationActionDic();

        m_controller = new ActionController();
        m_controller.Init();
    }

    public override void ExternalInitManager()
    {
    }

    private void InitSituationActionDic()
    {
        m_situationActionDic = new Dictionary<SituationTypeEnum, ActionData>();

        //path
        string jsonPath = DefineString.JsonActionPath + DefineString.JsonSituationActionInfo;
        string jsonText = JsonUtil.LoadJson(jsonPath);

        var loadedData = JsonUtil.ParseJson<SituationActionInfo>(jsonText);
        m_situationActionDic = loadedData.SituationActionDic;
    }

    private void InitActionHash()
    {
        m_actionHash = new HashSet<ActionTypeEnum>();

        string jsonPath = DefineString.JsonActionPath + DefineString.JsonAction;
        string jsonText = JsonUtil.LoadJson(jsonPath);
        var loadedData = JsonUtil.ParseJson<ActionInfo>(jsonText);

        m_actionHash = loadedData.ActionHash;
    }

    public void TryExecuteAction(ActionData actionData)
    {
        if (actionData == null)
            return;

        if (m_actionHash.Contains(actionData.ActionType) == false)
            return;

        m_controller.ExecuteAction(actionData);
    }

    public void TryExecuteSituationAction(SituationTypeEnum type)
    {
        if (m_situationActionDic.TryGetValue(type, out var situationActionData) == false)
        {
            return;
        }

        m_controller.ExecuteAction(situationActionData);
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