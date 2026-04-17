using UnityEngine;

///<summary>
///ジャンプ後に床に触れるとEXIT
///一時的にデュレーションを3秒間設定して3秒後に終了させる
///</summary>
public class EnemyJumpState : IEnemyActionState
{
    private EnemyFSM m_fsm;
    private EnemyBrain m_brain;
    private EnemyController m_controller;

    private float m_jumpTimer; //ジャンプ直後に無視タイマー

    public EnemyJumpState(EnemyFSM fsm, EnemyBrain brain)
    {
        m_fsm = fsm;
        m_brain = brain;
        m_controller = brain.Controller;
    }

    public void Enter()
    {
        m_jumpTimer = 0f;

        m_controller.PerformJump();
        m_controller.Anim.SetTrigger("Flip");
    }

    public void Update()
    {
        //着地判定、Y速度が -、下降　+　床
        if (m_controller.RigidBody.linearVelocity.y <= 0.1f && m_controller.IsGrounded())
        {
            m_fsm.ChangeState(EnemyStateEnum.Idle);
        }
    }

    public void FixedUpdate()
    {
        m_jumpTimer += Time.deltaTime;

        //ジャンプ直後の着地判定を無視、グラウンドチェックが地面から外れる時間を稼ぐ。
        if (m_jumpTimer < 0.15f)
            return;
    }

    public void Exit()
    {
    }

    public void HandleIdle()
    {
    }

    public void HandleMove()
    {
    }

    public void HandleJump()
    {
    }

    public void HandleChase()
    {
    }

    public void HandleAttack()
    {
    }
}
