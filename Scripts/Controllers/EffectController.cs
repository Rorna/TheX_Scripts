using System;
using UnityEngine;
using System.Collections;
using Object = UnityEngine.Object;

public class EffectController
{
    private EffectManager m_manager;

    public void Init(EffectManager manager)
    {
        m_manager = manager;
    }

    ///<summary>
    ///MUZZLE FLASH のように動くものは親を設定する
    ///</summary>
    public void PlayParticleEffect(CustomObjectPool<ParticleSystem> pool, Vector3 pos,
        Vector3 dir, Action<ParticleSystem> initAction = null, Transform parent = null)
    {
        ParticleSystem particle = pool.Get();

        //ローカル座標で設定
        if (parent != null)
        {
            //親の位置 動いてもそれに合わせてプレイするため。
            particle.transform.SetParent(parent);

            //親の位置回転同期
            particle.transform.localPosition = Vector3.zero;
            particle.transform.localRotation = Quaternion.identity;

            //方向を上書きする
            if (dir != Vector3.zero)
                particle.transform.rotation = Quaternion.LookRotation(dir);
        }
        else
        {
            //ワールド座標で設定（被撃エフェクトみたいに）
            particle.transform.position = pos;
            if (dir != Vector3.zero)
                particle.transform.rotation = Quaternion.LookRotation(dir);
        }

        initAction?.Invoke(particle);

        particle.Play();
        m_manager.StartCoroutine(CoReturnParticle(pool, particle, particle.main.duration));
    }


    public void PlayParticleEffectDirectly(ParticleSystem prefab, Vector3 pos, Vector3 dir,
        Action<ParticleSystem> initAction = null,
        Transform parent = null)
    {
        ParticleSystem particle = Object.Instantiate(prefab, pos, Quaternion.LookRotation(dir), parent);
        if (parent != null)
        {
            particle.transform.localPosition = Vector3.zero;
        }

        initAction?.Invoke(particle);

        particle.Play();
        Object.Destroy(particle.gameObject, particle.main.duration);
    }

    private IEnumerator CoReturnParticle(CustomObjectPool<ParticleSystem> pool, ParticleSystem ps, float delay)
    {
        yield return new WaitForSeconds(delay);
        pool.Return(ps);
    }

    public void PlayBulletTrail(CustomObjectPool<BulletTrail> pool, Vector3 startPos, Vector3 endPos)
    {
        BulletTrail trail = pool.Get();

        //トレイル終了関数（目的地に到達）、POOLにリターン
        trail.SetupTrail(startPos, endPos, (completedTrail) =>
        {
            pool.Return(completedTrail);
        });
    }

    //弾丸軌跡の再生(プーリング未使用)
    public void PlayBulletTrailDirectly(BulletTrail prefab, Vector3 startPos, Vector3 endPos)
    {
        BulletTrail trail = Object.Instantiate(prefab);

        trail.SetupTrail(startPos, endPos, (completedTrail) =>
        {
            Object.Destroy(completedTrail.gameObject);
        });
    }

    ///<summary>
    ///poolがnull　ー＞　すぐ削除
    ///nullじゃなかったらPOOLにすぐ返却。
    ///</summary>
    public void DestroyParticle(ParticleSystem particle, CustomObjectPool<ParticleSystem> particlePool)
    {
        particle.Stop();

        if (particlePool == null)
        {
            Object.Destroy(particle.gameObject);
        }
        else //return to pool
        {
            particlePool.Return(particle);
        }
    }

    public void RefreshController()
    {
        m_manager.StopAllCoroutines();
    }
}