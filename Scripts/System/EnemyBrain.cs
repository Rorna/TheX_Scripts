using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

///<summary>
///エネミーAI。
///fsmと行動管理。
///</summary>
public class EnemyBrain
{
    private EnemyController m_controller;
    private EnemyFSM m_fsm;

    private float m_minActionInterval;
    private float m_maxActionInterval;

    private float m_timer;

    public float RandomActionInterval { get; private set; }

    private List<EnemyActionEnum> m_actionList = new List<EnemyActionEnum>();
    private List<SkillEnum> m_skillList = new List<SkillEnum>();

    public List<SkillEnum> SkillList => m_skillList;
    public EnemyController Controller => m_controller;

    public EnemyFSM FSM => m_fsm;
    private EnemySkillHandler m_skillHandler;

    public void InitBrain(EnemyController controller)
    {
        m_controller = controller;
        m_actionList = controller.ActionList;
        m_skillList = controller.SkillList;

        m_minActionInterval = m_controller.MinActionInterval;
        m_maxActionInterval = m_controller.MaxActionInterval;

        m_fsm = new EnemyFSM(this);
        m_skillHandler = new EnemySkillHandler();
        m_skillHandler.Init(this);
    }

    public void BrainUpdate()
    {
        m_fsm.Update();
        m_skillHandler.UpdateSkill();

        //IDLE時のみタイマーを減らす
        if (m_fsm.CurrentStateEnum == EnemyStateEnum.Idle)
        {
            m_timer -= Time.deltaTime;
            if (m_timer <= 0f)
            {
                ExecuteRandomAction();
            }
        }
    }

    public void BrainFixedUpdate()
    {
        m_fsm.FixedUpdate();
    }

    private void ExecuteRandomAction()
    {
        int randomIndex = Random.Range(0, m_actionList.Count);
        EnemyActionEnum action = m_actionList[randomIndex];

        //状態遷移要求  
        var actionState = m_fsm.CurrentState as IEnemyActionState;
        switch (action)
        {
            case EnemyActionEnum.Idle:
                actionState?.HandleIdle();
                break;
            case EnemyActionEnum.Move:
                actionState?.HandleMove();
                break;
            case EnemyActionEnum.Jump:
                actionState?.HandleJump();
                break;
            case EnemyActionEnum.Skill:
                SetRandomSkill();
                actionState?.HandleAttack();
                break;
        }
    }

    public void ExecuteSkill(Action callback)
    {
        m_skillHandler.ExecuteSkill(callback);
    }

    private void SetRandomSkill()
    {
        if (SkillList.Count == 0)
            return;

        int randomIndex = Random.Range(0, SkillList.Count);
        SkillEnum skill = SkillList[randomIndex];

        m_skillHandler.InitSkill(skill);
    }

    public void ExecuteAction(EnemyActionEnum state)
    {
        var actionState = m_fsm.CurrentState as IEnemyActionState;
        switch (state)
        {
            case EnemyActionEnum.Idle:
                actionState?.HandleIdle();
                break;
            case EnemyActionEnum.Move:
                actionState?.HandleMove();
                break;
            case EnemyActionEnum.Jump:
                actionState?.HandleJump();
                break;
            case EnemyActionEnum.Skill:
                actionState?.HandleAttack();
                break;
        }
    }

    public void ResetInterval()
    {
        RandomActionInterval = Random.Range(m_minActionInterval, m_maxActionInterval);
        m_timer = RandomActionInterval;
    }

    private void RefreshAction()
    {
    }
}