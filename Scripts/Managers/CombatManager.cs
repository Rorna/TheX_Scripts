using System;
using UnityEngine;
///<summary>
///戦闘の開始・終了を管理する。
///開始・終了のリクエストがあったら処理する
///</summary>
public class CombatManager : BaseManager
{
    public static CombatManager Instance { get; private set; }
    private CombatController m_controller;

    public static Action<bool> OnCombatActiveAction;        
    
    public override void CreateManager()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public override void InternalInitManager()
    {
        InitController();
    }

    public override void ExternalInitManager()
    {
        
    }

    private void InitController()
    {
        m_controller = new CombatController();
        m_controller.InitController(this);
    }

    public override void RefreshManager()
    {
        
    }

    public override void ClearManager()
    {
        m_controller.Release();
    }

    public void HandleCombatActive(bool active)
    {
        OnCombatActiveAction?.Invoke(active);
    }

    public void TryStartCombat(string targetName)
    {
        //オブジェクトマネージャーから生成された敵を受け取る
        GameObject enemyObj = Facade.Instance.RequestSpawnEnemy(targetName);
        if (enemyObj == null)
            return;

        EnemyController enemy = enemyObj.GetComponent<EnemyController>();
        if (enemy == null)
            return;

        m_controller.StartCombat(enemy);
    }
}
