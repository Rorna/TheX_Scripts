using System.Collections;
using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour, IWeapon
{
    [field: Header("Weapon Stats")]
    [field: SerializeField] public int Damage { get; private set; }
    [field: SerializeField] public int MaxAmmo { get; private set; }
    [field: SerializeField] public float ReloadTime { get; private set; }
    [field: SerializeField] public float FireRate { get; private set; }
    [field: SerializeField] public float FireRange { get; private set; } //最大有効距離

    [field: Header("Animator")]
    [field: SerializeField] public Animator WeaponAnimator { get; private set; }

    [field: Header("Effects")]
    [field: SerializeField] public ParticleSystem HitEffect { get; private set; }
    [field: SerializeField] public ParticleSystem DecalEffect { get; private set; }
    [field: SerializeField] public ParticleSystem MuzzleFlash { get; private set; }
    [field: SerializeField] public TrailRenderer BulletTrailRenderer { get; private set; }
    [field: SerializeField] public Transform ShootPoint { get; private set; } //effect 再生位置用

    [field: Header("AudioClip")]
    [field: SerializeField] public AudioClip AudioClip { get; private set; }

    public int CurrentAmmo { get; protected set; }

    public bool IsReloading => m_isReloading;

    //クールタイム計算のための内部変数
    protected float m_lastFireTime = -9999f;
    protected bool m_isReloading = false;

    protected Transform m_firstCamTr;

    private void Awake()
    {
        CurrentAmmo = MaxAmmo;
    }

    private void Start()
    {
        var firstCam = CameraManager.Instance.FirstCamera;
        m_firstCamTr = firstCam ? firstCam.transform : null;
    }

    public virtual void Fire()
    {
        if (m_isReloading)
        {
            return;
        }

        //クールタイムチェック
        if (Time.time < m_lastFireTime + FireRate)
            return;

        if (CurrentAmmo <= 0)
        {
            return;
        }

        CurrentAmmo--;
        m_lastFireTime = Time.time;

        OnFire();
    }

    protected abstract void OnFire();

    public virtual void Reload()
    {
        if (m_isReloading || CurrentAmmo == MaxAmmo) 
            return;

        Facade.Instance.RequestPlaySFX(DefineString.SFX_GunReload, Vector3.zero, 0.3f, false);
        StartCoroutine(ReloadCoroutine());
    }

    protected virtual IEnumerator ReloadCoroutine()
    {
        m_isReloading = true;
        yield return new WaitForSeconds(ReloadTime);

        CurrentAmmo = MaxAmmo;
        m_isReloading = false;
    }
}