using System;

public interface IScreenEffect
{
    public bool IsActive { get; }

    public void PlayFadeIn(float duration, Action onComplete = null);
    public void PlayFadeOut(float duration, Action onComplete = null);
}
