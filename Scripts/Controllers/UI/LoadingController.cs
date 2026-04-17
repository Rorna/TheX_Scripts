public class LoadingController
{
    private ILoading m_loadingUI;
    public float FadeTime { get; private set; }

    public void Init(ILoading loading)
    {
        m_loadingUI = loading;
        FadeTime = m_loadingUI.FadeTime;

        BindEvent();
    }

    private void BindEvent()
    {
        GameSceneManager.OnStartLoadSceneAction += ShowLoading;
        //シーンロードが完全に終わったときフェードアウト
        GameSceneManager.OnCompleteLoadSceneAction += HideLoading;
    }

    private void UnBindEvent()
    {
        GameSceneManager.OnStartLoadSceneAction -= ShowLoading;
        GameSceneManager.OnCompleteLoadSceneAction -= HideLoading;
    }

    public void RefreshController()
    {
    }

    private void ShowLoading()
    {
        m_loadingUI.ShowLoading();
    }

    private void HideLoading()
    {
        m_loadingUI.HideLoading();
    }

    public void Release()
    {
        UnBindEvent();
    }
}
