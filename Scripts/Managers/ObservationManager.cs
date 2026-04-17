using System;

///<summary>
///観察シーンでのみ動作する
///観察シーンで「観察」「審判」「フィールドに戻る」のような行為を管理する
///もちろん、実際の管理はコントローラがする
///</summary>

public class ObservationManager : BaseManager
{
    public static ObservationManager Instance { get; private set; }
    public bool Active { get; private set; }

    private ObservationController m_observationController;
    private JudgementController m_judgementController;

    public static event Action<bool> OnObservationAction;
    public static event Action OnJudgementAction;

    public string CurrentTargetName => m_observationController.CurrentTargetName;
    public bool ObservationMode => m_observationController.ObservationMode;

    public override void CreateManager()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }
    public override void InternalInitManager()
    {
        InitControllers();
        RefreshActiveState();
    }

    public override void ExternalInitManager()
    {
    }

    private void InitControllers()
    {
        m_observationController = new ObservationController();
        m_observationController.Init();

        m_judgementController = new JudgementController();
        m_judgementController.Init(this);
    }

    //観察、審判ロジック観察シーンじゃなかったら無効
    private void RefreshActiveState()
    {
        Active = GameSceneManager.Instance.CurrentSceneType == SceneTypeEnum.Observe;
        m_observationController.UpdateActivation(Active);
        m_judgementController.UpdateActivation(Active);
    }

    public void TryJudgement(string xName)
    {
        m_judgementController.RunJudgement(xName);
    }

    public void HandleObservationAction(bool active)
    {
        OnObservationAction?.Invoke(active);
    }

    public void HandleJudgementAction()
    {
        OnJudgementAction?.Invoke();
    }

    public override void RefreshManager()
    {
        RefreshActiveState();
    }

    public override void ClearManager()
    {
        m_observationController.Release();
        m_judgementController.Release();
    }
}
