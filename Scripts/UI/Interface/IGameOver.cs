using System;
using UnityEngine;

public interface IGameOver
{
    public Action ContinueAction { get; }
    public Action TitleAction { get; }
    public bool IsActive { get; }

    public void InitContinueButton(bool active);
    public void BindContinueCallback(Action action);
    public void BindTitleCallback(Action action);
    public void OnClickContinue();
    public void OnClickTitle();
    public void ShowGameOver();
    public void HideGameOver();
}
