using System.Collections.Generic;
using UnityEngine;

public class PlayerActionFSM
{
    private Dictionary<PlayerStateEnum, IState> m_stateDic;
    public IState CurrentState { get; private set; }
    public PlayerStateEnum CurrentStateEnum { get; private set; }

    public PlayerActionFSM(PlayerController playerController)
    {
        m_stateDic = new Dictionary<PlayerStateEnum, IState>();

        m_stateDic.Add(PlayerStateEnum.ActionIdle, new PlayerActionIdleState(this, playerController));
        m_stateDic.Add(PlayerStateEnum.Dialog, new PlayerDialogState(this, playerController));
        m_stateDic.Add(PlayerStateEnum.Fire, new PlayerFireState(this, playerController));
        m_stateDic.Add(PlayerStateEnum.Reload, new PlayerReloadState(this, playerController));

        ChangeState(PlayerStateEnum.ActionIdle);
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
        CurrentState?.FixedUpdate();
    }

    public void RefreshFSM()
    {
        ChangeState(PlayerStateEnum.ActionIdle);
    }
}
