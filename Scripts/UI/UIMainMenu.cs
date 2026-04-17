using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMainMenu : UIObject, IMainMenu
{
    [SerializeField] private GameObject m_continueMenu;

    public Action NewGameAction { get; private set; }
    public Action ContinueAction { get; private set; }
    public Action HelpAction { get; private set; }
    public Action ExitAction { get; private set; }

    public override void OnShow()
    {

    }

    public override void OnHide()
    {
    }

    #region Bind

    public void InitContinueButton(bool active)
    {
        m_continueMenu.SetActive(active);
    }

    public void BindNewGameCallback(Action action)
    {
        NewGameAction = action;
    }

    public void BindContinueCallback(Action action)
    {
        ContinueAction = action;
    }

    public void BindHelpCallback(Action action)
    {
        HelpAction = action;
    }

    public void BindExitCallback(Action action)
    {
        ExitAction = action;
    }
    #endregion


    public void OnClickNewGame()
    {
        NewGameAction?.Invoke();
    }

    public void OnClickContinue()
    {
        ContinueAction?.Invoke();
    }

    public void OnClickHelp()
    {
        HelpAction?.Invoke();
    }

    public void OnClickExit()
    {
        ExitAction?.Invoke();
    }

    public void ShowMainMenu()
    {
        Show();
    }

    public void HideMainMenu()
    {
        Hide();
    }
}
