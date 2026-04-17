using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementFSM
{
    private Dictionary<PlayerStateEnum, IState> m_stateDic;
    public IState CurrentState { get; private set; }
    public PlayerStateEnum CurrentStateEnum { get; private set; }

    public PlayerMovementFSM(PlayerController playerController)
    {
        m_stateDic = new Dictionary<PlayerStateEnum, IState>();

        m_stateDic.Add(PlayerStateEnum.MovementIdle, new PlayerMovementIdleState(this, playerController));
        m_stateDic.Add(PlayerStateEnum.Move, new PlayerMoveState(this, playerController));
        m_stateDic.Add(PlayerStateEnum.Jump, new PlayerJumpState(this, playerController));

        ChangeState(PlayerStateEnum.MovementIdle);
    }

    public void ChangeState(PlayerStateEnum newState)
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

    }

    public void RefreshFSM()
    {
        ChangeState(PlayerStateEnum.MovementIdle);
    }
}
