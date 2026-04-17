using System;
using UnityEngine;

public class MainMenuController
{
    private IMainMenu m_mainMenu;

    public void Init(IMainMenu mainMenu)
    {
        m_mainMenu = mainMenu;
        InitMainMenuElement();
    }

    private void InitMainMenuElement()
    {
        SaveData saveData = SaveManager.Instance.GameSaveData;
        if (saveData != null)
        {
            m_mainMenu.InitContinueButton(true);
            m_mainMenu.BindContinueCallback(ContinueAction);
        }
        else
        {
            m_mainMenu.InitContinueButton(false);
        }

        m_mainMenu.BindNewGameCallback(NewGameAction);
        m_mainMenu.BindHelpCallback(HelpAction);
        m_mainMenu.BindExitCallback(ExitAction);
    }

    private void NewGameAction()
    {
        SaveData saveData = SaveManager.Instance.GameSaveData;
        if (saveData != null)
        {
            //popup
            Facade.Instance.RequestShowConfirmPopup(NewGameConfirmText(), ConfirmNewGameAction);
            return;
        }

        Facade.Instance.RequestMoveScene(DefineString.StartScene);
        Facade.Instance.RequestPlaySFX(DefineString.SFX_UISound, Vector3.zero, 0.3f, false);
    }

    private string NewGameConfirmText()
    {
        switch (GameDirector.Instance.CurrentLanguageType)
        {
            case (LanguageTypeEnum.KOR):
                return DefineString.NewGameConfirmKor;
            case (LanguageTypeEnum.JPN):
                return DefineString.NewGameConfirmJpn;
        }

        return null;
    }

    private void ConfirmNewGameAction()
    {
        Facade.Instance.RequestDeleteSaveData();
        Facade.Instance.RequestMoveScene(DefineString.StartScene);
        Facade.Instance.RequestPlaySFX(DefineString.SFX_UISound, Vector3.zero, 0.3f, false);
    }

    private void ContinueAction()
    {
        Facade.Instance.RequestLoadGame();
        Facade.Instance.RequestPlaySFX(DefineString.SFX_UISound, Vector3.zero, 0.3f, false);
    }

    private void HelpAction()
    {
        Facade.Instance.RequestPlaySFX(DefineString.SFX_UISound, Vector3.zero, 0.3f, false);
    }

    private void ExitAction()
    {
        Application.Quit();
        Facade.Instance.RequestPlaySFX(DefineString.SFX_UISound, Vector3.zero, 0.3f, false);
    }


    public void RefreshController()
    {
        if(GameSceneManager.Instance.CurrentSceneType == SceneTypeEnum.MainMenu)
        {
            m_mainMenu.ShowMainMenu();

            SaveData saveData = SaveManager.Instance.GameSaveData;
            if (saveData != null)
            {
                m_mainMenu.InitContinueButton(true);
                m_mainMenu.BindContinueCallback(ContinueAction);
            }
            else
            {
                m_mainMenu.InitContinueButton(false);
            }
        }
        else
        {
            m_mainMenu.HideMainMenu();
        }
    }

    public void Release()
    {
    }
}
