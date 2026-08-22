using UnityEngine;

public class KakaruQ : SkillBase
{
    [Header("스킬 수치")]
    public int skillLevel = 1;
    public float range = 3f;
    public LayerMask enemyLayer;

    private float[] damage = { 65f, 100f, 135f, 170f };
    private float[] cooldowns = { 8f, 7f, 6f, 5f };
    private float[] manaCosts = { 55f, 65f, 75f, 85f };

    private PlayerStats stats;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
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
        int idx = Mathf.Clamp(skillLevel - 1, 0, 3);
        float attackPower = stats != null ? stats.attackPower : 64f;
        float totalDamage = damage[idx] + attackPower * 0.6f;

        // R 활성화 중 방어 관통 보정
        KakaruR r = GetComponent<KakaruR>();
        if (r != null && r.IsActive)
            totalDamage *= (1f + r.DamageBonus);

        Collider[] hits = Physics.OverlapSphere(
            transform.position, range, enemyLayer);

        foreach (Collider hit in hits)
        {
            EnemyHealth eh = hit.GetComponent<EnemyHealth>();
            if (eh != null)
            {
                eh.TakeDamage((int)totalDamage);
                Debug.Log($"척살 명령 적중! 피해: {totalDamage}");
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}