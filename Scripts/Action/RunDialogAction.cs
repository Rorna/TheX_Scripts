public class RunDialogAction : IAction
{
    private string m_dialogID;

    public ActionTypeEnum ActionType { get; }

    public void InitAction(ActionData actionData)
    {
        m_dialogID = actionData.TargetID;
    }

    public void OnExecute()
    {
        Facade.Instance.RequestStartDialog(m_dialogID);
    }
}
