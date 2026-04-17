using UnityEngine;

public class DefaultGun : BaseWeapon
{
    /// <summary>
    /// 1. 発射判定（レイキャスト）
    /// 2. マズルフラッシュのパーティクル再生リクエスト
    /// 3. バレットトレイル（弾道軌跡）の生成リクエスト
    /// 4. 条件に応じて着弾点パーティクルの再生リクエスト
    /// Hitした位置に生成する。
    /// </summary>
    protected override void OnFire()
    {
        WeaponAnimator.SetTrigger("Fire");

        Vector3 origin = m_firstCamTr.position;
        Vector3 dir = m_firstCamTr.forward;
        RaycastHit hit;

        PlayMuzzleFlash();

        string soundID = AudioClip.name;
        Facade.Instance.RequestPlaySFX(soundID, origin, 0.8f, false);

        Vector3 hitPosition;
        if (Physics.Raycast(origin, dir, out hit, FireRange))
        {
            hitPosition = hit.point;
            IDamagable target = hit.collider.GetComponent<IDamagable>();
            if (target != null)
            {
                target.TakeDamage(Damage);
                Facade.Instance.RequestPlayEffect(HitEffect, hit.point, 
                    hit.normal, true, MaxAmmo);
            }
            else
            {
                Facade.Instance.RequestPlayEffect(DecalEffect, hitPosition, 
                    Vector3.zero, false, MaxAmmo);
            }
        }
        else
        {
            // 未ヒット時、最大射程距離の地点を終点として設定。
            hitPosition = origin + (dir * FireRange);
        }

        //トレイル生成リクエスト
        if (BulletTrailRenderer != null)
        {
            BulletTrail bulletTrail = BulletTrailRenderer.gameObject.GetComponent<BulletTrail>();
            Facade.Instance.RequestPlayTrailEffect(bulletTrail, ShootPoint.position,
                hitPosition, true);
        }
    }

    private void PlayMuzzleFlash()
    {
        Vector3 muzzlePos = ShootPoint.position;
        int poolCapacity = (MaxAmmo / 2); //半分に設定。
        Facade.Instance.RequestPlayEffect(MuzzleFlash, muzzlePos,
            ShootPoint.forward, true, poolCapacity, null, ShootPoint);
    }
}