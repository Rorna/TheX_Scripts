using UnityEngine;

///<summary>
///観測コントローラを持つ
///観察モードとフィールド復帰を管理する
///観察モードのオン・オフを管理する
///</summary>
public class ObservationController
{
    private bool m_controllerActive;
    public bool ObservationMode { get; private set; }
    public string CurrentTargetName { get; private set; } 
    public void Init()
    {
        BindEvent();
    }

    private void BindEvent()
    {
        InputManager.OnObservationModeAction += HandleObservationAction;
        InputManager.OnReturnToFieldAction += HandleReturnToFieldAction;
    }

    private void UnBindEvent()
    {
        InputManager.OnObservationModeAction -= HandleObservationAction;
        InputManager.OnReturnToFieldAction -= HandleReturnToFieldAction;
    }

    public void UpdateActivation(bool isActive)
    {
        m_controllerActive = isActive;

        if(isActive)
            SetCurrentTargetName();
        else
        {
            CurrentTargetName = null;
            ObservationMode = false;

            ObservationManager.Instance.HandleObservationAction(ObservationMode);
        }
    }

    private void SetCurrentTargetName()
    {
        CurrentTargetName = DialogManager.Instance.CurrentDialogObjName;
    }

    private void HandleObservationAction()
    {
        if (m_controllerActive == false)
            return;

        //現在の観察モードのオン/オフ状態を知らせる
        RefreshObservationMode();
    }

    private void HandleReturnToFieldAction()
    {
        Facade.Instance.RequestRunSituationAction(SituationTypeEnum.ReturnToField);
    }

    private void RefreshObservationMode()
    {
        //オンオフを見て判断。オフ状態ならターゲットがあればオンに変更する
        if (ObservationMode == false)
        {
            //ターゲットがある場合にのみ有効
            var currentTarget = Facade.Instance.RequestGetCurrentTarget();
            if (currentTarget == null)
                return;

            //アクションマップチェンジ ->観察モードへ。
            Facade.Instance.RequestSwitchInputMap(InputTypeEnum.ObservationMode);
            ObservationMode = true;
            Facade.Instance.RequestPlaySFX(DefineString.SFX_ObservationModeOn, Vector3.zero, 0.3f, false);
        }
        else //すでにオンの場合はオフに切り替える
        {
            Facade.Instance.RequestSwitchInputMap(InputTypeEnum.Observe);
            ObservationMode = false;
        }

        ObservationManager.Instance.HandleObservationAction(ObservationMode);
        Facade.Instance.RequestPlaySFX(DefineString.SFX_ObservationModeOff, Vector3.zero, 0.3f, false);
    }

    public void Release()
    {
        UnBindEvent();
    }


}
