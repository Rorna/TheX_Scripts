public interface INoticePopup
{
    public bool IsActive { get; }

    public void Init(string msg);
    public void ShowNotice();
}
