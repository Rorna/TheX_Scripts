using UnityEngine;

public class PlayerMoveState : IPlayerMovementState
{
    private PlayerMovementFSM m_fsm;
    private PlayerController m_playerController;

    private LayerMask m_layerMask;
    private GameObject m_groundChecker;
    private bool m_isGrounded;

    //足音タイマー関連変数
    private float m_footstepTimer = 0f;
    private readonly float m_walkStepInterval = 0.55f;
    private readonly float m_runStepInterval = 0.3f;

    public PlayerMoveState(PlayerMovementFSM fsm, PlayerController playerController)
    {
        m_fsm = fsm;
        m_playerController = playerController;
    }

    public void Enter()
    {
        m_playerController.Animator.SetInteger("AnimationPar", 1);
    }

    public void Update()
    {
        UpdateMovement();
        UpdateFootStepSound();
        m_playerController.UpdateGravity();

        //移動入力 ->ムーブ
        if (InputManager.Instance.MoveInput == Vector2.zero)
        {
            m_fsm.ChangeState(PlayerStateEnum.MovementIdle);
        }
    }

    public void FixedUpdate()
    {
    }

    public void Exit()
    {

    }

    public void HandleMoveInput()
    {
        
    }

    public void HandleJumpInput()
    {
        m_fsm.ChangeState(PlayerStateEnum.Jump);
    }

    private void UpdateFootStepSound()
    {
        bool isGrounded = m_playerController.CheckIsGrounded();

        //移動中か確認
        float moveMagnitude = InputManager.Instance.MoveInput.sqrMagnitude;
        bool isMoving = moveMagnitude > 0.01f;

        if (isGrounded && isMoving)
        {
            m_footstepTimer -= Time.deltaTime;
            if (m_footstepTimer <= 0f)
            {
                Facade.Instance.RequestPlaySFX("SFX_PlayerFootstep", m_playerController.transform.position, 1f, false);

                bool isRunning = InputManager.Instance.IsSprinting;
                m_footstepTimer = isRunning ? m_runStepInterval : m_walkStepInterval;
            }
        }
        else
        {
            m_footstepTimer = 0f;
        }
    }

    private void UpdateMovement()
    {
        Vector2 moveInput = InputManager.Instance.MoveInput;
        bool isRunning = InputManager.Instance.IsSprinting;

        Vector3 camForward = CameraManager.Instance.MainCamera.transform.forward;
        Vector3 camRight = CameraManager.Instance.MainCamera.transform.right;

        Vector3 forward = camForward;
        Vector3 right = camRight;

        forward.y = 0;
        right.y = 0;

        //forward 0の場合
        if (forward.sqrMagnitude < 0.001f)
        {
            //カメラ垂直 ->上に設定
            forward = CameraManager.Instance.MainCamera.transform.up;

            //カメラが空を見ているときは、Upベクトルがキャラクターの後ろ側を指すから、方向を反転
            if (camForward.y > 0)
            {
                forward = -forward;
            }
            forward.y = 0;
        }

        //速度の積み重ねを防ぐ
        forward.Normalize();
        right.Normalize();

        float currentSpeed = isRunning ? m_playerController.RunSpeed : m_playerController.Speed;
        Vector3 moveDirection = (forward * moveInput.y + right * moveInput.x).normalized;

        if (float.IsNaN(moveDirection.x) || float.IsNaN(moveDirection.y) || float.IsNaN(moveDirection.z))
        {
            moveDirection = Vector3.zero;
        }

        m_playerController.CharacterController.Move(moveDirection * currentSpeed * Time.deltaTime);

        //方向に回転
        if (moveDirection.sqrMagnitude > 0.001f)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            m_playerController.transform.rotation = Quaternion.Slerp(m_playerController.transform.rotation, toRotation, 10f * Time.deltaTime);
        }
    }
}
