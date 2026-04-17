using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

///<summary>
///ボタンテキスト設定管理クラス
///ボタンアクションを登録して押すと実行する
///</summary>
public class ChoiceButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_buttonText;
    
    private Button m_button;
    public string TargetID { get; private set; } //dialog ID
    private event Action<string> m_clickAction; 

    private void Awake()
    {
        m_button = GetComponent<Button>();
        m_button.onClick.AddListener(OnClick);
    }

    public void Setup(string text, string targetID, Action<string> action)
    {
        m_buttonText.text = text;
        TargetID = targetID;
        m_clickAction = action;
    }

    private void OnClick()
    {
        m_clickAction?.Invoke(TargetID);
    }
}