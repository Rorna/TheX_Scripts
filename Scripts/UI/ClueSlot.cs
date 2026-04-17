using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClueSlot : MonoBehaviour
{
    [SerializeField] private Button m_button;
    [SerializeField] private Image m_sprite;

    private event Action<string> m_clickAction;

    public string ID { get; private set; }
    private string m_title;
    private string m_description;
    private string m_spriteID;


    private void Awake()
    {
        m_button.onClick.AddListener(OnClick);
    }

    public void SetEmpty()
    {
        m_sprite.sprite = UnityUtil.GetSprite(DefineString.ClueSpritePath, DefineString.ClueEmpty);

        ID = string.Empty;
        m_title = string.Empty;
        m_description = string.Empty;
        m_spriteID = string.Empty;
        m_clickAction = null;
    }

    public void Setup(string id, string title, string desc, 
        string spriteID, Action<string> action)
    {
        ID = id;
        m_title = title;
        m_description = desc;
        m_spriteID = spriteID;

        m_clickAction = action;

        SetImage();
    }

    private void SetImage()
    {
        m_sprite.sprite = UnityUtil.GetSprite(DefineString.ClueSpritePath, m_spriteID);
    }

    private void OnClick()
    {
        m_clickAction?.Invoke(ID);
    }
}
