using UnityEngine;

///<summary>
///CircleShockWave プレハブの制御スクリプト
///生成/消滅/ダメージ管理
///</summary>
public class ShockwaveController : MonoBehaviour
{
    private float m_scaleUpTime;
    private float m_scaleUpParam;
    private float m_lifeTime;

    private float m_elapsedScaleUpTime = 0f;
    private ParticleSystem m_ps;

    private bool m_isReturned = false; //接触時の削除可否判定用
    private int m_damage;

    private float m_currentTime;

    private void Awake()
    {
        m_ps = GetComponent<ParticleSystem>();
    }

    public void Init(float scaleUpTime, float scaleUpParam, float lifeTime, int damage)
    {
        m_scaleUpTime = scaleUpTime;
        m_scaleUpParam = scaleUpParam;
        m_lifeTime = lifeTime;
        m_damage = damage;

        transform.localScale = Vector3.one;
        m_elapsedScaleUpTime = 0f;

        m_isReturned = false;

        m_currentTime = 0f;
    }

    void Update()
    {
        m_currentTime += Time.deltaTime;
        if (m_currentTime >= m_lifeTime)
        {
            m_ps.Stop();
            m_ps.Clear();

            Facade.Instance.RequestDestroyParticle(m_ps);
        }

        //時間とともにスケールが大きくなる
        m_elapsedScaleUpTime += Time.deltaTime;
        if (m_elapsedScaleUpTime >= m_scaleUpTime)
        {
            transform.localScale += new Vector3(m_scaleUpParam, m_scaleUpParam, m_scaleUpParam);
            m_elapsedScaleUpTime = 0f;
        }
    }

    private void OnParticleCollision(GameObject obj)
    {
        if (m_isReturned) 
            return;

        var player = PlayerManager.Instance.Player;
        if (obj == player)
        {
            IDamagable damagable = obj.GetComponent<IDamagable>();
            if (damagable != null)
            {
                damagable.TakeDamage(m_damage);
            }
        }
    }
}