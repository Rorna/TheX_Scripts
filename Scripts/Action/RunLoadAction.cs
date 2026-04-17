
public class RunLoadAction : IAction
{
    public ActionTypeEnum ActionType { get; }
    public void InitAction(ActionData actionData)
    {
        
    }

    public void OnExecute()
    {
        Facade.Instance.RequestLoadGame();
    }
}
