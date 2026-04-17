using System;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///エフェクト管理。
///制御はコントローラがする
///</summary>
public class EffectManager : BaseManager
{
    public static EffectManager Instance { get; private set; }
    private EffectController m_controller;

    ///<summary>
    ///key: prefab.name
    ///</summary>
    private Dictionary<string, CustomObjectPool<ParticleSystem>> m_particlePoolDic;
    private Dictionary<string, CustomObjectPool<BulletTrail>> m_bulletTrailDic;
    
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
        m_controller = new EffectController();
        m_controller.Init(this);

    }

    public override void ExternalInitManager()
    {
        InitPoolDic();
    }

    private void InitPoolDic()
    {
        m_particlePoolDic = new Dictionary<string, CustomObjectPool<ParticleSystem>>();
        m_bulletTrailDic = new Dictionary<string, CustomObjectPool<BulletTrail>>();
    }

    public void TryPlayEffect<T>(T prefab, Vector3 createPos, Vector3 createDir,
        bool usePool, int poolCapacity = 20, Action<T> InitAction = null, Transform parent = null) where T : Component
    {
        if (prefab == null)
            return;

        if (prefab is ParticleSystem particlePrefab)
        {
            if (usePool)
            {
                //プールを所得したり生成する
                var pool = GetOrCreateParticlePool(particlePrefab, poolCapacity);
                m_controller.PlayParticleEffect(pool, createPos, createDir, InitAction as Action<ParticleSystem>, parent);
            }
            else
            {
                //プールを使わないなら即再生
                m_controller.PlayParticleEffectDirectly(particlePrefab, createPos, createDir, InitAction as Action<ParticleSystem>, parent);
            }
        }
    }

    private CustomObjectPool<ParticleSystem> GetOrCreateParticlePool(ParticleSystem prefab, int maxCapacity)
    {
        string key = prefab.name;

        if (m_particlePoolDic.TryGetValue(key, out var pool))
        {
            return pool;
        }

        int preCreateCount = Mathf.Max(1, maxCapacity);

        pool = new CustomObjectPool<ParticleSystem>();
        pool.Init(prefab, preCreateCount, transform);

        m_particlePoolDic.Add(key, pool);

        return pool;
    }

    public void TryPlayBulletTrail(BulletTrail prefab, Vector3 startPos, 
        Vector3 endPos, bool usePool, int poolCapacity = 20)
    {
        if (prefab == null)
            return;

        if (usePool)
        {
            var pool = GetOrCreateTrailPool(prefab, poolCapacity);
            m_controller.PlayBulletTrail(pool, startPos, endPos);
        }
        else
        {
            m_controller.PlayBulletTrailDirectly(prefab, startPos, endPos);
        }
    }

    private CustomObjectPool<BulletTrail> GetOrCreateTrailPool(BulletTrail prefab, int maxCapacity)
    {
        string key = prefab.name;
        if (m_bulletTrailDic.TryGetValue(key, out var pool))
        {
            return pool;
        }

        int preCreateCount = Mathf.Max(1, maxCapacity);
        pool = new CustomObjectPool<BulletTrail>();
        pool.Init(prefab, preCreateCount, transform);
        m_bulletTrailDic.Add(key, pool);

        return pool;
    }

    /// <summary>
    /// パーティクルの破棄処理（外部に対してプール管理を隠す）。
    /// 
    /// 呼び出し元はプールの有無を意識せずに、単に破棄を依頼できる。
    /// 内部で対象パーティクルに紐づくプールを確認し、
    /// プールが存在すれば返却し、無ければオブジェクト自体を破棄する。
    /// </summary>
    public void TryDestroyParticle(ParticleSystem particle)
    {
        string key = particle.name;

        CustomObjectPool<ParticleSystem> particlePool = null;
        if (m_particlePoolDic.TryGetValue(key, out var pool))
        {
            particlePool = pool;
        }

        m_controller.DestroyParticle(particle, particlePool);
    }

    public override void RefreshManager()
    {
        //pool clear
        foreach (var particlePool in m_particlePoolDic)
        {
            var pool = particlePool.Value;
            pool.ClearPool();
        }

        foreach (var trailPool in m_bulletTrailDic)
        {
            var pool = trailPool.Value;
            pool.ClearPool();
        }

        m_particlePoolDic.Clear();
        m_bulletTrailDic.Clear();

        m_controller.RefreshController();
    }

    public override void ClearManager()
    {
        
    }
}
