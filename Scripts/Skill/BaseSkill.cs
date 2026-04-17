using System;
using UnityEngine;

//すべてのスキルクラスはこのクラスを継承しなければならない
public abstract class BaseSkill : ISkill
{
    public GameObject m_caster;
    public GameObject Caster => m_caster;

    public event Action OnSkillCompleteAction;

    //values
    public int Damage { get; private set; }

    public SkillStateEnum CurrentState { get; protected set; } = SkillStateEnum.Ready;
    public SkillTypeEnum SkillType { get; protected set; }
    public SkillCastTypeEnum SkillCastType { get; protected set; }

    public virtual void Init(GameObject caster, SkillData data)
    {
        m_caster = caster;

        //データが追加されたら、メンバー変数　+　ここで初期化する
        Damage = data.Damage;
        CurrentState = SkillStateEnum.Ready;
        SkillType = data.SkillType;
        SkillCastType = data.SkillCastType;
    }

    public void OnExecute(Action callback)
    {
        //終了コールバックを登録する
        OnSkillCompleteAction = callback;

        //実行。
        CurrentState = SkillStateEnum.Running;
        Execute();
    }

    protected void InvokeSkillComplete()
    {
        OnSkillCompleteAction?.Invoke();
    }

    public abstract void Execute();

    public abstract void Update();

    public abstract void Cancel();

    public abstract void Complete();
    public abstract void Clear();
}
