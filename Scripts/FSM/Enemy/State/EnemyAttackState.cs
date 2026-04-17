using System;
using UnityEngine;

/// <summary>
/// アタックが終わったらEXIT
/// スキルリストからランダムに選んで攻撃する
/// </summary>
public class EnemyAttackState : IEnemyActionState
{
    private EnemyFSM m_fsm;
    private EnemyBrain m_brain;


    public EnemyAttackState(EnemyFSM fsm, EnemyBrain brain)
    {
        m_fsm = fsm;
        m_brain = brain;
    }

    public void Enter()
    {
        m_brain.ExecuteSkill(HandleSkillComplete);
    }

    private void HandleSkillComplete()
    {
        m_fsm.ChangeState(EnemyStateEnum.Idle);
    }

    public void Update()
    {
        
    }

    public void FixedUpdate()
    {
    }

    public void Exit()
    {
    }


    public void HandleIdle()
    { }

    public void HandleMove()
    { }

    public void HandleJump()
    { }

    public void HandleChase()
    { }

    public void HandleAttack()
    { }
}