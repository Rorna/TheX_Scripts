using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 次のダイアログが進められるかどうかはこちらで判断
/// ここでアクションも実行
/// </summary>
public class DialogController
{
    private IDialog m_dialogUI;
    private DialogSession m_session;
    private DialogNode m_currentNode; //設定するノード
    public string CurrentDialogObjName { get; private set; } //現在の会話相手
    public bool IsOnDialog { get; private set; } // 現在ダイアログ中か。

    public static event Action<string> OnDialogTargetAction; //ターゲットと会話した。

    private DialogData m_cachedDialogData;

    // キャッシュキュー、会話中外部から会話要求入ったとき順次処理
    private Queue<DialogData> m_dialogQueue = new Queue<DialogData>();


    public void Init(IDialog iHUD)
    {
        m_session = new DialogSession();
        m_dialogUI = iHUD;
        BindEvent();
    }

    public void Release()
    {
        UnBindEvent();
        m_dialogQueue.Clear();
    }

    private void BindEvent()
    {
        InputManager.OnDialogAction += ShowDialog;
        CutsceneManager.OnCutsceneStateChangeAction += HandleCutsceneStateChangeAction;
    }

    private void UnBindEvent()
    {
        InputManager.OnDialogAction -= ShowDialog;
        CutsceneManager.OnCutsceneStateChangeAction -= HandleCutsceneStateChangeAction;
    }

    // 最初の会話を開始。
    public void StartDialog(DialogData data)
    {
        // 会話中だったらキューに入れる
        if (IsOnDialog)
        {
            m_dialogQueue.Enqueue(data);
            return;
        }

        m_session.SetSession(data.StartNodeID, data.Nodes);

        if (PlayerManager.Instance.CurrentTarget != null)
            CurrentDialogObjName = PlayerManager.Instance.CurrentTarget.ObjectName;

        SetDialogData();
        PlaySound();
        m_dialogUI.ShowDialog();

        // 現在のノードに基づいて
        // 設定する次のノードをセッションにリクエスト
        m_session.RefreshSetUpNode(GetRealNextNodeID(m_currentNode));

        OnDialogTargetAction?.Invoke(CurrentDialogObjName);
        IsOnDialog = true;

        DialogManager.Instance.HandleDialogStateChange(true);
    }

    private void SetDialogData()
    {
        m_currentNode = m_session.GetNodeToSetup();
        m_dialogUI.SetData(m_currentNode, ShowTargetDialog);
        SetDialogNode(m_currentNode);
    }

    private void SetDialogNode(DialogNode dialogNode)
    {
        SetAction(dialogNode.Action);
    }

    private void SetAction(ActionData actionData)
    {
        if (actionData == null)
            return;

        // アクションがあれば実行
        Facade.Instance.RequestExecuteAction(actionData);
    }

    // 該当ノードについて次のノードを決める
    // クエストが完了したかどうかによって、次のノードID設定
    private string GetRealNextNodeID(DialogNode node)
    {
        if (node == null) 
            return null;

        // ブランチタイプの確認
        if (node.BranchType != DialogBranchTypeEnum.None)
        {
            // ブランチタイプによってバリューを別々に設定してノードの情報を取得
            switch (node.BranchType)
            {
                case DialogBranchTypeEnum.Quest:

                    // wrong data
                    if (string.IsNullOrEmpty(node.BranchValue))
                    {
                        return node.FailTargetNodeID;
                    }

                    if (QuestManager.Instance.IsQuestComplete(node.BranchValue))
                    {
                        return node.NextNodeID; // クエストクリア
                    }
                    return node.FailTargetNodeID; // クエストを破る
                    break;

                case DialogBranchTypeEnum.Clue:
                    // wrong data
                    if (string.IsNullOrEmpty(node.BranchValue))
                    {
                        return null;
                    }

                    var clueDic = ClueManager.Instance.CurrentClueDic;
                    if (clueDic.ContainsKey(node.BranchValue) == false)
                        return node.FailTargetNodeID;

                    return node.NextNodeID;
                    break;

                case DialogBranchTypeEnum.Count:
                    break;
            }
        }
        
        // ブランチがない場合、元の nextNodeID リターン
        return node.NextNodeID;
    }

    // 次の会話設定
    public void ShowDialog()
    {
        // カーソルがUIHUDの上にある　ー＞　進まない
        GameObject uiObj = UnityUtil.GetTopUIOnCursorPos();
        if (uiObj != null)
        {
            var uihud = uiObj.GetComponentInParent<UIHUD>();
            if (uihud != null)
                return;
        }

        // カーソルがPopupの上にある　ー＞　進まない
        GameObject uipopup = UnityUtil.GetTopUIOnCursorPos();
        if (uipopup != null)
        {
            var cluePopup = uipopup.GetComponentInParent<UIPopup>();
            if (cluePopup != null)
                return;
        }

        if (CanSetDialog() == false)
            return;

        SetDialogData();
        m_dialogUI.ShowDialog();
        PlaySound();

        m_session.RefreshSetUpNode(GetRealNextNodeID(m_currentNode));
    }

