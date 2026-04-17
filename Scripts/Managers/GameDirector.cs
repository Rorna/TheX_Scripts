using UnityEngine;

/// <summary>
/// ゲーム全体の進行を管理するクラス
/// 
/// 【主な役割】
/// 1. ゲームの初期化（各種マネージャーのセットアップ）
/// 2. ゲームデータの管理
/// 3. ゲーム進行（フロー）の制御
/// 
/// マネージャーの準備からゲームのセットアップ、そして全体の進行管理までを統括する。
/// 注意：最初のシーンにゲームオブジェクトとして配置しておく必要がある。
/// </summary>
public class GameDirector : MonoBehaviour
{
    public static GameDirector Instance { get; private set; }
    private ManagerController m_controller;

    public LanguageTypeEnum CurrentLanguageType => LanguageTypeEnum.JPN;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);

//実際のビルド環境ではログ出力無効
#if !UNITY_EDITOR
    Debug.unityLogger.logEnabled = false;
#endif
    }

    private void Start()
    {
        InitFacade();
        SetUpManagers();
        m_controller.RefreshManagers(); //設定後、現在のシーンに合わせてリフレッシュ
    }

    private void InitFacade()
    {
        string facadeObjName = "@" + "Facade";

        GameObject go = new GameObject(facadeObjName);
        go.transform.SetParent(gameObject.transform);

        var facade = go.AddComponent<Facade>();
        facade.Init();
    }

    private void SetUpManagers()
    {
        GameObject masterManagerObj = new GameObject("@MasterManager");
        m_controller = masterManagerObj.AddComponent<ManagerController>();
        DontDestroyOnLoad(m_controller);
        m_controller.SetManagerControllers();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}