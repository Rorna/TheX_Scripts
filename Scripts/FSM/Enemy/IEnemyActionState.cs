using UnityEngine;

/// <summary>
/// 外部制御用.
/// </summary>
public interface IEnemyActionState : IState
{
    public void HandleIdle();
    public void HandleMove();
    public void HandleJump();
    public void HandleChase();
    public void HandleAttack();
}
