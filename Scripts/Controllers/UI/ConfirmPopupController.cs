using System;
using UnityEngine;

public class ConfirmPopupController
{
    private IConfirmPopup m_popup;

    public void Init(IConfirmPopup popup)
    {
        m_popup = popup;
    }

    public void ShowPopup(string text, Action confirmAction)
    {
        m_popup.ClearPopup();
        m_popup.InitPopup(text, confirmAction);
        m_popup.ShowPopup();
    }
}
