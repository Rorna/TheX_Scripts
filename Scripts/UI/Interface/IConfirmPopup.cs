using System;
using UnityEngine;

public interface IConfirmPopup
{
    public Action ConfirmAction { get; }

    public void InitPopup(string text, Action action);

    public void OnClickConfirm();
    public void OnClickCancel();

    public void ShowPopup();
    public void ClearPopup();
}
