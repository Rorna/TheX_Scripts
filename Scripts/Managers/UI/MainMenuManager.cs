/// <summary>
/// MainMenu UI 管理マネージャー
/// </summary>
public class MainMenuManager : BaseManager
{
    public MainMenuManager Instance { get; private set; }
    private MainMenuController m_controller;
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
        m_controller = new MainMenuController();
        IMainMenu uiInterface = UISystemManager.Instance.GetUIInterface<IMainMenu>();
        m_controller.Init(uiInterface);
    }

    public override void RefreshManager()
    {
        m_controller.RefreshController();
    }

    public override void ClearManager()
    {
        
    }
}
