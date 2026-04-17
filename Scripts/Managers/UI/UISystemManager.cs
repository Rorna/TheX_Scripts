using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

///<summary>
///全体 UI の構造を管理。
///UIクラスを外部からインタフェースで制御できるように管理
///UIObject 管理およびインタフェースをバインディングする
///バインドしたuiをコントローラに渡す
///</summary>
public partial class UISystemManager : BaseManager
{
    public static UISystemManager Instance { get; private set; }

    private Dictionary<Type, UIObject> m_uiObjectDic = new Dictionary<Type, UIObject>();

    ///<summary>
    ///key: interface
    ///value: UIObject
    ///</summary>
    private Dictionary<Type, Type> m_uiBindingDic = new Dictionary<Type, Type>();
    public RectTransform RootCanvas { get; private set; }

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
        InitRootCanvas();
        InitEventSystem();
        InitInterfaceBindings();

        CutsceneManager.OnCutsceneStateChangeAction += HandleCutsceneStateChangeAction;
    }

    public override void ExternalInitManager()
    {
    }

    private void InitRootCanvas()
    {
        if (RootCanvas != null)
            return;

        GameObject rootCanvas = new GameObject("Root");
        rootCanvas.transform.SetParent(transform);

        var canvasRect = rootCanvas.AddComponent<RectTransform>();
        RootCanvas = canvasRect;
    }

    private void InitEventSystem()
    {
        var eventSystem = FindAnyObjectByType<EventSystem>();
        if (eventSystem != null)
        {
            eventSystem.gameObject.transform.SetParent(gameObject.transform);
            return;
        }

        var go = new GameObject("EventSystem");
        go.AddComponent<EventSystem>();
        go.AddComponent<InputSystemUIInputModule>();

        go.transform.SetParent(gameObject.transform);
    }

    /// <summary>
    /// インターフェースとUIの紐づけを行う。
    /// コントローラーにインターフェースを提供し、クラス間の結合度を下げる（疎結合にする）ため。
    /// 各UIマネージャーは具体的な詳細を知らなくても、インターフェースのみで動作するようにする
    /// ところで、このインターフェースを実装しているクラスの取得はどこで？　ー＞　ここで。
    /// </summary>
    private void InitInterfaceBindings()
    {
        //以降、新しいUIクラスを追加するたびに更新
        //必要に応じて追加

        BindUI<IDialog, UIDialog>();
        BindUI<IHUD, UIHUD>();
        BindUI<ILoading, UILoading>();
        BindUI<IMainMenu, UIMainMenu>();
        BindUI<IPause, UIPause>();
        BindUI<IGameOver, UIGameOver>();
        BindUI<IScreenEffect, UIScreenEffect>();
        BindUI<IEnding, UIEnding>();
        
        //popup
        BindPopupUI<IConfirmPopup, UIConfirmPopup>();
        BindPopupUI<INoticePopup, UINoticePopup>();
        BindPopupUI<ICluePopup, UICluePopup>();
    }

    ///<summary>
    ///UI と Interface をディクショナリに保存。
    ///</summary>
    private void BindUI<TInterface, T>() where T : UIObject
    {
        Type interfaceType = typeof(TInterface);
        Type uiType = typeof(T);

        if (interfaceType.IsInterface == false)
            return;

        //インタフェースがそのuiクラスで実装されているか確認、なかったらfalse
        if (interfaceType.IsAssignableFrom(uiType) == false)
            return;

        m_uiBindingDic[interfaceType] = uiType;
    }

    //コントローラはこれでインターフェイスを取得する
    public TInterface GetUIInterface<TInterface>() where TInterface : class
    {
        Type interfaceType = typeof(TInterface);
        Type uiType = null;

        if (m_uiBindingDic.TryGetValue(interfaceType, out uiType) == false)
        {
            if (m_uiPopupBindingDic.TryGetValue(interfaceType, out uiType) == false)
            {
                return null;
            }
        }

        //クラスが UIObject を継承したか確認する
        UIObject uiObject = GetUIObject(uiType);
        if (uiObject == null)
            return null;

        //uiオブジェクトがインタフェースを継承しない場合、エラー
        if (uiObject is TInterface uiInterface)
            return uiInterface;

        return null;
    }
    
    public override void RefreshManager()
    {
        HideAll();
        HideAllPopup();
    }

    public override void ClearManager()
    {
        HideAll();
        CutsceneManager.OnCutsceneStateChangeAction -= HandleCutsceneStateChangeAction;
    }

    public void HideAll()
    {
        foreach (var uiObject in m_uiObjectDic.Values)
        {
            //ローディングUIは無視
            if (uiObject is UILoading)
                continue;

            if (uiObject.isActiveAndEnabled)
            {
                uiObject.Hide();
            }
        }

        HideAllPopup();
    }

    private UIObject GetUIObject(Type type)
    {
        if (m_uiObjectDic.ContainsKey(type) == false)
        {
            LoadUIObject(type);

            if (m_uiObjectDic.ContainsKey(type) == false)
            {
                return null;
            }
        }

        return m_uiObjectDic[type];
    }

    private void LoadUIObject(Type type)
    {
        if (m_uiObjectDic.ContainsKey(type))
            return;

        string path; 
        if (typeof(UIPopup).IsAssignableFrom(type))
            path = DefineString.UIPopUpPath + type.Name;
        else
        {
            path = DefineString.UIPath + type.Name;
        }

        var ui = UnityUtil.Instantiate(path, RootCanvas.transform);
        if (ui == null)
        {
            return;
        }

        var uiObject = ui.GetComponent<UIObject>();
        if (uiObject == null)
        {
            Destroy(ui);
            return;
        }

        ui.SetActive(false);
        AddUIObjectDic(uiObject);
    }

    private void AddUIObjectDic(UIObject uiObject)
    {
        if (m_uiObjectDic.ContainsKey(uiObject.GetType()))
            return;

        m_uiObjectDic.Add(uiObject.GetType(), uiObject);
    }


    private void HandleCutsceneStateChangeAction(CutsceneStateEnum state)
    {
        switch (state)
        {
            case CutsceneStateEnum.Play:
                HideAll();
                break;
        }
    }
}
