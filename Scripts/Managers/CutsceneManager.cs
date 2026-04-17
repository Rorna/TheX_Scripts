using System;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneManager : BaseManager
{
    public static CutsceneManager Instance { get; private set; }

    //シーンの中にあるカットシーンをIDで管理
    private Dictionary<string, CutsceneData> m_cutsceneDic;

    private GameObject m_cutsceneObj;

    private CutsceneController m_controller;
    public CutsceneStateEnum State => m_controller.State;

    public static event Action<CutsceneStateEnum> OnCutsceneStateChangeAction;

    public override void CreateManager()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public override void InternalInitManager()
    {
        m_controller = new CutsceneController();
        m_controller.Init();
        InitCutsceneDic();
    }

    public override void ExternalInitManager()
    {
    }

    public void InitCutsceneDic()
    {
        m_cutsceneDic = new Dictionary<string, CutsceneData>();

        //path
        string jsonPath = DefineString.JsonCutscenePath + DefineString.JsonCutsceneInfo;
        string jsonText = JsonUtil.LoadJson(jsonPath);

        var loadedData = JsonUtil.ParseJson<CutsceneInfo>(jsonText);

        m_cutsceneDic = loadedData.cutsceneInfoDic;
    }

    public void TryPlayCutscene(string cutsceneName)
    {
        if (m_cutsceneDic.TryGetValue(cutsceneName, out var cutsceneData) == false)
            return;

        //fadeIn -> play & pause Cutscene -> fadeOut -> play
        Facade.Instance.RequestScreenFadeIn(1f, () =>
        {
            m_controller.PlayCutscene(cutsceneData);
            m_controller.PauseCutscene(); //フェード効果のためのタイムライン一時停止

            Facade.Instance.RequestScreenFadeOut(1f, () =>
            {
                m_controller.ResumeCutscene();
            });
        });
    }

    public void TryHandleSignal(string key, string value)
    {
        m_controller.ReceiveSignal(key, value);
    }

    public void SetParent(GameObject obj)
    {
        obj.transform.SetParent(gameObject.transform);
    }

    public void HandleCutsceneStateChange(CutsceneStateEnum state)
    {
        OnCutsceneStateChangeAction?.Invoke(state);
    }

    public override void RefreshManager()
    {
        m_controller.RefreshController();
    }

    public override void ClearManager()
    {
        m_controller.Release();
    }
}