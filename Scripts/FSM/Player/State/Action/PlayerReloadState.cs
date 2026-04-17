using UnityEngine;

public class PlayerReloadState : IPlayerActionState
{
    private PlayerActionFSM m_fsm;
    private PlayerController m_playerController;

    private float m_timer;
    private float m_reloadTime;

    public PlayerReloadState(PlayerActionFSM fsm, PlayerController playerController)
    {
        m_fsm = fsm;
        m_playerController = playerController;
    }

    public void Enter()
    {
        m_timer = 0f;

        var currentWeapon = m_playerController.WeaponHandler.CurrentWeapon;
        m_reloadTime = currentWeapon != null ? currentWeapon.ReloadTime : 1.0f;

        m_playerController.WeaponHandler.TryReload();
    }

    public void Update()
    {
        m_timer += Time.deltaTime;
        if (m_timer >= m_reloadTime)
        {
            m_fsm.ChangeState(PlayerStateEnum.ActionIdle);
        }
    }

    public void FixedUpdate()
    {
    }

    public void Exit()
    {
        
    }

    public void HandleDialogInput(bool isDialogActive)
    {
        
    }

    public void HandleFireInput()
    {
        
    }

    public void HandleReloadInput()
    {
        
    }
}
