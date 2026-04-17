using System.Collections.Generic;
using UnityEngine;


///<summary>
///プレーヤーコントローラ。 FSM、相互作用ロジックを持つ
///このため、Update処理用にMonoBehaviourを継承する
///</summary>
public partial class PlayerController : MonoBehaviour, IDamagable
{
    [SerializeField] 
    private GameObject m_groundCheckerObj;

    [SerializeField]
    private LayerMask m_groundLayer;

    [SerializeField]
    private float m_groundDistance = 0.4f;

    public GameObject Player { get; private set; }
    
    public float Speed { get; private set; }
    public float RunSpeed { get; private set; }

    public float JumpHeight { get; private set; }
    public float Gravity { get; private set; }
    public int CurrentHP { get; private set; }
    public int MaxHP { get; private set; }

    public int JudgementBullet { get; private set; }
    private int m_judgementBulletMaxCount;

    private float m_timer;
    private float m_interactInterval;

    public Vector2 Velocity;
    public Animator Animator { get; private set; }
    public CharacterController CharacterController { get; private set; }

    private PlayerMovementFSM m_movementFSM;
    private PlayerActionFSM m_actionFSM;

    private PlayerInfo m_playerInfo;

    public WeaponHandler WeaponHandler { get; private set; }

    public bool IsDead { get; private set; }

    private Vector3? m_cachedPos;
    private Quaternion? m_cachedRot;

    ///<summary>
    ///一度だけ起動するトリガーを保存
    ///この情報を参照して繰り返してトリガを発動させないようにする。
    ///保存されないデータ。
    ///</summary>
    public HashSet<string> TriggerInfoHash { get; private set; }

    private void Awake()
    {
        Animator = GetComponent<Animator>();
        CharacterController = GetComponent<CharacterController>();

        TriggerInfoHash = new HashSet<string>();
    }

    ///<summary>
    ///プレーヤーオブジェクトに付いているコントローラー
    ///プレーヤーオブジェクトが生成されたら、マネージャに逆にリクエスト
    ///</summary>
    private void Start()
    {
        Player = gameObject;

        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.RegisterController(this);
        }

