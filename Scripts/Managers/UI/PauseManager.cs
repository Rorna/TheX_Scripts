public class PauseManager : BaseManager
{
    public PauseManager Instance { get; private set; }
    private PauseController m_controller;

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
        m_controller = new PauseController();
        IPause uiInterface = UISystemManager.Instance.GetUIInterface<IPause>();
        m_controller.Init(uiInterface);
    }

    public override void RefreshManager()
    {
        
    }

    public override void ClearManager()
    {
        
    }
}
