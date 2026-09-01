using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class JinchuuW : SkillBase
{
    [Header("스킬 수치")]
    public int skillLevel = 1;
    public LayerMask enemyLayer;

    private float[] damages = { 60f, 90f, 120f };
    private float[] cooldowns = { 8f, 7f, 6f };
    private float[] manaCosts = { 65f, 75f, 85f };

    private float dashDistance = 5f;   // 500유닛
    private float dashSpeed = 20f;

    private PlayerStats stats;
    private NavMeshAgent navAgent;
    private JinchuuPassive passive;
    private bool isDashing = false;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
        navAgent = GetComponent<NavMeshAgent>();
        passive = GetComponent<JinchuuPassive>();
        ApplyLevel();
    }

    public void ApplyLevel()
    {
        int idx = Mathf.Clamp(skillLevel - 1, 0, 2);
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

        navAgent.ResetPath();
        navAgent.enabled = false;

        Vector3 startPos = transform.position;
        Vector3 dashTarget = transform.position
            + transform.forward * dashDistance;

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
                        int idx = Mathf.Clamp(skillLevel - 1, 0, 2);
                        float atkPower = stats != null
                            ? stats.attackPower : 58f;
                        float totalDamage = damages[idx] + atkPower * 0.7f;

                        eh.TakeDamage((int)totalDamage);

                        // 명중 시 패시브 스택 적립
                        if (passive != null) passive.OnHit();

                        Debug.Log($"견천하 적중! 데미지: {totalDamage}");
                        hasHit = true;
                    }
                }
            }

            yield return null;
        }

        navAgent.enabled = true;
        isDashing = false;
    }
}
