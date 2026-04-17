using UnityEngine;

///<summary>
///ブレインからランダム値を抜き、そのランダム値をマックスでまたランダム値を取得。
///それで持続時間設定。
///持続時間が過ぎると、EXIT
///</summary>
public class EnemyIdleState : IEnemyActionState
{
    private EnemyFSM m_fsm;
    private EnemyBrain m_brain;

    private EnemyController m_controller;

    public EnemyIdleState(EnemyFSM fsm, EnemyBrain brain)
    {
        m_fsm = fsm;
        m_brain = brain;
        m_controller = brain.Controller;
    }

    public void Enter()
    {
        m_controller.StopMovement();

        m_controller.Anim.SetBool("IsWalk", false);
        m_brain.ResetInterval();
    }

    public void Update()
    {
    }

    public void FixedUpdate()
    {
        m_controller.StopMovement();
    }

    public void Exit()
    {
    }

    //---Idle状態の場合は、外部のHandleリクエストを受け入れてステータスを変更する---

    public void HandleIdle()
    {
        //ランダム値でまたIdleが選ばれたらインターバル再びリセット
        m_brain.ResetInterval();
    }

    public void HandleMove()
    {
        m_fsm.ChangeState(EnemyStateEnum.Move);
    }

    public void HandleJump()
    {
        m_fsm.ChangeState(EnemyStateEnum.Jump);
    }

    public void HandleChase()
    {
        m_fsm.ChangeState(EnemyStateEnum.Chase);
    }

    public void HandleAttack()
    {
        m_fsm.ChangeState(EnemyStateEnum.Attack);
    }
}