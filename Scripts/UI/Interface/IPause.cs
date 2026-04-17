using System;
using UnityEngine;

public interface IPause
{
    public Action ContinueAction { get; }
    public Action LoadAction { get; }
    public Action TitleAction { get; }
    public bool IsActive { get; }

    public void BindContinueCallback(Action action);
    public void BindLoadCallback(Action action);
    public void BindTitleCallback(Action action);
    public void OnClickContinue();
    public void OnClickLoad();
    public void OnClickTitle();
    public void ShowPause();
    public void HidePause();
}
