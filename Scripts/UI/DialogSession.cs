using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MVCパターンにおける「Model（モデル）」を担当するセッションクラス。
/// ダイアログのデータと現在の状態（ステート）を保持する。
/// 
/// コントローラーは本クラスから提供されるデータを用いてダイアログを制御する。
/// 重要：セッションはあくまで「データと状態の保持」に徹し、実際の進行管理やロジック処理は行わない
/// </summary>
public class DialogSession 
{
    private Dictionary<string, DialogNode> m_nodeDic; //データ
    private DialogNode m_setUpNode; // セットアップ対象ノード

    public void SetSession(string startNodeID, Dictionary<string, DialogNode> nodeDic)
    {
        m_nodeDic = new Dictionary<string, DialogNode>(nodeDic);
        m_setUpNode = m_nodeDic[startNodeID];
    }

    public ActionData GetTargetActionData(string targetID)
    {
        if (m_nodeDic.TryGetValue(targetID, out DialogNode node) == false)
        {
            return null;
        }

        return node.Action;
    }

    public DialogNode GetNodeToSetup()
    {
        return m_setUpNode;
    }

    public DialogNode GetTargetNode(string targetID)
    {
        return targetID == null ? m_setUpNode : m_nodeDic[targetID];
    }

    /// <summary>
    /// セットアップ対象ノードの更新
    /// </summary>
    public void RefreshSetUpNode(string nextNodeID)
    {
        // 遷移先のノードIDがない場合は何もしない
        if (nextNodeID == null)
            return;

        if (m_nodeDic != null && m_nodeDic.TryGetValue(nextNodeID, out var node))
            m_setUpNode = node;
    }

    public void ClearSession()
    {
        m_nodeDic?.Clear();
        m_setUpNode = null;
    }
}