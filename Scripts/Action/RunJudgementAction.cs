
public class RunJudgementAction : IAction
{
    private string m_xName;
    public ActionTypeEnum ActionType { get; private set; }
    public void InitAction(ActionData actionData)
    {
        m_xName = actionData.TargetID;
        ActionType = actionData.ActionType;
    }

    public void OnExecute()
    {
        Facade.Instance.RequestJudgement(m_xName);
    }
}
