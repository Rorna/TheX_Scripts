using UnityEngine;

public class CutsceneController
{
    private TimelineHandler m_timlineHandler;
    private SignalHandler m_signalHandler;
    public GameObject CutsceneObject { get; private set; }
    public CutsceneStateEnum State { get; private set; }

    public void Init()
    {
        m_timlineHandler = new TimelineHandler();
        m_signalHandler = new SignalHandler();
        m_signalHandler.Init(this);
        State = CutsceneStateEnum.NonPlay;

        BoundEvent();
    }

    private void BoundEvent()
    {
        DialogManager.OnDialogStateChangedAction += HandleDialogStateChangeAction;
    }

    private void UnBoundEvent()
    {
        DialogManager.OnDialogStateChangedAction -= HandleDialogStateChangeAction;
    }

    public void ReceiveSignal(string key, string value)
    {
        m_signalHandler.HandleSignal(key, value);
    }

    public void PlayCutscene(CutsceneData cutsceneData)
    {
        CreateCutsceneObject(cutsceneData.Path);
        if (CutsceneObject == null)
            return;

        m_timlineHandler.Init(CutsceneObject);
        m_timlineHandler.SetDynamicBindings(cutsceneData.TrackDynamicBindingDic);
        m_timlineHandler.Play();

        RefreshCutsceneState(CutsceneStateEnum.Play);
    }

    private void RefreshCutsceneState(CutsceneStateEnum state)
    {
        State = state;
        CutsceneManager.Instance.HandleCutsceneStateChange(state);

        //終了を知らせたあとノンプレイに変更
        if (state == CutsceneStateEnum.End)
            State = CutsceneStateEnum.NonPlay;
    }

    private void CreateCutsceneObject(string path)
    {
        GameObject cutsceneObj = UnityUtil.Instantiate(path);
        if (cutsceneObj == null)
        {
            return;
        }

        CutsceneObject = cutsceneObj;
        CutsceneManager.Instance.SetParent(cutsceneObj);
    }

    public void PauseCutscene()
    {
        m_timlineHandler.Pause();
        RefreshCutsceneState(CutsceneStateEnum.Pause);
    }

    public void ResumeCutscene()
    {
        m_timlineHandler.Resume();
        RefreshCutsceneState(CutsceneStateEnum.Resume);
    }

    public void EndCutscene()
    {
        m_timlineHandler.RestoreBindings();
        m_timlineHandler.ClearHandler();

        if (CutsceneObject != null)
        {
            GameObject.Destroy(CutsceneObject);
            CutsceneObject = null;
        }

        RefreshCutsceneState(CutsceneStateEnum.End);
    }

    private void HandleDialogStateChangeAction(bool active)
    {
        //タイムラインが一時停止　+　会話終了 ->カットシーンを再開
        if (State == CutsceneStateEnum.Pause && active == false)
        {
            ResumeCutscene();
        }
    }

    public void RefreshController()
    {
        State = CutsceneStateEnum.NonPlay;
    }

    public void Release()
    {
        UnBoundEvent();
    }
}