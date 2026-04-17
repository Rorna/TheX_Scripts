using UnityEngine;

public class NoticePopupController
{
    private INoticePopup m_notice;

    public void Init(INoticePopup noticeInterface)
    {
        m_notice = noticeInterface;
    }

    public void ShowNotice(string msg)
    {
        if (m_notice.IsActive)
            return;

        m_notice.Init(msg);
        m_notice.ShowNotice();
    }
}
