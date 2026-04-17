
public class RunEndingAction : IAction
{
    private ActionData m_actionData;
    public ActionTypeEnum ActionType { get; }
    public void InitAction(ActionData actionData)
    {
        m_actionData = actionData;
    }

    public void OnExecute()
    {
        Facade.Instance.RequestEnding();
    }
}
