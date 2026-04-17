using System;

///<summary>
///あちこち汎用的に使われるポップアップUI管理マネージャ
///Confirm popupとか...
///</summary>
public class CommonPopupManager : BaseManager
{
    public static CommonPopupManager Instance { get; private set; }

    private ConfirmPopupController m_confirmController;
    private NoticePopupController m_noticeController;
    public override void CreateManager()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public override void InternalInitManager()
    {
        
    }

    public override void ExternalInitManager()
    {
        
    }

    private void InitConfirmController()
    {
        m_confirmController = new ConfirmPopupController();
        IConfirmPopup popupInterface = UISystemManager.Instance.GetUIInterface<IConfirmPopup>();
        m_confirmController.Init(popupInterface);
    }

    private void InitNoticeController()
    {
        m_noticeController = new NoticePopupController();
        INoticePopup popupInterface = UISystemManager.Instance.GetUIInterface<INoticePopup>();
        m_noticeController.Init(popupInterface);
    }

    public void TryShowConfirmPopup(string text, Action confirmAction)
    {
        if(m_confirmController == null)
            InitConfirmController();

        m_confirmController.ShowPopup(text, confirmAction);
    }

    public void TryNoticePopup(string msg)
    {
        if(m_noticeController == null)
            InitNoticeController();

        m_noticeController.ShowNotice(msg);
    }

    public override void RefreshManager()
    {
        
    }

    public override void ClearManager()
    {
        
    }
}
