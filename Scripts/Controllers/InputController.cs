using UnityEngine;
using UnityEngine.InputSystem;

public class InputController
{
    //インプットマップのバインディング、スイッチングマップなど
    private InputSystemAction m_inputMap;
    private InputManager m_manager;

    public Vector2 MoveInput { get; private set; }
    public bool IsSprinting { get; private set; }
    public InputTypeEnum CurrentInputType { get; private set; }

    public void Init(InputManager manager, InputSystemAction inputMap)
    {
        m_manager = manager;
        m_inputMap = inputMap;

        BindingActions();
    }

    public void RefreshController()
    {
        RefreshInputMapByScene();
    }

    #region Bind & Unbind
    private void BindingActions()
    {
        BindingFieldActions();
        BindingDialogActions();
        BindingObserveActions();
        BindingUIActions();
        BindingCombatActions();

        DialogManager.OnDialogStateChangedAction += HandleDialogStateChangeAction;
        GameSceneManager.OnStartLoadSceneAction += StopAllInput;
        CutsceneManager.OnCutsceneStateChangeAction += HandleCutsceneStateChangeAction;
        GameSceneManager.OnCompleteLoadSceneAction += HandleCompleteLoadSceneAction;

        CombatManager.OnCombatActiveAction += HandleCombatActiveAction;
    }

    private void UnBindingActions()
    {
        UnBindingFieldActions();
        UnBindingDialogActions();
        UnBindingObserveActions();
        UnBindingUIActions();
        UnBindingCombatActions();

        DialogManager.OnDialogStateChangedAction -= HandleDialogStateChangeAction;
        GameSceneManager.OnStartLoadSceneAction -= StopAllInput;
        CutsceneManager.OnCutsceneStateChangeAction -= HandleCutsceneStateChangeAction;
        GameSceneManager.OnCompleteLoadSceneAction -= HandleCompleteLoadSceneAction;

        CombatManager.OnCombatActiveAction -= HandleCombatActiveAction;
    }

    private void BindingFieldActions()
    {
        m_inputMap.Field.Move.performed += OnMove;
        m_inputMap.Field.Move.canceled += OnMove;
        m_inputMap.Field.Jump.performed += OnJump;
        m_inputMap.Field.Sprint.performed += OnSprint;
        m_inputMap.Field.Sprint.canceled += OnSprint;
        m_inputMap.Field.Interact.performed += OnInteract;
        m_inputMap.Field.Esc.performed += OnEsc;
        m_inputMap.Field.Clue.performed += OnClue;
    }

    private void UnBindingFieldActions()
    {
        m_inputMap.Field.Move.performed -= OnMove;
        m_inputMap.Field.Move.canceled -= OnMove;
        m_inputMap.Field.Jump.performed -= OnJump;
        m_inputMap.Field.Sprint.performed -= OnSprint;
        m_inputMap.Field.Sprint.canceled -= OnSprint;
        m_inputMap.Field.Interact.performed -= OnInteract;
        m_inputMap.Field.Esc.performed -= OnEsc;
        m_inputMap.Field.Clue.performed -= OnClue;
    }

    private void BindingDialogActions()
    {
        m_inputMap.Dialog.Interact.performed += OnDialog;
    }

    private void UnBindingDialogActions()
    {
        m_inputMap.Dialog.Interact.performed -= OnDialog;
    }

    private void BindingObserveActions()
    {
        m_inputMap.Observe.Move.performed += OnMove;
        m_inputMap.Observe.Move.canceled += OnMove;
        m_inputMap.Observe.Jump.performed += OnJump;
        m_inputMap.Observe.Sprint.performed += OnSprint;
        m_inputMap.Observe.Sprint.canceled += OnSprint;
        m_inputMap.Observe.ObservationMode.performed += OnObservationMode;
        m_inputMap.Observe.JudgementMode.performed += OnJudgement;

        m_inputMap.Observe.Esc.performed += OnEsc;
        m_inputMap.Observe.Interact.performed += OnInteract;

        m_inputMap.ObservationMode.Zoom.performed += OnMouseZoom;
        m_inputMap.ObservationMode.Interact.performed += OnInteract;
        m_inputMap.ObservationMode.Observe.performed += OnObservationMode;
    }

    private void UnBindingObserveActions()
    {
        m_inputMap.Observe.Move.performed -= OnMove;
        m_inputMap.Observe.Move.canceled -= OnMove;
        m_inputMap.Observe.Jump.performed -= OnJump;
        m_inputMap.Observe.Sprint.performed -= OnSprint;
        m_inputMap.Observe.Sprint.canceled -= OnSprint;
        m_inputMap.Observe.ObservationMode.performed -= OnObservationMode;
        m_inputMap.Observe.JudgementMode.performed -= OnJudgement;

        m_inputMap.Observe.Esc.performed -= OnEsc;
        m_inputMap.Observe.Interact.performed -= OnInteract;

        m_inputMap.ObservationMode.Zoom.performed -= OnMouseZoom;
        m_inputMap.ObservationMode.Interact.performed -= OnInteract;
        m_inputMap.ObservationMode.Observe.performed -= OnObservationMode;
    }

    private void BindingUIActions()
    {
        m_inputMap.UI.Esc.performed += OnEsc;
        m_inputMap.UI.Clue.performed += OnClue;
    }

    private void UnBindingUIActions()
    {
        m_inputMap.UI.Esc.performed -= OnEsc;
        m_inputMap.UI.Clue.performed -= OnClue;
    }

