
public class ShowNotifyAction : IAction
{
    public ActionTypeEnum ActionType { get; }

    private ActionData m_actionData;
    public void InitAction(ActionData actionData)
    {
        m_actionData = actionData;
    }

    public void OnExecute()
    {
        Facade.Instance.RequestNoticePopup(m_actionData.TargetID);
    }
}
