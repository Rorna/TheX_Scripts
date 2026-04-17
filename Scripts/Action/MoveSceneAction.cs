using UnityEngine;

public class MoveSceneAction : IAction
{
    public ActionTypeEnum ActionType { get; }
    private string m_sceneName;

    public void InitAction(ActionData actionData)
    {
        m_sceneName = actionData.TargetID;
    }

    public void OnExecute()
    {
        Facade.Instance.RequestMoveScene(m_sceneName);
    }
}