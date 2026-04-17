public class GetClueAction : IAction
{
    private string m_clueID;

    public ActionTypeEnum ActionType { get; }

    public void InitAction(ActionData actionData)
    {
        m_clueID = actionData.TargetID;
    }

    public void OnExecute()
    {
        // クルーマネージャーにリクエスト
        Facade.Instance.RequestGetClue(m_clueID);
    }
}
