using UnityEngine;

public class PauseController
{
    private IPause m_pause;
    private InputTypeEnum m_cachedInputType;

    public void Init(IPause uiInterface)
    {
        m_pause = uiInterface;
        
        InitPauseElement();
        BindEvent();
    }

    private void BindEvent()
    {
        InputManager.OnEscAction += HandleEscAction;
    }

    private void InitPauseElement()
    {
        m_pause.BindContinueCallback(HandleEscAction);
        m_pause.BindLoadCallback(RequestLoadGame);
        m_pause.BindTitleCallback(RequestTitle);
    }

    private void RequestLoadGame()
    {
        Facade.Instance.RequestPlaySFX(DefineString.SFX_UISound, Vector3.zero, 0.3f, false);
        ClearPauseState();
        Facade.Instance.RequestLoadGame();
    }

    private void RequestTitle()
    {
        Facade.Instance.RequestPlaySFX(DefineString.SFX_UISound, Vector3.zero, 0.3f, false);

        ClearPauseState();
        Facade.Instance.RequestMoveScene(DefineString.MainMenu);
    }

    private void HandleEscAction()
    {
        var currentScene = GameSceneManager.Instance.CurrentSceneType;
        if (currentScene == SceneTypeEnum.MainMenu)
            return;

        Facade.Instance.RequestPlaySFX(DefineString.SFX_UISound, Vector3.zero, 0.3f, false);

        if (m_pause.IsActive)
        {
            ClearPauseState();
        }
        else
        {
            SetUIInteraction(true);
            m_pause.ShowPause();

            //一時停止
            Time.timeScale = 0f;
        }
    }

    //uiモード切替
    //input type, cursor 設定のため。
    private void SetUIInteraction(bool active)
    {
        if (active)
        {
            //現在の入力タイプを保存
            m_cachedInputType = InputManager.Instance.CurrentInputType;

            //inputswitch　ー＞　UIへ。
            Facade.Instance.RequestSwitchInputMap(InputTypeEnum.UI);
        }
        else
        {
            //キャッシュした入力タイプで入力タイプを切替
            Facade.Instance.RequestSwitchInputMap(m_cachedInputType);
        }
    }

    private void ClearPauseState()
    {
        if (m_pause.IsActive)
        {
            SetUIInteraction(false);
            m_pause.HidePause();
            Time.timeScale = 1f;
        }
    }

    private void UnBindEvent()
    {
        InputManager.OnEscAction -= HandleEscAction;
    }

    public void Release()
    {
        UnBindEvent();
    }
}
