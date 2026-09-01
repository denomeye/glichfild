using UnityEngine;

public class CenterureichiE : SkillBase
{
    [Header("스킬 수치")]
    public int skillLevel = 1;
    public LayerMask enemyLayer;

    private float[] damages = { 60f, 100f, 140f };
    private float[] cooldowns = { 12f, 11f, 10f };
    private float[] manaCosts = { 0f, 0f, 0f };
    private float adRatio = 0.5f;
    private float knockback = 2.5f; // 250 유닛

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
        float atkPower = stats != null ? stats.attackPower : 62f;
        float totalDamage = damages[idx] + atkPower * adRatio;

        Collider[] hits = Physics.OverlapSphere(
            transform.position, 1.5f, enemyLayer);

        foreach (Collider hit in hits)
        {
            EnemyHealth eh = hit.GetComponent<EnemyHealth>();
            if (eh != null)
            {
                eh.TakeDamage((int)totalDamage);
                Debug.Log($"아크 기아스 적중! 데미지: {totalDamage}");
            }

            // 밀쳐내기
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 dir = (hit.transform.position
                    - transform.position).normalized;
                rb.AddForce(dir * knockback, ForceMode.Impulse);
            }
        }
    }
}
