using UnityEngine;

public class GameOverController
{
    private IGameOver m_gameOver;
    public bool IsGameOver;

    public void Init(IGameOver gameOver)
    {
        m_gameOver = gameOver;

        InitGameOverElement();
        BindEvent();
    }

    private void InitGameOverElement()
    {
        m_gameOver.BindTitleCallback(RequestTitle);
    }

    private void RequestLoadGame()
    {
        Time.timeScale = 1f;
        IsGameOver = false;
        Facade.Instance.RequestLoadGame();
    }

    private void RequestTitle()
    {
        Time.timeScale = 1f;
        IsGameOver = false;
        Facade.Instance.RequestMoveScene(DefineString.MainMenu);
    }

    public void GameOver()
    {
        if (m_gameOver.IsActive)
            return;

        // アクションマップをuiに変更
        Facade.Instance.RequestSwitchInputMap(InputTypeEnum.UI);
        Facade.Instance.RequestPlaySFX(DefineString.SFX_GameOver, Vector3.zero, 0.5f, false);

        m_gameOver.ShowGameOver();
        IsGameOver = true;

        Time.timeScale = 0f;
    }

    private void BindEvent()
    {
        
    }

    private void UnBindEvent()
    {
        
    }

    public void Release()
    {
        UnBindEvent();
    }
}
