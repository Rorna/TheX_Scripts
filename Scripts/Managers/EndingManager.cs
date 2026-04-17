/// <summary>
/// Ending UI 管理
/// </summary>
public class EndingManager : BaseManager
{
    public static EndingManager Instance { get; private set; }
    private EndingController m_controller;
    public bool IsEnding => m_controller.IsEnding;

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
        m_controller = new EndingController();
        IEnding endingInterface = UISystemManager.Instance.GetUIInterface<IEnding>();
        m_controller.InitController(endingInterface);
    }

    public override void ExternalInitManager()
    {
    }

    public override void RefreshManager()
    {
    }

    public void TryEnding()
    {
        m_controller.ShowEnding();
    }

    public override void ClearManager()
    {
    }
}
