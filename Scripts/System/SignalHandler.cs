using UnityEngine;

public class SignalHandler
{
    private CutsceneController m_controller;

    public void Init(CutsceneController controller)
    {
        m_controller = controller;
    }

    public void HandleSignal(string key, string value)
    {
        switch (key)
        {
            case "Scene":
                HandleSceneSignal(value);
                break;

            case "Effect":
                HandleEffectSignal(value);
                break;

            case "Dialog":
                HandleDialogSignal(value);
                break;
            case "Sound":
                HandleSoundSignal(value);
                break;
            default:
                break;
        }
    }

    private void HandleSceneSignal(string value)
    {
        if (value == DefineString.Start)
        {
            
        }
        else if (value == DefineString.End)
        {
            m_controller.PauseCutscene();

            //フェードイン、完了時にアクション ー＞　アンドカットシン　+　フェードアウト
            Facade.Instance.RequestScreenFadeIn(1f, () =>
            {
                m_controller.EndCutscene();

                Facade.Instance.RequestScreenFadeOut(1f);
            });
        }
    }

    private void HandleEffectSignal(string value)
    {
    }

    private void HandleSoundSignal(string value)
    {
        Facade.Instance.RequestPlaySFX(value, Vector3.zero, 0.3f, false);
    }

    private void HandleDialogSignal(string dialogID)
    {
        m_controller.PauseCutscene();
        Facade.Instance.RequestStartDialog(dialogID);
    }
}