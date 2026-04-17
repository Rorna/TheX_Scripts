// すべてのアクションクラスはこのインタフェースを実装すること
public interface IAction
{
    public ActionTypeEnum ActionType { get; }
    public void InitAction(ActionData actionData);
    public void OnExecute();
}