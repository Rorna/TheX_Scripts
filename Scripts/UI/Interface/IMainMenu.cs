using System;

public interface IMainMenu
{
    public Action NewGameAction { get; }
    public Action ContinueAction { get; }
    public Action HelpAction { get; }
    public Action ExitAction { get; }

    public void InitContinueButton(bool active);

    public void BindNewGameCallback(Action action);
    public void BindContinueCallback(Action action);
    public void BindHelpCallback(Action action);
    public void BindExitCallback(Action action);

    public void OnClickNewGame();
    public void OnClickContinue();
    public void OnClickHelp();
    public void OnClickExit();

    public void ShowMainMenu();
    public void HideMainMenu();
}
