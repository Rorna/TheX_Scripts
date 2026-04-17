using System;
using System.Collections;
using UnityEngine;

public class BulletTrail : MonoBehaviour
{
    private TrailRenderer m_trailRenderer;
    [SerializeField] private float m_speed;

    private Action<BulletTrail> m_onComplete; //完了時に呼び出すコールバック、プールで使用

    private void Awake()    
    {
        m_trailRenderer = GetComponent<TrailRenderer>();
    }

    public void SetupTrail(Vector3 startPos, Vector3 endPos, Action<BulletTrail> onComplete)
    {
        m_onComplete = onComplete;
        transform.position = startPos;

        m_trailRenderer.Clear();
        m_trailRenderer.emitting = true;

        StartCoroutine(CoMoveTrail(endPos));
    }

    ///<summary>
    ///実際の発射ロジックはレイキャストを使うので即ヒットだが、
    ///演出として意図的にディレイを設ける
    ///</summary>
    private IEnumerator CoMoveTrail(Vector3 targetPos)
    {
        float distance = Vector3.Distance(transform.position, targetPos);
        float remainingDistance = distance;

        while (remainingDistance > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, m_speed * Time.deltaTime);
            remainingDistance -= m_speed * Time.deltaTime;
            yield return null;
        }

        m_trailRenderer.emitting = false;
        yield return new WaitForSeconds(m_trailRenderer.time);

        m_onComplete?.Invoke(this);
    }
}