using UnityEngine;

///<summary>
///一定時間（DelayTime）ごとに衝撃波パーティクルを一定回数だけ生成した後終了するスキル
///パーティクル数 = 100、パーティクルの Max Particle で調整
///</summary>
public class ShockWaveAttack : BaseSkill
{
    private EnemyController m_enemyController;

    //JSON
    private float m_delayTime;
    private int m_attackCount;
    private float m_scaleUpTime;
    private float m_scaleUpParam;
    private float m_lifeTime;
    private string m_particlePath;

    private float m_currentTimer;
    private int m_currentSpawnCount;

    public override void Init(GameObject caster, SkillData data)
    {
        base.Init(caster, data);

        m_enemyController = caster.GetComponent<EnemyController>();

        m_delayTime = data.DelayTime;
        m_attackCount = data.AttackCount;
        m_scaleUpTime = data.ScaleUpTime;
        m_scaleUpParam = data.ScaleUpParam;
        m_lifeTime = data.LifeTime;

        m_particlePath = data.ParticlePath;
    }

    public override void Execute()
    {
        Facade.Instance.RequestPlaySFX(DefineString.SFX_ShockWave, Vector3.zero, 0.3f, false);
        m_currentSpawnCount = 0;
        m_currentTimer = m_delayTime; //すぐ発射
    }

    public override void Update()
    {
        if (CurrentState != SkillStateEnum.Running) 
            return;

        m_currentTimer += Time.deltaTime;
        if (m_currentTimer >= m_delayTime)
        {
            m_currentTimer = 0f;
            SpawnShockWave();
            m_currentSpawnCount++;

            //全部発射したらスキル終了
            if (m_currentSpawnCount >= m_attackCount)
            {
                Complete();
            }
        }
    }

    private void SpawnShockWave()
    {
        m_enemyController.Anim.SetTrigger("ShockWaveAttack");

        GameObject effectPrefabObj = UnityUtil.LoadPrefab(m_particlePath);
        if (effectPrefabObj == null)
            return;

        Vector3 enemyPos = m_enemyController.Enemy.transform.position;
        enemyPos.y += 1.2f; //高さ調整
        ParticleSystem prefabParticle = effectPrefabObj.GetComponent<ParticleSystem>();

        //poolを使う
        Facade.Instance.RequestPlayEffect(prefabParticle, enemyPos, Vector3.zero, true, m_attackCount,
            (spawnedParticle) => //エフェクトマネージャからプールから取り出したものを設定する
            {
                var wave = spawnedParticle.gameObject.GetComponent<ShockwaveController>();
                if (wave != null)
                {
                    wave.Init(m_scaleUpTime, m_scaleUpParam, m_lifeTime, Damage);
                }
            });
    }

    public override void Cancel()
    {
        CurrentState = SkillStateEnum.Canceled;
    }

    public override void Complete()
    {
        CurrentState = SkillStateEnum.Completed;
        InvokeSkillComplete();
    }

    public override void Clear()
    {
        m_currentSpawnCount = 0;
        m_currentTimer = 0f;
    }
}