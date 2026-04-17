using System;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

///<summary>
///取得したダイアログデータを設定、出力するクラス
///</summary>

public class UIDialog : UIObject, IDialog
{
    [SerializeField] private TextMeshProUGUI m_dialogText;
    [SerializeField] private GameObject m_choiceContainer;

    [SerializeField] private GameObject m_talkerObj;
    [SerializeField] private TextMeshProUGUI m_talkerText;

    private string m_currentText;
    private List<ChoiceButton> m_choiceButtonList;

    public void SetData(DialogNode node, Action<string> buttonAction = null)
    {
        SetText(node.Text);
        SetTalker(node.Talker);
        SetChoices(node.Choices, buttonAction);
    }

    private void SetText(string text)
    {
        m_currentText = text;
    }

    private void SetTalker(string talker)
    {
        if(string.IsNullOrEmpty(talker))
            m_talkerObj.SetActive(false);
        else
        {
            m_talkerObj.SetActive(true);
            m_talkerText.text = talker;
        }
    }

    private void SetChoices(List<DialogChoice> choiceList, Action<string> buttonAction)
    {
        //選択肢がない場合：選択肢UIを必ず整理　／　非表示（以前のノード残像を防止）
        if (choiceList == null || choiceList.Count == 0 || buttonAction == null)
        {
            if (m_choiceButtonList != null)
            {
                for (int i = 0; i < m_choiceButtonList.Count; i++)
                    m_choiceButtonList[i].gameObject.SetActive(false);
            }

            m_choiceContainer.SetActive(false);
            return;
        }

        if (m_choiceButtonList == null)
            m_choiceButtonList = new List<ChoiceButton>();

        m_choiceContainer.SetActive(true);

        int requireButtonCount = choiceList.Count;
        for (int i = 0; i < requireButtonCount; i++)
        {
            ChoiceButton button;

            //数が足りる
            if (m_choiceButtonList.Count > i)
            {
                button = m_choiceButtonList[i];
            }
            else        
            {
                button = UnityUtil.Instantiate(DefineString.ChoiceButtonPath, m_choiceContainer.transform).GetComponent<ChoiceButton>();
                if (button == null)
                    return;

                m_choiceButtonList.Add(button);
            }

            string finalTargetID = choiceList[i].TargetNodeID;

            if (choiceList[i].BranchType != DialogBranchTypeEnum.None)
            {
                //ブランチタイプに応じてバリューを異なる設定してノード情報を取得
                switch (choiceList[i].BranchType)
                {
                    case DialogBranchTypeEnum.Quest:

                        //このタイプはバリューが必ず必要
                        if (string.IsNullOrEmpty(choiceList[i].BranchValue))
                            return;

                        if (QuestManager.Instance.IsQuestComplete(choiceList[i].BranchValue))
                        {
                            finalTargetID = choiceList[i].TargetNodeID;　//　クエストクリア  
                        }
                        else
                        {
                            finalTargetID = choiceList[i].FailTargetNodeID;
                        }
                        break;
                    case DialogBranchTypeEnum.Save:
                        //saveデータがあるか確認
                        if (SaveManager.Instance.HasSaveData)
                        {
                            finalTargetID = choiceList[i].TargetNodeID;
                        }
                        else
                        {
                            finalTargetID = choiceList[i].FailTargetNodeID;
                        }
                        break;

                    case DialogBranchTypeEnum.Clue:
                        //wrong data
                        if (string.IsNullOrEmpty(choiceList[i].BranchValue))
                        {
                            return;
                        }

                        //なければ獲得
                        var clueDic = ClueManager.Instance.CurrentClueDic;
                        if (clueDic.ContainsKey(choiceList[i].BranchValue) == false)
                            finalTargetID = choiceList[i].FailTargetNodeID;

                        finalTargetID = choiceList[i].TargetNodeID;
                        break;
                    case DialogBranchTypeEnum.Count:
                        break;
                }
            }

            //ボタンアクションを追加。
            button.Setup(choiceList[i].Text, finalTargetID, buttonAction);      
            button.gameObject.SetActive(true);
        }

        //使っていないボタンを無効にする
        for (int i = requireButtonCount; i < m_choiceButtonList.Count; i++)
        {
            m_choiceButtonList[i].gameObject.SetActive(false);
        }
    }

    public void ShowDialog()
    {
        Show();

        if (m_currentText == null)
            return;

        m_dialogText.DOKill();
        m_dialogText.text = "";
        m_dialogText.DOText(m_currentText, m_currentText.Length * 0.2f);
    }

    public void HideDialog()
    {
        ClearMessage();
        m_choiceContainer.SetActive(false);
        Hide();
    }

    public bool IsProgressing()
    {
        //会話中だったらすぐに出力を終了
        if (DOTween.IsTweening(m_dialogText))
        {
            m_dialogText.text = m_currentText;
            m_dialogText.DOKill(true);
            return false;
        }
        return true;
    }

    public void EndThisDialog()
    {
        if (DOTween.IsTweening(m_dialogText))
        {
            m_dialogText.text = m_currentText;
            m_dialogText.DOKill(true);
        }
    }

    private void ClearMessage()
    {
        m_currentText = string.Empty;
        m_dialogText.text = m_currentText;
    }
}