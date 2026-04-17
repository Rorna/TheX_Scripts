using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// エナミーの全体的に管理
/// </summary>
public class EnemyManager : BaseManager
{
    public static EnemyManager Instance { get; private set; }

    private Dictionary<string, EnemyController> m_enemyDic = new Dictionary<string, EnemyController>();
    private Dictionary<string, EnemyData> m_enemyInfoDic;

    public static Action<EnemyController, float> OnEnemyDamageAction; //誰が、いくつのダメージを受けたか
    public static Action<EnemyController> OnSpawnEnemyAction;
    public static Action<EnemyController> OnEnemyDeadAction;

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
        InitEnemyInfoDic();
    }

    public override void ExternalInitManager()
    {
    }

    public override void RefreshManager()
    {
        m_enemyDic.Clear();
    }

    public void TrySetupEnemy(GameObject obj)
    {
        EnemyController controller = obj.GetComponent<EnemyController>();
        if (controller == null)
            return;

        string objName = obj.name;
        m_enemyDic.Add(objName, controller);

        //該当コントローラーとエナミーデータをHANDLERに渡す
        if (m_enemyInfoDic.TryGetValue(objName, out var enemyData) == false)
            return;

        controller.InitEnemyController(this, controller, enemyData);
    }

    private void InitEnemyInfoDic()
    {
        m_enemyInfoDic = new Dictionary<string, EnemyData>();

        // path
        string jsonPath = DefineString.JsonObjPath + DefineString.JsonEnemyInfo;
        string jsonText = JsonUtil.LoadJson(jsonPath);

        var loadedData = JsonUtil.ParseJson<EnemyInfo>(jsonText);

        m_enemyInfoDic = loadedData.EnemyInfoDic;
    }

    public void HandleSpawnEnemy(EnemyController enemy)
    {
        OnSpawnEnemyAction?.Invoke(enemy);
    }

    public void HandleDeadEnemy(EnemyController enemy)
    {
        OnEnemyDeadAction?.Invoke(enemy);
    }

    public void DamageEnemy(EnemyController controller, float damage)
    {
        OnEnemyDamageAction?.Invoke(controller, damage);
    }

    public void DeadEnemy(EnemyController controller)
    {
        // オブジェクト無効か
        var enemyName = controller.gameObject.name;
        Facade.Instance.RequestActiveObject(enemyName, false);

        // ディクショナリーから削除
        m_enemyDic.Remove(enemyName);
    }

    public override void ClearManager()
    {
    }
}