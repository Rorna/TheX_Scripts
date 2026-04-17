
public class PlayerActionIdleState : IPlayerActionState
{
    private PlayerActionFSM m_fsm;
    private PlayerController m_playerController;

    public PlayerActionIdleState(PlayerActionFSM fsm, PlayerController playerController)
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
        if(isDialogActive)
            m_fsm.ChangeState(PlayerStateEnum.Dialog);
    }

    public void HandleFireInput()
    {
        m_fsm.ChangeState(PlayerStateEnum.Fire);
    }

    public void HandleReloadInput()
    {
        m_fsm.ChangeState(PlayerStateEnum.Reload);
    }
}
