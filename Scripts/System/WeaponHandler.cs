using UnityEngine;

public class WeaponHandler
{
    private PlayerController m_controller;
    public IWeapon CurrentWeapon { get; private set; }
    private GameObject m_weaponGameObj;
    public GameObject WeaponMountPoint { get; private set; }

    public float ReloadTime => CurrentWeapon.ReloadTime;
    public int MaxAmmo => CurrentWeapon.MaxAmmo;
    public int CurrentAmmo => CurrentWeapon.CurrentAmmo;
    public bool IsReloading => CurrentWeapon.IsReloading;

    public void Init(PlayerController controller)
    {
        m_controller = controller;

        var mainCam = CameraManager.Instance.MainCamera;
        WeaponMountPoint = UnityUtil.GetChildObject(mainCam, DefineString.WeaponMountPoint);

        InitDefaultWeapon();
    }

    private void InitDefaultWeapon()
    {
        //プレハブ生成、プレイヤーに取り付ける(プレイヤーコントローラ)
        string path = DefineString.ObjectPath + DefineString.DefaultGun;

        GameObject weapon = UnityUtil.Instantiate(path);
        if (weapon == null)
        {
            return;
        }

        m_weaponGameObj = weapon;
        CurrentWeapon = m_weaponGameObj.GetComponent<IWeapon>();

        m_weaponGameObj.transform.SetParent(WeaponMountPoint.transform, false);
        m_weaponGameObj.SetActive(false);
    }

    public void RefreshWeapon(bool active = false)
    {
        if (m_weaponGameObj == null)
            return;

        if(active)
        {
            m_weaponGameObj.SetActive(true);
            return;
        }

        m_weaponGameObj.SetActive(false);
    }

    public void TryFire()
    {
        CurrentWeapon?.Fire();
    }

    public void TryReload()
    {
        CurrentWeapon?.Reload();
    }
}
