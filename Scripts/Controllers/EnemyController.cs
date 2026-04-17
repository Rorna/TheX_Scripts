using System;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///ブレインを制御する
///</summary>
public class EnemyController : MonoBehaviour, IDamagable
{
    [Header("Ground Check")]
    [SerializeField] private Transform m_groundCheck; //グラウンドチェックオブジェクト
    [SerializeField] private float m_groundDistance = 0.2f; //床判定半径
    [SerializeField] private LayerMask m_groundMask;

    public GameObject Enemy { get; private set; }
    public GameObject Player { get; private set; }
    public Animator Anim { get; private set; }
    public Rigidbody RigidBody { get; private set; }

    public int CurrentHP { get; private set; }
    public int MaxHP { get; private set; }
    public int Attack { get; private set; }
    public float Speed { get; private set; }
    public float JumpHeight { get; private set; }
    public float Gravity { get; private set; }

    public float MaxActionInterval { get; private set; }
    public float MinActionInterval { get; private set; }

    public List<EnemyActionEnum> ActionList { get; private set; }
    public List<SkillEnum> SkillList { get; private set; }

    //move values
    public float BaseMoveDistance { get; private set; }
    public float MaxMoveDistance { get; private set; }
    public float MinMoveDistance { get; private set; }
    public float MaxMoveAngle { get; private set; }
    public float MinMoveAngle { get; private set; }
    public float MaxMoveDuration { get; private set; }
    public float MinMoveDuration { get; private set; }

    private EnemyManager m_manager;
    private EnemyBrain m_brain;

    public event Action<GameObject> OnCollisionAction; //衝突アクションイベント

    public void InitEnemyController(EnemyManager manager, EnemyController controller, EnemyData enemyData)
    {
        m_manager = manager;
        Enemy = gameObject;

        SetEnemyInfo(enemyData);

        m_brain = new EnemyBrain();
        m_brain.InitBrain(this);

        Player = PlayerManager.Instance.Player;

        m_manager.HandleSpawnEnemy(this);
    }

    public void Awake()
    {
        Anim = GetComponent<Animator>();
        RigidBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        m_brain.BrainUpdate();
    }

    private void FixedUpdate()
    {
        m_brain.BrainFixedUpdate();
        LookAtPlayer();
    }

    private void SetEnemyInfo(EnemyData enemyData)
    {
        CurrentHP = enemyData.HP;
        MaxHP = CurrentHP;
        Attack = enemyData.Attack;
        Speed = enemyData.Speed;
        JumpHeight = enemyData.JumpHeight;
        Gravity = enemyData.Gravity;
        MinActionInterval = enemyData.MinActionInterval;
        MaxActionInterval = enemyData.MaxActionInterval;
        ActionList = enemyData.ActionList;
        SkillList = enemyData.SkillList;

        BaseMoveDistance = enemyData.BaseMoveDistance;
        MaxMoveDistance = enemyData.MaxMoveDistance;
        MinMoveDistance = enemyData.MinMoveDistance;
        MaxMoveAngle = enemyData.MaxMoveAngle;
        MinMoveAngle = enemyData.MinMoveAngle;
        MaxMoveDuration = enemyData.MaxMoveDuration;
        MinMoveDuration = enemyData.MinMoveDuration;
    }

    public void TakeDamage(int damage)
    {
        CurrentHP -= damage;
        m_manager.DamageEnemy(this, damage);

        if (CurrentHP <= 0)
        {
            Facade.Instance.RequestPlaySFX(DefineString.SFX_EnemyDead, gameObject.transform.position, 0.7f, false);
            m_manager.DeadEnemy(this);
            m_manager.HandleDeadEnemy(this);
        }
    }

    //move = dirVector *speed *deltaTime
    public void MoveEnemy(Vector3 direction)
    {
        Vector3 moveVelocity = direction * Speed;
        moveVelocity.y = RigidBody.linearVelocity.y;
        RigidBody.linearVelocity = moveVelocity;
    }

    public void StopMovement()
    {
        RigidBody.linearVelocity = new Vector3(0f, RigidBody.linearVelocity.y, 0f);
        RigidBody.angularVelocity = Vector3.zero; //角速度も0
    }

    public void PerformJump()
    {
        RigidBody.AddForce(Vector3.up * JumpHeight, ForceMode.Impulse);
    }

    //床に触れたかチェック
    public bool IsGrounded()
    {
        bool isGround = Physics.CheckSphere(m_groundCheck.position, m_groundDistance, m_groundMask);
        return isGround;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (m_groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(m_groundCheck.position, m_groundDistance);
        }
    }
#endif

    private void OnCollisionEnter(Collision collision)
    {
        //wall や player の場合、移動中なら停止する。
        if (collision.gameObject.CompareTag("Wall") ||
            collision.gameObject.CompareTag("Player"))
        {
            if (m_brain.FSM.CurrentStateEnum == EnemyStateEnum.Move)
            {
                StopMovement();
                m_brain.ExecuteAction(EnemyActionEnum.Idle);
            }
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        //ぶつかったターゲットに対してイベントを実行する
        GameObject obj = collider.gameObject;
        OnCollisionAction?.Invoke(obj);

        //ぶつかった対象がプレイヤーならダメージを与える
        if (obj == Player)
        {
            IDamagable damage = obj.GetComponent<IDamagable>();
            damage.TakeDamage(Attack);
        }
    }

    private void LookAtPlayer()
    {
        //敵からプレイヤーに向かう方向ベクトルを出す
        Vector3 direction = Player.transform.position - transform.position;

        //プレイヤージャンプの時ENEMYの上下の傾きを防止
        direction.y = 0f;

        //方向ベクトルがゼロでない場合にのみ回転（ゼロベクトル時のエラー防止）
        if (direction.sqrMagnitude > 0.001f)
        {
            //その方向を見る回転値の計算
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            //Slerp を使って現在の回転から目標回転にスムーズに補間(10f は回転速度)
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }
}