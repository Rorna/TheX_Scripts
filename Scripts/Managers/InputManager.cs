using System;
using UnityEngine;

/// <summary>
/// ユーザー入力を統括・管理するマネージャー。
/// 
/// 主な役割とデータの流れ
/// インプットマップを保持し、入力イベントを検知してコントローラーへ伝達する。
/// 入力に対する実際のロジック処理はコントローラー側に委譲している。
/// 外部から入力値（MoveInputなど）の要求があった場合は、
/// 本マネージャーが仲介役となり、コントローラーから値を取得して外部へ提供する。
/// </summary>
public class InputManager : BaseManager
{
    public static InputManager Instance { get; private set; }

    private InputSystemAction m_inputMap;
    private InputController m_controller;

    //input event
    public static event Action OnInteractAction; //対話（UI、オブジェクトなど）
    public static event Action OnJumpAction;     //ジャンプ
    public static event Action<float> OnMouseZoomAction; //マウスズーム(float値渡し)
    public static event Action OnDialogAction; //会話の進行
    public static event Action OnObservationModeAction;
    public static event Action OnJudgementAction;
    public static event Action OnReturnToFieldAction;
    public static event Action OnEscAction;
    public static event Action OnClueAction;
    public static event Action OnReloadAction;
    public static event Action OnFireAction;

    public static event Action OnSwitchInputMap;

    public Vector2 MoveInput => m_controller.MoveInput;
    public bool IsSprinting => m_controller.IsSprinting;
    public InputTypeEnum CurrentInputType => m_controller.CurrentInputType;

    public override void CreateManager()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public override void InternalInitManager()
    {
        InitInputMap();
        InitController();
    }

    public override void ExternalInitManager()
    {
    }

    private void InitInputMap()
    {
        if (m_inputMap == null)
        {
            m_inputMap = new InputSystemAction();
            m_inputMap.Enable();
        }
    }

    private void InitController()
    {
        m_controller = new InputController();
        m_controller.Init(this, m_inputMap);
    }

    public void HandleJump()
    {
        OnJumpAction?.Invoke();
    }

    public void HandleInteract()
    {
        OnInteractAction?.Invoke();
    }

    public void HandleDialog()
    {
        OnDialogAction?.Invoke();
    }

    public void HandleMouseZoom(float value)
    {
        OnMouseZoomAction?.Invoke(value);
    }

    public void HandleObservationMode()
    {
        OnObservationModeAction?.Invoke();
    }

    public void HandleReturnToField()
    {
        OnReturnToFieldAction?.Invoke();
    }

    public void HandleJudgement()
    {
        OnJudgementAction?.Invoke();
    }

    public void HandleEsc()
    {
        OnEscAction?.Invoke();
    }

    public void HandleClue()
    {
        OnClueAction?.Invoke();
    }

    public void HandleSwitchActionType()
    {
        OnSwitchInputMap?.Invoke();
    }

    public void HandleReload()
    {
        OnReloadAction?.Invoke();
    }

    public void HandleFire()
    {
        OnFireAction?.Invoke();
    }

    public void TrySwitchInputMap(InputTypeEnum inputType)
    {
        m_controller.SwitchInputMap(inputType);
    }

    public void TryActiveInput(bool active)
    {
        m_controller.ActiveCurrentInputMap(active);
    }

    public override void RefreshManager()
    {
        m_controller.RefreshController();
    }

    public override void ClearManager()
    {
        m_controller.Release();
    }

}