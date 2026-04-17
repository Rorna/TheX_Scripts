using UnityEngine;

public class EndingController
{
    public bool IsEnding { get; private set; }
    private IEnding m_ending;
    public void InitController(IEnding ending)
    {
        m_ending = ending;
        InitEndingElement();
    }

    public void ShowEnding()
    {
        Facade.Instance.RequestSwitchInputMap(InputTypeEnum.Ending);
        Time.timeScale = 0f;
        m_ending.ShowEnding();

        IsEnding = true;
    }

    private void InitEndingElement()
    {
        m_ending.BindTitleCallback(() =>
        {
            IsEnding = false;
            Time.timeScale = 1f;
            Facade.Instance.RequestMoveScene(DefineString.MainMenu);
        });
    }
}
