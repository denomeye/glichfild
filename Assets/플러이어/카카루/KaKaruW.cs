using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class KakaruW : SkillBase
{
    [Header("스킬 수치")]
    public int skillLevel = 1;
    public float dashDistance = 4f;
    public float dashSpeed = 20f;
    public LayerMask enemyLayer;

    private float[] damages = { 70f, 105f, 140f, 175f };
    private float[] cooldowns = { 14f, 13f, 12f, 11f };
    private float[] manaCosts = { 70f, 80f, 90f, 100f };

    private PlayerStats stats;
    private NavMeshAgent agent;
    private bool isDashing = false;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
        agent = GetComponent<NavMeshAgent>();
        ApplyLevel();
    }

    public void ApplyLevel()
    {
        int idx = Mathf.Clamp(skillLevel - 1, 0, 3);
        cooldown = cooldowns[idx];
        manaCost = manaCosts[idx];
    }

    protected override void OnUse()
    {
        if (isDashing) return;
        StartCoroutine(DashRoutine());
    }

    IEnumerator DashRoutine()
    {
        isDashing = true;

        agent.ResetPath();
        agent.enabled = false;

        Vector3 startPos = transform.position;
        Vector3 dashTarget = transform.position + transform.forward * dashDistance;

        float elapsed = 0f;
        float duration = dashDistance / dashSpeed;
        bool hasHit = false;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(startPos, dashTarget, t);

            if (!hasHit)
            {
                Collider[] hits = Physics.OverlapSphere(
                    transform.position, 0.8f, enemyLayer);

                foreach (Collider hit in hits)
                {
                    EnemyHealth eh = hit.GetComponent<EnemyHealth>();
                    if (eh != null)
                    {
                        int idx = Mathf.Clamp(skillLevel - 1, 0, 3);
                        float attackPower = stats != null ? stats.attackPower : 64f;
                        float totalDamage = damages[idx] + attackPower * 0.8f;

                        // R 활성화 중 방어 관통 보정
                        KakaruR r = GetComponent<KakaruR>();
                        if (r != null && r.IsActive)
                            totalDamage *= (1f + r.DamageBonus);

                        eh.TakeDamage((int)totalDamage);

                        EnemyStatus status = hit.GetComponent<EnemyStatus>();
                        if (status != null)
                            status.ApplyStun(1f);

                        Debug.Log($"사냥 임무 적중! 피해: {totalDamage}");
                        hasHit = true;
                    }
                }
            }

            yield return null;
        }

        agent.enabled = true;
        isDashing = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(
            transform.position + transform.forward * dashDistance, 0.8f);
    }
}