using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;

    private GameObject target;

    // 타겟 지정 함수 추가
    public void SetTarget(GameObject t)
    {
        target = t;
    }

    void Update()
    {
        if (target != null)
        {
            // 타겟 방향으로 이동
            Vector3 dir = (target.transform.position - transform.position).normalized;
            transform.position += dir * speed * Time.deltaTime;

            // 근접하면 적중
            if (Vector3.Distance(transform.position, target.transform.position) < 1.2f)
            {
                EnemyHealth eh = target.GetComponent<EnemyHealth>();
                if (eh != null) eh.TakeDamage(30);
                Destroy(gameObject);
            }
        }
        else
        {
            // 타겟 없으면 기존처럼 직진
            transform.position += transform.forward * speed * Time.deltaTime;
        }
    }

    void Start()
    {
        Destroy(gameObject, 3f);
    }
}