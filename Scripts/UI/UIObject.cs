using DG.Tweening;
using UnityEngine;

///<summary>
///すべてのUIクラスはこの抽象クラスを継承しなければならない
///このクラスには UI 共通の関数が含まれている
///</summary>
public abstract class UIObject : MonoBehaviour
{
    [SerializeField] protected CanvasGroup m_canvasGroup;
    [SerializeField] protected float m_fadeTime;

    protected virtual void OnAwake()
    { }

    public virtual void OnShow()
    {
    }

    public virtual void OnHide()
    {
    }

    private void Awake()
    {
        OnAwake();
    }

    public bool IsVisible()
    {
        return gameObject.activeSelf;
    }

    public void Show()
    {
        if (IsVisible())
            return;

        gameObject.SetActive(true);
        OnShow();
    }

    public void Hide()
    {
        if (gameObject.activeSelf == false)
            return;

        gameObject.SetActive(false);
        OnHide();
    }

    public void FadeIn()
    {
        m_canvasGroup.alpha = 0f;
        m_canvasGroup.DOFade(1, m_fadeTime).SetUpdate(true);
    }

    public void FadeOut()
    {
        m_canvasGroup.alpha = 1f;
        m_canvasGroup.DOFade(0, m_fadeTime).SetUpdate(true);
    }

    public void FadeInShow()
    {
        m_canvasGroup.alpha = 0f;
        Show();

        m_canvasGroup.DOFade(1, m_fadeTime).SetUpdate(true);
    }

    public void FadeOutHide()
    {
        m_canvasGroup.alpha = 1f;
        m_canvasGroup.DOFade(0, m_fadeTime).SetUpdate(true).OnComplete(() =>
        {
            Hide();
            m_canvasGroup.alpha = 1f;
        });
    }
}