using UnityEngine;

public class PlayCutSceneAction : IAction
{
    public ActionTypeEnum ActionType { get; private set; }
    private string m_cutsceneName;
    public void InitAction(ActionData actionData)
    {
        m_cutsceneName = actionData.TargetID;
        ActionType = actionData.ActionType;
    }

    public void OnExecute()
    {
        Facade.Instance.RequestPlayCutscene(m_cutsceneName);
    }
}
