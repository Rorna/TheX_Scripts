public class GetQuestAction : IAction
{
    public ActionTypeEnum ActionType { get; private set; }
    private string m_questID;

    public void InitAction(ActionData actionData)
    {
        m_questID = actionData.TargetID;
    }

    public void OnExecute()
    {
        Facade.Instance.RequestGetQuest(m_questID);
    }
}
