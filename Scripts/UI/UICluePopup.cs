using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICluePopup : UIPopup, ICluePopup
{
    [SerializeField] private Image m_sprite;
    [SerializeField] private TMP_Text m_title;
    [SerializeField] private TMP_Text m_desc;

    public bool IsActive => gameObject.activeSelf;

    public void ShowPopup(string spriteID, string title, string desc)
    {
        m_sprite.sprite = UnityUtil.GetSprite(DefineString.ClueSpritePath, spriteID);
        m_title.text = title;
        m_desc.text = desc;

        Show();
    }

    public void HidePopup()
    {
        Hide();
    }

    public void OnClickExit()
    {
        ClearPopup();
        Facade.Instance.RequestPlaySFX(DefineString.SFX_UISound, Vector3.zero, 0.3f, false);
        Hide();
    }

    public void ClearPopup()
    {
        m_title.text = "";
        m_desc.text = "";
        m_sprite.sprite = null;
    }
}
