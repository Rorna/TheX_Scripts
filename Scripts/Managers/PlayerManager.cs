using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : BaseManager, ISavable
{
    public static PlayerManager Instance { get; private set; }

    public static event Action<Interactable> OnInteractTargetChanged; //ターゲット変更とか
    public static event Action<int> OnDamageAction; //hud 更新とか
    public static event Action OnPlayerDeathAction;

    private PlayerController m_controller;
    public GameObject Player => m_controller?.Player;
    public Interactable CurrentTarget => m_controller?.CurrentTarget;
    public int JudgementBullet => m_controller.JudgementBullet;

    private SaveData m_cachedSaveData;

    public GameObject WeaponMountPoint => m_controller.WeaponHandler.WeaponMountPoint;

    public WeaponHandler WeaponHandler => m_controller.WeaponHandler;
    public int HP => m_controller.CurrentHP;
    public int MaxHP => m_controller.MaxHP;
    public bool IsDead => m_controller.IsDead;

    public HashSet<string> TriggerInfoHash => m_controller.TriggerInfoHash;

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
    }

    public override void ExternalInitManager()
    {
        Facade.Instance.RequestRegisterSaveData(this);
    }

    public GameObject GetPlayer()
    {
        if (m_controller == null)
            return null;

        if (m_controller.Player == null)
            return null;

        return m_controller.Player;
    }

    public void RegisterController(PlayerController controller)
    {
        m_controller = controller;
        m_controller.Init();

        if(m_cachedSaveData != null)
            m_controller.LoadCachedSaveData(m_cachedSaveData);

        //player はプレーヤーマネージャーの下に置く
        controller.transform.SetParent(gameObject.transform);
    }

    public void HandleTargetChange(Interactable target)
    {
        OnInteractTargetChanged?.Invoke(target);
    }

    public Interactable TryGetCurrentTarget()
    {
        return m_controller.CurrentTarget;
    }

    public override void RefreshManager()
    {
        if (m_controller == null)
            return;

        m_controller.RefreshController();
    }

    public override void ClearManager()
    {
        if (m_controller == null)
            return;

        m_controller.Release();
    }

    public void TryRefreshJudgementBullet(int count)
    {
        m_controller.RefreshJudgementBullet(count);
    }

    public void SaveGameData(SaveData saveData)
    {
        m_controller.SaveGame(saveData);
    }

    public void LoadGameData(SaveData saveData)
    {
        //コントローラがnull（生成されていない場合）の場合
        //セーブデータをキャッシュする
        //コントローラが生成されるとき、キャッシュセーブデータがあったらセーブデータを設定する
        if (m_controller == null)
        {
            m_cachedSaveData = saveData;
            return;
        }

        m_controller.LoadGame(saveData);
    }

    public void HandleDamage(int damage)
    {
        OnDamageAction?.Invoke(damage);
    }

    public void HandleDeath()
    {
        OnPlayerDeathAction?.Invoke();
    }

    public void TrySaveTrigger(string triggerName)
    {
        m_controller.SaveTriggerInfo(triggerName);
    }
}
