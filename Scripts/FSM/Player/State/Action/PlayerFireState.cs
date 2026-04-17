using UnityEngine;

public class PlayerFireState : IPlayerActionState
{
    private PlayerActionFSM m_fsm;
    private PlayerController m_playerController;

    private float m_timer;
    private float m_fireDuration;

    public PlayerFireState(PlayerActionFSM fsm, PlayerController playerController)
    {
        m_fsm = fsm;
        m_playerController = playerController;
    }

    public void Enter()
    {
        m_timer = 0f;

        var currentWeapon = m_playerController.WeaponHandler.CurrentWeapon;
        m_fireDuration = currentWeapon != null ? currentWeapon.FireRate : 0.5f;

        m_playerController.WeaponHandler.TryFire();
    }

    public void Update()
    {
        m_timer += Time.deltaTime;
        if (m_timer >= m_fireDuration)
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
        //武器がリロード可能か判定（残弾がMax未満、かつリロード中でない場合）
        var currentWeapon = m_playerController.WeaponHandler.CurrentWeapon;
        if (currentWeapon != null && currentWeapon.CurrentAmmo < currentWeapon.MaxAmmo && currentWeapon.IsReloading == false)
        {
            //射撃ディレイ中でもリロード状態に即座に切り替える
            m_fsm.ChangeState(PlayerStateEnum.Reload);
        }
    }
}