    private void PlaySound()
    {
        if (string.IsNullOrEmpty(m_currentNode.Sound))
            return;

        string soundID = m_currentNode.Sound;
        Facade.Instance.RequestPlaySFX(soundID, Vector3.zero, 0.4f, false);
    }

    /// <summary>
    /// ボタンを押したとき、次に行けるか確認
    /// </summary>
    /// <param name="targetID"></param>
    public void ShowTargetDialog(string targetID)
    {
        if (UISystemManager.Instance.HasActivePopup)
            return;

        if (CanSetDialog(targetID) == false)
            return;

        m_session.RefreshSetUpNode(targetID);

        SetDialogData();
        m_dialogUI.ShowDialog();
        PlaySound();

        m_session.RefreshSetUpNode(GetRealNextNodeID(m_currentNode));
    }

    // 色々チェックして、次の会話に進められるか確認。
    private bool CanSetDialog(string targetID = null)
    {
        // ターゲットノードIDにアクセスできるかどうかをチェック
        if (targetID != null)
        {
            // check action
            ActionData actionData = m_session.GetTargetActionData(targetID);
            if (actionData != null && CanRunTargetAction(actionData) == false)
                return false;
        }

        // uiが会話中だったら
        // 現在の会話を即時終了
        if (m_dialogUI.IsProgressing() == false)
        {
            m_dialogUI.EndThisDialog();
            return false;
        }

        // テキストがない場合（最後のノード/選択肢なし）会話を終了
        // テキストない。が、次のノードIDがあったら？　ー＞　無効なデータ
        // テキストない。が、アクションがあった？アクションのみ実行後にダイアログを閉じる
        // つまり、テキストがなくても良いのは、最後のノードである場合のみ。次のノードID /選択肢があったらだめ
        var targetIDNode = m_session.GetTargetNode(targetID);
        if (string.IsNullOrEmpty(targetIDNode.Text))
        {
            if (GetRealNextNodeID(targetIDNode) != null || targetIDNode.Choices != null)
            {
                return false;
            }

            // アクションがあれば実行後終了
            if (targetIDNode.Action != null)
            {
                Facade.Instance.RequestExecuteAction(targetIDNode.Action);
            }

            EndDialog();
            return false;
        }


        // 最後のノードチェック
        // 次のもない、行くべきところもない
        // クエスト情報もない
        if (GetRealNextNodeID(m_currentNode) == null && targetID == null)
        {
            // しかし、選択肢だ。
            // 選択肢を選ぶまで何もしない。
            if (m_currentNode.Choices != null)
                return false;

            EndDialog();
            return false;
        }

        // 現在のノードと設定するノードが同じであれば設定する必要はない
        if (m_currentNode == m_session.GetTargetNode(targetID))
            return false;

        return true;
    }


    /// <summary>
    /// 同じクエスト、アクションは受け入れない
    /// </summary>
    private bool CanRunTargetAction(ActionData actionData)
    {
        switch (actionData.ActionType)
        {
            case ActionTypeEnum.GetQuest:
                if (QuestManager.Instance.CanGetQuest == false)
                {
                    return false;
                }
                break;
            case ActionTypeEnum.GetClue:
                if (Facade.Instance.RequestCanGetClue(actionData.TargetID) == false)
                {
                    return false;
                }
                break;
        }

        return true;
    }

    public void EndDialog()
    {
        EndDialogInternal(true); // イベントを送信する
    }

    private void EndDialogInternal(bool triggerEvent)
    {
        if (IsOnDialog == false)
        {
            m_dialogUI.HideDialog();
            return;
        }

        m_dialogUI.HideDialog();
        m_session.ClearSession();

        IsOnDialog = false; // 現在の会話状態を解除

        // キャッシュデータの確認
        if (m_dialogQueue.Count > 0)
        {
            DialogData nextDialog = m_dialogQueue.Dequeue();
            StartDialog(nextDialog);
            return;
        }

        // 会話完了
        if (triggerEvent)
        {
            DialogManager.Instance.HandleDialogStateChange(false);
        }
    }

    // イベント送信せずに会話終了
    public void ForceEndDialog()
    {
        EndDialogInternal(false); // イベントを送信しない
    }

    private void HandleCutsceneStateChangeAction(CutsceneStateEnum state)
    {
        switch (state)
        {
            // カットシーン開始の時に会話中のものを終了
            case CutsceneStateEnum.Play:
                ForceEndDialog();
                break;
        }
    }

    public void RefreshController()
    {
        EndDialog();
        m_dialogQueue.Clear();
    }
}
