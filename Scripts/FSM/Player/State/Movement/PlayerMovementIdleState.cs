using UnityEngine;

public class PlayerMovementIdleState : IPlayerMovementState
{
    private PlayerMovementFSM m_fsm;
    private PlayerController m_playerController;

    public PlayerMovementIdleState(PlayerMovementFSM fsm, PlayerController playerController)
    {
        m_fsm = fsm;
        m_playerController = playerController;
    }

    public void Enter()
    {
        m_playerController.Animator.SetInteger("AnimationPar", 0);
    }

    public void Update()
    {
        //移動入力が入ると、Move 状態に切り替える
        if (InputManager.Instance.MoveInput != Vector2.zero)
        {
            m_fsm.ChangeState(PlayerStateEnum.Move);
        }
    }

    public void FixedUpdate()
    {
        //地面にくっつくように重力を弱く適用し続ける
        m_playerController.CharacterController.Move(new Vector3(0, m_playerController.Gravity, 0) * Time.deltaTime);
    }

    public void Exit() { }

    public void HandleJumpInput()
    {
        m_fsm.ChangeState(PlayerStateEnum.Jump);
    }

    public void HandleMoveInput()
    {
        m_fsm.ChangeState(PlayerStateEnum.Move);
    }
}
