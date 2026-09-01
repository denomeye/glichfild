using UnityEngine;

public class PerfumerE : SkillBase
{
    [Header("스킬 수치")]
    public int skillLevel = 1;
    public LayerMask enemyLayer;

    private float[] damages = { 60f, 90f, 120f };
    private float[] cooldowns = { 12f, 11f, 10f };
    private float[] manaCosts = { 95f, 95f, 95f };

    private float slowAmount = 0.25f; // 둔화 25%
    private float slowDuration = 1.5f;
    private float coneAngle = 70f;   // 부채꼴 각도
    private float coneRange = 3.75f; // 사거리 375

    private float apRatio = 0.4f;

    private PlayerStats stats;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
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
        int idx = Mathf.Clamp(skillLevel - 1, 0, 2);
        float spellPower = stats != null ? stats.spellPower : 45f;
        float totalDamage = damages[idx] + spellPower * apRatio;

        // 부채꼴 범위 내 적 감지
        Collider[] hits = Physics.OverlapSphere(
            transform.position, coneRange, enemyLayer);

        foreach (Collider hit in hits)
        {
            // 부채꼴 각도 체크
            Vector3 dir = (hit.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dir);
            if (angle > coneAngle * 0.5f) continue;

            EnemyHealth eh = hit.GetComponent<EnemyHealth>();
            if (eh != null)
            {
                eh.TakeDamage((int)totalDamage);
                Debug.Log($"포자 확산 적중! 데미지: {totalDamage}");
            }

            EnemyStatus status = hit.GetComponent<EnemyStatus>();
            if (status != null)
                status.ApplySlow(slowAmount, slowDuration);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, coneRange);
    }
}
