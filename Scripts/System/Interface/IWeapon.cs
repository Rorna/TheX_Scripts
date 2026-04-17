
public interface IWeapon
{
    public float FireRate { get; }
    public float ReloadTime { get; }
    public int MaxAmmo { get; }
    public int CurrentAmmo { get; }
    public bool IsReloading { get; }
    public void Fire();
    public void Reload();
}
