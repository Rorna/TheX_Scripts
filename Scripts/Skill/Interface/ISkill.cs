using System;
using UnityEngine;

public interface ISkill
{
    public SkillStateEnum CurrentState { get; }
    public SkillTypeEnum SkillType { get; }
    public SkillCastTypeEnum SkillCastType { get; }

    public void Init(GameObject caster, SkillData data);
    public void OnExecute(Action callback);
    public void Execute();
    public void Update();
    public void Cancel();
    public void Complete();
    public void Clear();

}
