using UnityEngine;

public class UIPopup : UIObject
{
    public Canvas PopUpCanvas { get; private set; }

    protected override void OnAwake()
    {
        Init();
    }

    public void Init()
    {
        PopUpCanvas = gameObject.GetComponent<Canvas>();
    }

    public override void OnShow()
    {
        //ポップアップスタック管理のため、ソートオーダーをリクエスト
        Facade.Instance.RequestControlPopupSortOrder(this, true);
        Facade.Instance.RequestPlaySFX(DefineString.SFX_UISound, Vector3.zero, 0.3f, false);
    }

    public override void OnHide()
    {
        //ポップアップスタック管理のため、ソートオーダーをリクエスト
        Facade.Instance.RequestControlPopupSortOrder(this, false);
        Facade.Instance.RequestPlaySFX(DefineString.SFX_UISound, Vector3.zero, 0.3f, false);
    }
}
