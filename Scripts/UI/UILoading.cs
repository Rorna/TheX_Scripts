public class UILoading : UIObject, ILoading
{
    public float FadeTime => m_fadeTime;

    public void ShowLoading()
    {
        FadeInShow();
    }

    public void HideLoading()
    {
        FadeOutHide();
    }
}