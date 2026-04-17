public class CombatController
{
    private bool m_controllerActive;
    private CombatManager m_manager;

    private EnemyController m_target;

    public void InitController(CombatManager manager)
    {
        m_manager = manager;
        BindEvent();
    }

    private void BindEvent()
    {
        PlayerManager.OnPlayerDeathAction += HandlePlayerDeathAction;
        EnemyManager.OnEnemyDeadAction += HandleDeadEnemyAction;
    }

    private void UnBindEvent()
    {
        PlayerManager.OnPlayerDeathAction -= HandlePlayerDeathAction;
        EnemyManager.OnEnemyDeadAction -= HandleDeadEnemyAction;
    }

    private void HandlePlayerDeathAction()
    {
        if (m_controllerActive == false)
            return;

        Facade.Instance.RequestRunGameOver();
    }

    public void Release()
    {
        UnBindEvent();
    }

    public void StartCombat(EnemyController enemy)
    {
        m_target = enemy;

        //入力タイプを変更
        m_manager.HandleCombatActive(true);
        m_controllerActive = true;

        //戻りオブジェクトを無効化
        Facade.Instance.RequestActiveObject(DefineString.ReturnObjectName, false);
        Facade.Instance.RequestPlaySceneBGM(SceneBGMTypeEnum.CombatBGM, 0, true, 0.5f);
    }

    private void HandleDeadEnemyAction(EnemyController enemy)
    {
        if (m_target != enemy)
            return;

        EndCombat();

        m_target = null;
    }

    private void EndCombat()
    {
        m_manager.HandleCombatActive(false);
        m_controllerActive = false;

        //帰還オブジェクトの有効化
        Facade.Instance.RequestActiveObject(DefineString.ReturnObjectName, true);
        Facade.Instance.RequestStopBGM(true, 0.5f);
    }
}
