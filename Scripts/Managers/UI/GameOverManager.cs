using UnityEngine;

public class GameOverManager : BaseManager
{
    public static GameOverManager Instance { get; private set; }
    private GameOverController m_controller;

    public bool IsGameOver => m_controller.IsGameOver;
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
        
    }

    public override void ExternalInitManager()
    {
        InitController();
    }

    private void InitController()
    {
        m_controller = new GameOverController();
        IGameOver uiInterface = UISystemManager.Instance.GetUIInterface<IGameOver>();
        m_controller.Init(uiInterface);
    }

    public void TryGameOver()
    {
        m_controller.GameOver();
    }

    public override void RefreshManager()
    {
        
    }

    public override void ClearManager()
    {
       
    }
}
