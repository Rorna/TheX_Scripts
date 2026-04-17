using System;
using UnityEngine;

public interface IEnding
{
    public Action TitleAction { get; }
    public void BindTitleCallback(Action action);
    public void OnClickTitle();
    public void ShowEnding();
    public void HideEnding();
}
