using System;

/// <summary>
/// 画面エフェクトを制御、管理するマネージャー
/// </summary>
public class ScreenEffectManager : BaseManager
{
    public static ScreenEffectManager Instance { get; private set; }

    private ScreenEffectController m_controller;

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
        InitScreenEffectController();
    }

    private void InitScreenEffectController()
    {
        m_controller = new ScreenEffectController();
        IScreenEffect effectInterface = UISystemManager.Instance.GetUIInterface<IScreenEffect>();
        m_controller.Init(effectInterface);
    }

    // fade in -> fade out
    public void TryFadeSequence(float duration = 0.5f)
    {
        m_controller.PlayFadeSequence(duration);
    }

    public void TryFadeIn(float duration = 0.5f, Action onComplete = null)
    {
        m_controller.PlayFadeIn(duration, onComplete);
    }

    public void TryFadeOut(float duration = 0.5f, Action onComplete = null)
    {
        m_controller.PlayFadeOut(duration, onComplete);
    }


    public override void RefreshManager()
    {
        
    }

    public override void ClearManager()
    {
        
    }
}
