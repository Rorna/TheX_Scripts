public class LoadingManager: BaseManager
{
    public static LoadingManager Instance { get; private set; }
    private LoadingController m_controller;
    public float FadeTime => m_controller.FadeTime;

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
        m_controller = new LoadingController();
        ILoading uiInterface = UISystemManager.Instance.GetUIInterface<ILoading>();
        m_controller.Init(uiInterface);
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
