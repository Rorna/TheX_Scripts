using UnityEngine;

public class PlayerJumpState : IPlayerMovementState
{
    private PlayerMovementFSM m_fsm;
    private PlayerController m_playerController;

    public PlayerJumpState(PlayerMovementFSM fsm, PlayerController playerController)
    {
        m_fsm = fsm;
        m_playerController = playerController;
    }
    public void Enter()
    {
        Facade.Instance.RequestPlaySFX(DefineString.SFX_Jump, Vector3.zero, 0.3f, false);
        m_playerController.Animator.SetInteger("AnimationPar", 1);
        m_playerController.Velocity.y = Mathf.Sqrt(m_playerController.JumpHeight * -2f * m_playerController.Gravity);
    }

    public void Update()
    {
        HandleHorizontalMovement();

        //空中に浮いている間に重力を適用する
        m_playerController.Velocity.y += m_playerController.Gravity * Time.deltaTime;
        m_playerController.CharacterController.Move(m_playerController.Velocity * Time.deltaTime);

        if (m_playerController.CharacterController.isGrounded && m_playerController.Velocity.y < 0)
        {
            m_fsm.ChangeState(PlayerStateEnum.MovementIdle);
        }
    }

    public void FixedUpdate()
    {

    }

    public void Exit()
    {
        m_playerController.Velocity.y = 0;
    }

    private void HandleHorizontalMovement()
    {
        Vector2 moveInput = InputManager.Instance.MoveInput;

        Vector3 forward = CameraManager.Instance.MainCamera.transform.forward;
        Vector3 right = CameraManager.Instance.MainCamera.transform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize     ();
        float currentSpeed = m_playerController.Speed;
        Vector3 moveDirection = (forward * moveInput.y + right * moveInput.x).normalized;

        m_playerController.CharacterController.Move(moveDirection * currentSpeed * Time.deltaTime);

        if (moveDirection.sqrMagnitude > 0.001f)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            m_playerController.transform.rotation = Quaternion.Slerp(m_playerController.transform.rotation, toRotation, 10f * Time.deltaTime);
        }
    }


    public void HandleMoveInput()
    {
        m_fsm.ChangeState(PlayerStateEnum.Move);
    }

    public void HandleJumpInput()
    {
        //無視
    }
    
}