        Facade.Instance.RequestSetPlayerCamera(gameObject);
    }

    public void Init()
    {
        InitFSM();
        UpdateCamInfo();
        SetPlayerInfo();
        InitWeaponHandler();

        BindEvent();
    }

    private void SetPlayerInfo()
    {
        string jsonPath = DefineString.JsonPlayerValuePath;
        string jsonText = JsonUtil.LoadJson(jsonPath);

        m_playerInfo = JsonUtil.ParseJson<PlayerInfo>(jsonText);
        Speed = m_playerInfo.Speed;
        RunSpeed = m_playerInfo.RunSpeed;
        JumpHeight = m_playerInfo.JumpHeight;
        Gravity = m_playerInfo.Gravity;
        MaxHP = m_playerInfo.HP;
        CurrentHP = MaxHP;

        IsDead = false;

        FirstPersonReach = m_playerInfo.FirstPersonReach;
        ThirdPersonRange = m_playerInfo.ThirdPersonRange;
        InteractableMask = LayerMask.GetMask(m_playerInfo.InteractionLayer);

        m_interactInterval = m_playerInfo.InteractInterval;
        JudgementBullet = m_playerInfo.JudgementBullet;
        m_judgementBulletMaxCount = JudgementBullet;
    }

    private void InitFSM()
    {
        m_movementFSM = new PlayerMovementFSM(this);
        m_actionFSM = new PlayerActionFSM(this);
    }

    private void InitWeaponHandler()
    {
        WeaponHandler = new WeaponHandler();
        WeaponHandler.Init(this);
    }

    private void BindEvent()
    {
        InputManager.OnJumpAction += HandleJump;
        InputManager.OnInteractAction += HandleInteract;
        InputManager.OnFireAction += HandleFire;
        InputManager.OnReloadAction += HandleReload;

        DialogManager.OnDialogStateChangedAction += HandleDialogStateChangedAction;
        CameraManager.OnChangeViewAction += UpdateCamInfo;
        GameSceneManager.OnCompleteLoadSceneAction += HandleCompleteLoadSceneAction;

        CombatManager.OnCombatActiveAction += HandleCombatActiveAction;

        ObjectManager.OnCreateObjectAction += HandleCreateAction;

        GameSceneManager.OnStartLoadSceneAction += HandleStartLoadSceneAction;
    }

    private void UnBindEvent()
    {
        InputManager.OnJumpAction -= HandleJump;
        InputManager.OnInteractAction -= HandleInteract;
        InputManager.OnFireAction -= HandleFire;
        InputManager.OnReloadAction -= HandleReload;

        DialogManager.OnDialogStateChangedAction -= HandleDialogStateChangedAction;
        CameraManager.OnChangeViewAction -= UpdateCamInfo;
        GameSceneManager.OnCompleteLoadSceneAction -= HandleCompleteLoadSceneAction;

        CombatManager.OnCombatActiveAction -= HandleCombatActiveAction;

        ObjectManager.OnCreateObjectAction -= HandleCreateAction;

        GameSceneManager.OnStartLoadSceneAction -= HandleStartLoadSceneAction;
    }

    private void Update()
    {
        m_movementFSM.Update();
        m_actionFSM.Update();

        //0.2秒単位で更新
        m_timer -= Time.deltaTime;
        if (m_timer <= 0f)
        {
            m_timer = m_interactInterval;
            CheckInteractable();
        }
    }

    private void HandleJump()
    {
        (m_movementFSM.CurrentState as IPlayerMovementState)?.HandleJumpInput();
    }

    public void UpdateGravity()
    {
        if (CharacterController.isGrounded && Velocity.y < 0)
        {
            Velocity.y = -2f; //地面にくっついているときに重力が積み重ねることを防ぐ
        }

        Velocity.y += Gravity * Time.deltaTime;
        CharacterController.Move(Velocity * Time.deltaTime);
    }

    private void HandleDialogStateChangedAction(bool isDialogActive)
    {
        (m_actionFSM.CurrentState as IPlayerActionState)?.HandleDialogInput(isDialogActive);
    }

    private void HandleFire()
    {
        (m_actionFSM.CurrentState as IPlayerActionState)?.HandleFireInput();
    }

    private void HandleReload()
    {
        (m_actionFSM.CurrentState as IPlayerActionState)?.HandleReloadInput();
    }

    public void SaveGame(SaveData saveData)
    {
        PlayerSaveData playerData = new PlayerSaveData();

        playerData.Pos = Player.transform.position;
        playerData.Angle = Player.transform.rotation.eulerAngles;
        playerData.JudgementBullet = JudgementBullet;
        playerData.MaxHP = MaxHP;
        playerData.TriggerInfoHash = TriggerInfoHash;

        saveData.PlayerData = playerData;
    }

    public void LoadCachedSaveData(SaveData saveData)
    {
        LoadGame(saveData);
    }

    public void LoadGame(SaveData saveData)
    {
        PlayerSaveData playerData = saveData.PlayerData;

        //pos 設定するためには文字コントローラをオフにする必要がある
        CharacterController = Player.GetComponent<CharacterController>();

        if (CharacterController != null)
            CharacterController.enabled = false;

        Player.transform.position = playerData.Pos;
        Player.transform.rotation = Quaternion.Euler(playerData.Angle);

        if (CharacterController != null)
            CharacterController.enabled = true;

        CurrentHP = MaxHP;

        //変更された位置に合わせてカメラを設定
        CameraManager.Instance.TrySetPlayerTarget(Player.gameObject);

        JudgementBullet = playerData.JudgementBullet;

        if (TriggerInfoHash == null)
            TriggerInfoHash = new HashSet<string>();

        TriggerInfoHash = playerData.TriggerInfoHash;
    }

    public void RefreshJudgementBullet(int count)
    {
        if (count > m_judgementBulletMaxCount)
            return;

        if (count < 0)
            return;

        JudgementBullet = count;
    }

    private void HandleCompleteLoadSceneAction()
    {
        if (GameSceneManager.Instance.CurrentSceneType == SceneTypeEnum.MainMenu)
            gameObject.SetActive(false);
        else
        {
            gameObject.SetActive(true);
        }

        m_movementFSM.RefreshFSM();
        m_actionFSM.RefreshFSM();
        WeaponHandler.RefreshWeapon();

        if (CharacterController != null)
            CharacterController.enabled = true;
    }

    public void RefreshController()
    {
        CurrentTarget = null;
        PlayerManager.Instance.HandleTargetChange(null);

        if (GameSceneManager.Instance.CurrentSceneType == SceneTypeEnum.MainMenu)
        {
            DeleteTriggerInfo();

            m_cachedPos = null;
            m_cachedRot = null;
        }
    }


    public void Release()
    {
        UnBindEvent();
    }

    public bool CheckIsGrounded()
    {
        if (m_groundCheckerObj == null) 
            return false;

        return Physics.CheckSphere(m_groundCheckerObj.transform.position, m_groundDistance, m_groundLayer);
    }

    public void TakeDamage(int damage)
    {
        Facade.Instance.RequestPlaySFX(DefineString.SFX_ScifiHurt, Vector3.zero, 0.3f, false);
        
        CurrentHP -= damage;

        Facade.Instance.RequestPlayDamageFeedback();

        PlayerManager.Instance.HandleDamage(damage);

        if (CurrentHP <= 0)
        {
            IsDead = true;
            PlayerManager.Instance.HandleDeath();
            return;
        }
    }

    private void HandleCombatActiveAction(bool active)
    {
        WeaponHandler.RefreshWeapon(active);
    }

    public void SaveTriggerInfo(string triggerName)
    {
        if (TriggerInfoHash == null)
            TriggerInfoHash = new HashSet<string>();

        if (TriggerInfoHash.Contains(triggerName))
            return;

        TriggerInfoHash.Add(triggerName);
    }

    private void HandleStartLoadSceneAction()
    {
        //現在のシーンがフィールドの場合、位置、角度を保存
        if (GameSceneManager.Instance.CurrentSceneType != SceneTypeEnum.Field)
            return;

        m_cachedPos = gameObject.transform.position;
        m_cachedRot = gameObject.transform.rotation;

        if(CharacterController != null)
            CharacterController.enabled = false;
    }

    private void HandleCreateAction(GameObject obj)
    {
        if (obj != gameObject)
            return;

        if (m_cachedPos.HasValue == false&& m_cachedRot.HasValue == false)
            return;

        if (GameSceneManager.Instance.CurrentSceneType != SceneTypeEnum.Field)
            return;

        gameObject.transform.position = m_cachedPos.Value;
        gameObject.transform.rotation = m_cachedRot.Value;

        //キャッシュされた位置、角度データがあれば設定して削除
        m_cachedPos = null;
        m_cachedRot = null;
    }

    public void DeleteTriggerInfo()
    {
        if (TriggerInfoHash.Count == 0)
            return;

        TriggerInfoHash.Clear();
    }
}