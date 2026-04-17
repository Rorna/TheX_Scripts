using System.Collections.Generic;
using UnityEngine;

public class EnemyFSM
{
    private EnemyBrain m_brain;

    private Dictionary<EnemyStateEnum, IState> m_stateDic;
    public IState CurrentState { get; private set; }
    public EnemyStateEnum CurrentStateEnum { get; private set; }

    public EnemyFSM(EnemyBrain brain)
    {
        m_brain = brain;

        m_stateDic = new Dictionary<EnemyStateEnum, IState>();

        m_stateDic.Add(EnemyStateEnum.Idle, new EnemyIdleState(this, m_brain));
        m_stateDic.Add(EnemyStateEnum.Move, new EnemyMoveState(this, m_brain));
        m_stateDic.Add(EnemyStateEnum.Jump, new EnemyJumpState(this, m_brain));
        m_stateDic.Add(EnemyStateEnum.Attack, new EnemyAttackState(this, m_brain));

        ChangeState(EnemyStateEnum.Idle);
    }

    public void ChangeState(EnemyStateEnum newState)
    {
        CurrentState?.Exit();

        CurrentState = m_stateDic[newState];
        CurrentStateEnum = newState;

        CurrentState.Enter();
    }

    public void Update()
    {
        CurrentState?.Update();
    }

    public void FixedUpdate()
    {
        CurrentState?.FixedUpdate();
    }

    public void RefreshFSM()
    {
        ChangeState(EnemyStateEnum.Idle);
    }
}
