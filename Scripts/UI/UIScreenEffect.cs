using DG.Tweening;
using System;
using UnityEngine;

public class UIScreenEffect : UIObject, IScreenEffect
{
    public bool IsActive => gameObject.activeSelf;

    public void PlayFadeIn(float duration, Action onComplete = null)
    {
        Show();

        m_canvasGroup.DOKill(); // 進行中の効果停止
        m_canvasGroup.alpha = 0f;

        m_canvasGroup.DOFade(1f, duration).OnComplete(() => onComplete?.Invoke());
    }

    public void PlayFadeOut(float duration, Action onComplete = null)
    {
        m_canvasGroup.DOKill();
        m_canvasGroup.alpha = 1f;

        m_canvasGroup.DOFade(0f, duration).OnComplete(() =>
        {
            Hide();
            onComplete?.Invoke();
        });
    }
}
