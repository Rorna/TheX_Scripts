using System.Collections;
using TMPro;
using UnityEngine;

public class UINoticePopup : UIPopup, INoticePopup
{
    [SerializeField] private TMP_Text m_noticeText;
    [SerializeField] private float m_duration;

    public bool IsActive => gameObject.activeSelf;

    public void Init(string msg)
    {
        m_noticeText.text = msg;
    }

    public void ShowNotice()
    {
        Show();
    }

    public override void OnShow()
    {
        StartCoroutine(CoHide());
    }

    IEnumerator CoHide()
    {
        yield return new WaitForSeconds(m_duration);
        FadeOutHide();
    }

    public override void OnHide()
    {
        ClearPopup();
    }

    public void ClearPopup()
    {
        m_noticeText.text = "";
    }
}
