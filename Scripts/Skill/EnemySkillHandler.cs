using System;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///スキル管理クラス
///スキルの状態管理と制御を担当。
///</summary>
public class EnemySkillHandler
{
    private ISkill m_currentSkill;
    private EnemyBrain m_brain;

    private Dictionary<SkillEnum, SkillData> m_skillDic;

    private Dictionary<SkillEnum, ISkill> m_cachedSkillDic = new Dictionary<SkillEnum, ISkill>();

    public void Init(EnemyBrain brain)
    {
        m_brain = brain;
        InitSkillDic();
    }

    private void InitSkillDic()
    {
        m_skillDic = new Dictionary<SkillEnum, SkillData>();

        //path
        string jsonPath = DefineString.JsonSkillPath + DefineString.JsonSkillInfo;
        string jsonText = JsonUtil.LoadJson(jsonPath);

        var loadedData = JsonUtil.ParseJson<SkillInfo>(jsonText);

        m_skillDic = loadedData.SkillInfoDic;
    }
    public void InitSkill(SkillEnum skill)
    {
        if (m_skillDic.TryGetValue(skill, out var skillData) == false) 
            return;

        if (m_cachedSkillDic.ContainsKey(skill) == false)
        {
            ISkill newSkill = null;
            switch (skill)
            {
                case SkillEnum.RushAttack: 
                    newSkill = new RushAttack(); 
                    break;
                case SkillEnum.ShockWaveAttack: 
                    newSkill = new ShockWaveAttack(); 
                    break;
            }

            if (newSkill != null)
            {
                newSkill.Init(m_brain.Controller.gameObject, skillData);
                m_cachedSkillDic.Add(skill, newSkill);
            }
        }

        m_currentSkill = m_cachedSkillDic[skill];

        m_currentSkill.Clear();
        m_currentSkill.Init(m_brain.Controller.gameObject, skillData);
    }

    public void UpdateSkill()
    {
        if (m_currentSkill == null)
            return;

        if (m_currentSkill.CurrentState == SkillStateEnum.Running)
        {
            m_currentSkill.Update();
        }
    }

    public void ExecuteSkill(Action endCallback)
    {
        if (m_currentSkill == null) 
            return;

        //渡されたコールバックにハンドラコールバック（スキル初期化）を追加する
        void addedCallback()
        {
            endCallback?.Invoke(); //元のコールバック

            //skill クリア
            m_currentSkill = null;
        }

        m_currentSkill.OnExecute(addedCallback);
    }
}