    private void BindingCombatActions()
    {
        m_inputMap.Combat.Move.performed += OnMove;
        m_inputMap.Combat.Move.canceled += OnMove;
        m_inputMap.Combat.Jump.performed += OnJump;
        m_inputMap.Combat.Fire.performed += OnFire;
        m_inputMap.Combat.Sprint.performed += OnSprint;
        m_inputMap.Combat.Sprint.canceled += OnSprint;
        m_inputMap.Combat.Esc.performed += OnEsc;
        m_inputMap.Combat.Reload.performed += OnReload;
    }

    private void UnBindingCombatActions()
    {
        m_inputMap.Combat.Move.performed -= OnMove;
        m_inputMap.Combat.Move.canceled -= OnMove;
        m_inputMap.Combat.Jump.performed -= OnJump;
        m_inputMap.Combat.Fire.performed -= OnFire;
        m_inputMap.Combat.Sprint.performed -= OnSprint;
        m_inputMap.Combat.Sprint.canceled -= OnSprint;
        m_inputMap.Combat.Esc.performed -= OnEsc;
        m_inputMap.Combat.Reload.performed -= OnReload;
    }

    #endregion Bind & Unbind


    private void OnMove(InputAction.CallbackContext ctx)
    {
        MoveInput = ctx.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        m_manager.HandleJump();
    }

    private void OnSprint(InputAction.CallbackContext ctx)
    {
        IsSprinting = ctx.ReadValueAsButton();
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        m_manager.HandleInteract();
    }

    private void OnMouseZoom(InputAction.CallbackContext ctx)
    {
        float scroll = ctx.ReadValue<Vector2>().y;
        if (Mathf.Abs(scroll) > 0.0001f)
            m_manager.HandleMouseZoom(scroll);
    }

    private void OnDialog(InputAction.CallbackContext ctx)
    {
        Facade.Instance.RequestPlaySFX(DefineString.SFX_Click, Vector3.zero, 0.3f, false);
        m_manager.HandleDialog();
    }

    private void OnReturnToField(InputAction.CallbackContext ctx)
    {
        m_manager.HandleReturnToField();
    }

    private void OnObservationMode(InputAction.CallbackContext ctx)
    {
        m_manager.HandleObservationMode();
    }

    private void OnJudgement(InputAction.CallbackContext ctx)
    {
        m_manager.HandleJudgement();
    }

    private void OnEsc(InputAction.CallbackContext ctx)
    {
        m_manager.HandleEsc();
    }

    private void OnClue(InputAction.CallbackContext ctx)
    {
        m_manager.HandleClue();
    }

    private void OnReload(InputAction.CallbackContext obj)
    {
        m_manager.HandleReload();
    }

    private void OnFire(InputAction.CallbackContext obj)
    {
        m_manager.HandleFire();
    }

    public void Release()
    {
        UnBindingActions();
    }

    private void RefreshInputMapByScene()
    {
        var sceneType = GameSceneManager.Instance.CurrentSceneType;

        if (sceneType == SceneTypeEnum.Field)
            SwitchInputMap(InputTypeEnum.Field);
        else if (sceneType == SceneTypeEnum.Observe)
            SwitchInputMap(InputTypeEnum.Observe);
        else if (sceneType == SceneTypeEnum.MainMenu)
            SwitchInputMap(InputTypeEnum.UI);
    }

    private void HandleDialogStateChangeAction(bool isDialog)
    {
        if (isDialog)
        {
            SwitchInputMap(InputTypeEnum.Dialog);
        }
        else if (ObservationManager.Instance.ObservationMode)
        {
            SwitchInputMap(InputTypeEnum.ObservationMode);
        }
        else
        {
            RefreshInputMapByScene();
        }
    }

    private void HandleCompleteLoadSceneAction()
    {
        RefreshInputMapByScene();

        MoveInput = Vector2.zero;
        IsSprinting = false;
    }

    private void HandleCutsceneStateChangeAction(CutsceneStateEnum state)
    {
        switch (state)
        {
            case CutsceneStateEnum.Play:
            case CutsceneStateEnum.Resume:
                ActiveCurrentInputMap(false);
                break;

            case CutsceneStateEnum.Pause:
            case CutsceneStateEnum.End:
                ActiveCurrentInputMap(true);
                break;

        }
    }

    private void StopAllInput()
    {
        foreach (var inputAction in m_inputMap.asset)
        {
            inputAction.Disable();
        }
    }

    public void ActiveCurrentInputMap(bool active)
    {
        if (active)
        {
            SwitchInputMap(CurrentInputType);
            return;
        }

        var targetMap = m_inputMap.asset.FindActionMap(CurrentInputType.ToString());
        targetMap.Disable();
    }

    public void SwitchInputMap(InputTypeEnum inputType)
    {
        if (EndingManager.Instance.IsEnding)
            return;

        if (GameOverManager.Instance.IsGameOver)
            return;

        var targetMap = m_inputMap.asset.FindActionMap(inputType.ToString());
        if (targetMap == null)
        {
            return;
        }

        foreach (var map in m_inputMap.asset.actionMaps)
        {
            map.Disable();
        }

        targetMap.Enable();
        CurrentInputType = inputType;

        InputManager.Instance.HandleSwitchActionType();
    }

    private void HandleCombatActiveAction(bool active)
    {
        if (active)
        {
            SwitchInputMap(InputTypeEnum.Combat);
            return;
        }

        RefreshInputMapByScene();
    }
}
