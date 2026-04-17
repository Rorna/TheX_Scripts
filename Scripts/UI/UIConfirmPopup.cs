using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIConfirmPopup : UIPopup, IConfirmPopup
{
    [SerializeField] private TextMeshProUGUI m_descText;
    [SerializeField] private Button m_confirmButton;
    [SerializeField] private Button m_cancelButton;

    public Action ConfirmAction { get; private set; }

    protected override void OnAwake()
    {
        base.OnAwake();

        m_confirmButton.onClick.AddListener(OnClickConfirm);
        m_cancelButton.onClick.AddListener(OnClickCancel);
    }

    public void InitPopup(string text, Action action)
    {
        Init();

        m_descText.text = text;
        ConfirmAction = action;
    }

    public void OnClickConfirm()
    {
        ConfirmAction?.Invoke();
    }

    public void OnClickCancel()
    {
        Hide();
    }

    public void ShowPopup()
    {
        Show();
    }

    public override void OnHide()
    {
        ClearPopup();
    }

    public void ClearPopup()
    {
        m_descText.text = "";
        ConfirmAction = null;
    }
}
