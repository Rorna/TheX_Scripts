using UnityEngine;

public class PlayerDialogState : IPlayerActionState
{
    private PlayerActionFSM m_fsm;
    private PlayerController m_playerController;

    public PlayerDialogState(PlayerActionFSM fsm, PlayerController playerController)
    {
        m_fsm = fsm;
        m_playerController = playerController;
    }

    public void Enter()
    {
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

    public void HandleDialogInput(bool isDialogActive)
    {
        if(isDialogActive == false)
            m_fsm.ChangeState(PlayerStateEnum.ActionIdle);
    }

    public void HandleFireInput()
    {
        
    }

    public void HandleReloadInput()
    {
        
    }
}
