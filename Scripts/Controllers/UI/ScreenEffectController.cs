using System;

public class ScreenEffectController
{
    private IScreenEffect m_effect;

    public void Init(IScreenEffect effect)
    {
        m_effect = effect;
    }

    //フェードアウトアクションを受け取る
    public void PlayFadeSequence(float duration, Action onAllComplete = null)
    {
        m_effect.PlayFadeIn(duration, () =>
        {
            m_effect.PlayFadeOut(duration, () =>
            {
                onAllComplete?.Invoke();
            });
        });
    }

    public void PlayFadeIn(float duration, Action onComplete = null)
    {
        m_effect.PlayFadeIn(duration, onComplete);
    }

    public void PlayFadeOut(float duration, Action onComplete = null)
    {
        m_effect.PlayFadeOut(duration, onComplete);
    }

}
