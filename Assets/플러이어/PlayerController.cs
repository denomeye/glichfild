using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public NavMeshAgent agent;
    public float attackRange = 3f;    // 사거리 (나중에 캐릭터별로 다르게)
    public float attackCooldown = 1f;   // 공격속도

    private GameObject targetEnemy;
    private float lastAttackTime;
    private bool isChasing;
    private PlayerAttack attack;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        attack = GetComponent<PlayerAttack>();

    }

    void Update()
    {
        if (agent == null)
        { 
            return;
        }
        if (targetEnemy != null && !targetEnemy.activeInHierarchy)
        {
            targetEnemy = null;
            isChasing = false;
        }
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // 플레이어 레이어 무시
            int mask = LayerMask.GetMask("Ground", "Enemy");

            if (Physics.Raycast(ray, out hit, 1000f, mask))
            {
                Debug.Log("맞은 오브젝트: " + hit.collider.name);

                if (hit.collider.CompareTag("Enemy"))
                {
                    targetEnemy = hit.collider.gameObject;
                    isChasing = true;
                }
                else
                {
                    targetEnemy = null;
                    isChasing = false;
                    agent.SetDestination(hit.point);
                }
            }
        }

        // 적 추격 중if (isChasing && t
        if (isChasing)
        {
            if (targetEnemy == null)
            {
                isChasing = false;
            }
            else
            {
                float dist = Vector3.Distance(transform.position,
                    targetEnemy.transform.position);

                if (dist <= attackRange)
                {
                    agent.ResetPath();
                    TryAttack();

                    agent.stoppingDistance = attackRange - 0.2f;
                }
                else
                {
                    agent.SetDestination(targetEnemy.transform.position);
                }
            }
        }
    }
    void TryAttack()
    {
        CenterureichiAttack ca = GetComponent<CenterureichiAttack>();
        if (ca != null)
        {
            ca.TryFire(targetEnemy);
            return;
        }

        // 기존 카카루용
        GetComponent<PlayerAttack>()?.DoAttack(targetEnemy);
    }
}
