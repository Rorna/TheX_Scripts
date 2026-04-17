public class GameOverAction : IAction
{
    public ActionTypeEnum ActionType { get; private set; }

    public void InitAction(ActionData actionData)
    {
    }

    public void OnExecute()
    {
        Facade.Instance.RequestRunGameOver();
    }
}