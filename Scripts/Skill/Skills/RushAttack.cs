using System;
using UnityEngine;

public class RushAttack : BaseSkill
{
    private EnemyController m_enemyController;
    private bool m_isRush;
    private Rigidbody m_casterRigid;

    private float m_rushForce;
    private float m_delayTime;
    private float m_rushDuration;

    public override void Init(GameObject caster, SkillData data)
    {
        base.Init(caster, data);

        m_enemyController = caster.GetComponent<EnemyController>();
        if (m_enemyController == null)
            return;

        m_enemyController.OnCollisionAction -= HandleCollision;
        m_enemyController.OnCollisionAction += HandleCollision;

        m_delayTime = data.DelayTime;
        m_rushDuration = data.RushDuration;
        m_rushForce = data.RushForce;

        m_isRush = false;

        m_casterRigid = m_caster.GetComponent<Rigidbody>();
    }

    public override void Execute()
    {
        Facade.Instance.RequestPlaySFX(DefineString.SFX_RushAttack, Vector3.zero, 0.8f, false);
        m_enemyController.Anim.SetTrigger("RushAttack");
    }

    public override void Update()
    {
        if (m_isRush == false)
        {
            //1. 待ち時間処理
            m_delayTime -= Time.deltaTime;
            if (m_delayTime <= 0)
            {
                m_isRush = true;
                Rush();
            }
        }
        else //突進中
        {
            m_rushDuration -= Time.deltaTime;
            if (m_rushDuration <= 0)
            {
                Complete();
            }
        }
    }

    private void Rush()
    {
        //方向ベクトル　＝　プ​​レーヤーに向かうから（プレーヤー座標 -エネルギー座標）。normalized
        Vector3 playerPos = PlayerManager.Instance.Player.gameObject.transform.position;
        playerPos.y = 0f;

        Vector3 enemyPos = m_enemyController.Enemy.transform.position;
        enemyPos.y = 0f;

        //方向ベクトル
        Vector3 dirVector = (playerPos - enemyPos).normalized;

        //方向に向けて突進
        if (m_casterRigid == null)
            return;

        m_casterRigid.AddForce(dirVector * m_rushForce, ForceMode.Impulse);
    }

    ///<summary>
    ///ぶつかるとスキル終了
    ///</summary>
    private void HandleCollision(GameObject obj)
    {
        //プレイヤーならダメージを与え、あるいは単にコンプリート、スキル終了処理
        if (CurrentState != SkillStateEnum.Running) 
            return;

        if (obj.layer == LayerMask.NameToLayer("Ground"))
            return;

        var player = PlayerManager.Instance.Player;
        if (obj == player)
        {
            IDamagable damagable = obj.GetComponent<IDamagable>();
            if (damagable != null)
            {
                damagable.TakeDamage(Damage);
            }

            Facade.Instance.RequestCameraShake();
        }

        Complete();
    }


    public override void Cancel()
    {
        Complete();
    }

    public override void Complete()
    {
        //重複して終了防止
        if (CurrentState != SkillStateEnum.Running)
            return;

        //即時停止
        if (m_casterRigid != null)
        {
            m_casterRigid.linearVelocity = Vector3.zero;
            m_casterRigid.angularVelocity = Vector3.zero;
        }

        CurrentState = SkillStateEnum.Completed;
        m_enemyController.OnCollisionAction -= HandleCollision;

        InvokeSkillComplete();
    }

    public override void Clear()
    {
        m_delayTime = 0;
        m_rushDuration = 0; 

        m_enemyController = null;
        m_isRush = false;
        m_casterRigid = null;
}
}
