using System;
using UnityEngine;

public class UIGameOver : UIObject, IGameOver
{
    [SerializeField] private GameObject m_continueMenu;

    public Action ContinueAction { get; private set; }
    public Action TitleAction { get; private set; }
    public bool IsActive => gameObject.activeSelf;

    public void InitContinueButton(bool active)
    {
        m_continueMenu.SetActive(active);
    }

    public void BindContinueCallback(Action action)
    {
        ContinueAction = action;
    }

    public void BindTitleCallback(Action action)
    {
        TitleAction = action;
    }

    public void OnClickContinue()
    {
        ContinueAction?.Invoke();
    }

    public void OnClickTitle()
    {
        TitleAction?.Invoke();
    }

    public void ShowGameOver()
    {
        FadeInShow();
    }

    public void HideGameOver()
    {
        Hide();
    }
}
