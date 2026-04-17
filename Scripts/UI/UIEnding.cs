using System;

public class UIEnding : UIObject, IEnding
{
    public Action TitleAction { get; private set; }

    public void BindTitleCallback(Action action)
    {
        TitleAction = action;
    }

    public void ShowEnding()
    {
        FadeInShow();
    }

    public void HideEnding()
    {
        Hide();
    }

    public void OnClickTitle()
    {
        TitleAction?.Invoke();
    }
}