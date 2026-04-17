///<summary>
///すべてのStateはこのインタフェースを実装しなければならない
///</summary>
public interface IState
{
    public void Enter();
    public void Update();
    public void FixedUpdate();
    public void Exit();
}
