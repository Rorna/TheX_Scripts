using System;

public class UIPause : UIObject, IPause
{
    public Action ContinueAction { get; private set; }
    public Action LoadAction { get; private set; }
    public Action TitleAction { get; private set; }

    public bool IsActive => gameObject.activeSelf;

    public void BindContinueCallback(Action action)
    {
        ContinueAction = action;
    }

    public void BindLoadCallback(Action action)
    {
        LoadAction = action;
    }

    public void BindTitleCallback(Action action)
    {
        TitleAction = action;
    }

    public void OnClickContinue()
    {
        ContinueAction?.Invoke();
    }

    public void OnClickLoad()
    {
        LoadAction?.Invoke();
    }

    public void OnClickTitle()
    {
        TitleAction?.Invoke();
    }

    public void ShowPause()
    {
        Show();
    }

    public void HidePause()
    {
        Hide();
    }
}