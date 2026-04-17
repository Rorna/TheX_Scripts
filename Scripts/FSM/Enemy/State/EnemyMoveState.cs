using UnityEngine;

public class EnemyMoveState : IEnemyActionState
{
    private EnemyFSM m_fsm;
    private EnemyBrain m_brain;
    private EnemyController m_controller;

    //ランダム値
    private float m_baseDist;
    private float m_moveDist;
    private float m_moveAngle;
    private float m_moveDuration;

    private Vector3 m_targetPos;

    private bool m_isArrived;

    public EnemyMoveState(EnemyFSM fsm, EnemyBrain brain)
    {
        m_fsm = fsm;
        m_brain = brain;
        m_controller = brain.Controller;
    }

    public void Enter()
    {
        m_controller.Anim.SetBool("IsWalk", true);

        m_isArrived = false;

        //ランダム値抽出（移動距離、移動時間、ベクトル角度）
        SetMovementValues();

        //現在のプレイヤーとエナミーとの間の距離を測定する
        Vector3 playerPos = m_controller.Player.transform.position;
        playerPos.y = 0f;

        Vector3 enemyPos = m_controller.Enemy.transform.position;
        enemyPos.y = 0f;

        //現在の距離と基準距離を比較し、方向ベクトルを取得。
        float currentDist = Vector3.Distance(playerPos, enemyPos);
        Vector3 dirVector = Vector3.zero;

        if (currentDist >= m_baseDist)  
        {
            dirVector = (playerPos - enemyPos).normalized;
        }
        else    
        {
            dirVector = (enemyPos - playerPos).normalized;
        }

        Vector3 startPos = m_controller.Enemy.transform.position;
        startPos.y += 0.5f;

        //すぐ後ろに壁があるときは方向を反転する。後ろに行けないなら反転する
        if (Physics.Raycast(startPos, dirVector, 2f, LayerMask.GetMask("Wall")))
        {
            dirVector *= -1f;
        }

        //取得した方向ベクトルを挿入して目標座標を設定します。
        SetTargetPos(dirVector);
    }

    private void SetMovementValues()
    {
        m_baseDist = m_brain.Controller.BaseMoveDistance;
        m_moveDist = Random.Range(m_controller.MinMoveDistance, m_controller.MaxMoveDistance);
        m_moveAngle = Random.Range(m_controller.MinMoveAngle, m_controller.MaxMoveAngle);
        m_moveDuration = Random.Range(m_controller.MinMoveDuration, m_controller.MaxMoveDuration);
    }

    //targetPos=currentPos+(dirVector*dist)
    private void SetTargetPos(Vector3 normalizedVec)
    {
        Vector3 randomDir = Quaternion.Euler(0f, m_moveAngle, 0f) * normalizedVec;
        m_targetPos = m_controller.Enemy.transform.position + (randomDir * m_moveDist);
    }

    public void Update()
    {
        m_moveDuration -= Time.deltaTime;
        if (m_moveDuration <= 0f || m_isArrived)
        {
            m_fsm.ChangeState(EnemyStateEnum.Idle);
        }
    }

    public void FixedUpdate()
    {
        //時間が経過したり目的地に到達したら終わり
        if (m_isArrived == false)
        {
            MoveToTarget();
        }
    }

    private void MoveToTarget()
    {
        Transform enemyTr = m_brain.Controller.Enemy.transform;

        //目的地到着チェック
        if (Vector3.Distance(enemyTr.position, m_targetPos) <= 0.25f)
        {
            m_isArrived = true;
            m_brain.Controller.StopMovement();
            return;
        }

        //現在位置から目標位置に向かう方向ベクトルを取得
        Vector3 moveDirection = (m_targetPos - enemyTr.position).normalized;
        moveDirection.y = 0f;

        m_brain.Controller.MoveEnemy(moveDirection);
    }

    public void Exit()
    {
        m_controller.StopMovement();
    }

    public void HandleIdle()
    {
        m_fsm.ChangeState(EnemyStateEnum.Idle);
    }

    public void HandleMove()
    { }

    public void HandleJump()
    {
        m_fsm.ChangeState(EnemyStateEnum.Jump);
    }

    public void HandleChase()
    { }

    public void HandleAttack()
    { }
}