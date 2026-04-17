using System.Collections.Generic;
using UnityEngine;

///<summary>
///トリガオブジェクト制御スクリプト
///トリガ発動条件: TriggerActiveTypeEnum /m_activeValue
///トリガ発動後の実行ロジック: ActionTypeEnum /m_targetID
///</summary>
public class TriggerObject : MonoBehaviour
{
    [SerializeField]
    private TriggerActiveTypeEnum m_activeType;

    [SerializeField]
    private string m_activeValue;

    [SerializeField]
    private ActionTypeEnum m_actionType;

    [SerializeField]
    private string m_targetID;

    [SerializeField]
    private bool m_isRepeat; //チェックすると、通るたびにトリガー

    [SerializeField]
    private AudioClip m_clip;

    private ActionData m_actionData;

    private void Start()
    {
        m_actionData = new ActionData();
        m_actionData.SetActionType(m_actionType);
        m_actionData.SetTargetID(m_targetID);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null)
            return;

        //タイプ持っている？　ではタイプチェック
        if (m_activeType != TriggerActiveTypeEnum.None)
        {
            if (CheckActiveByType() == false)
                return;
        }

        //プレイヤーがトリガーと触れた情報を参照し、情報があったら発動しない。
        //なかったら実行
        HashSet<string> playerTriggerInfo = player.TriggerInfoHash;
        if (playerTriggerInfo.Contains(gameObject.name))
            return;
        else
        {
            Facade.Instance.RequestExecuteAction(m_actionData);

            if(m_clip != null)
                Facade.Instance.RequestPlaySFX(m_clip.name, gameObject.transform.position, 1.0f, false);
        }

        //リピートチェック解除されていたらトリガ情報を保存。
        if(m_isRepeat == false)
            Facade.Instance.RequestSaveTrigger(gameObject.name);
    }

    private bool CheckActiveByType()
    {
        switch (m_activeType)
        {
            case TriggerActiveTypeEnum.Quest:
                if (string.IsNullOrEmpty(m_activeValue))
                    return false;

                if (QuestManager.Instance.IsQuestComplete(m_activeValue))
                    return true;

                break;
            default:
                break;
        }

        return false;
    }
}
