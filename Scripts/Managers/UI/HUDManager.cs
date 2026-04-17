public class HUDManager : BaseManager
{
    public static HUDManager Instance { get; private set; }
    private HUDController m_controller;

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
        m_controller = new HUDController();
        IHUD uiInterface = UISystemManager.Instance.GetUIInterface<IHUD>();
        m_controller.Init(uiInterface);
    }

    public override void RefreshManager()
    {
        m_controller.RefreshHUD();
    }


    public override void ClearManager()
    {
        m_controller.Release();
    }

    public void TryPlayDamageFeedback()
    {
        m_controller.PlayDamageFeedback();
    }
}
