public interface ICluePopup
{
    public bool IsActive { get; }
    public void ShowPopup(string spriteID, string title, string desc);
    public void HidePopup();
    public void OnClickExit();
}
