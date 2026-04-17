public class ActionController
{
    private IAction m_action;
    private IAction m_cachedAction;

    public void Init()
    {
        BindEvent();
    }

    private void BindEvent()
    {
        CutsceneManager.OnCutsceneStateChangeAction += HandleCutsceneStateChange;
    }

    private void UnBindEvent()
    {
        CutsceneManager.OnCutsceneStateChangeAction -= HandleCutsceneStateChange;
    }

    private void HandleCutsceneStateChange(CutsceneStateEnum state)
    {
        switch (state)
        {
            case CutsceneStateEnum.End:
                m_cachedAction?.OnExecute();
                    break;
        }
    }

    public void ExecuteAction(ActionData actionData)
    {
        m_action = CreateAction(actionData);

        //カットシーンが実行中？ -> アクションキャッシュ
        if (CutsceneManager.Instance.State != CutsceneStateEnum.NonPlay)
        {
            m_cachedAction = m_action;
            return;
        }

        m_action?.OnExecute();
    }

    private IAction CreateAction(ActionData actionData)
    {
        IAction action = null;

        switch (actionData.ActionType)
        {
            case ActionTypeEnum.GetClue:
                action = new GetClueAction();
                break;
            case ActionTypeEnum.GetQuest:
                action = new GetQuestAction();
                break;
            case ActionTypeEnum.MoveScene:
                action = new MoveSceneAction();
                break;
            case ActionTypeEnum.RunDialog:
                action = new RunDialogAction();
                break;
            case ActionTypeEnum.PlayCutScene:
                action = new PlayCutSceneAction();
                break;
            case ActionTypeEnum.RunJudgement:
                action = new RunJudgementAction();
                break;
            case ActionTypeEnum.RunSave:
                action = new RunSaveAction();
                break;
            case ActionTypeEnum.RunLoad:
                action = new RunLoadAction();
                break;
            case ActionTypeEnum.GameOver:
                action = new GameOverAction();
                break;
            case ActionTypeEnum.ShowNotify:
                action = new ShowNotifyAction();
                break;
            case ActionTypeEnum.RunEnding:
                action = new RunEndingAction();
                break;
        }

        action?.InitAction(actionData);
        return action;
    }

    public void RefreshController()
    {
        m_action = null;
        m_cachedAction = null;
    }

    public void Release()
    {
        m_action = null;
        UnBindEvent();
    }
}