using UnityEngine;

public class ZhongliQ : SkillBase
{
    [Header("스킬 수치")]
    public int skillLevel = 1;
    public float projectileSpeed = 15f;
    public LayerMask enemyLayer;

    private float[] damages = { 80f, 120f, 160f };
    private float[] cooldowns = { 6f, 6f, 5f };
    private float[] manaCosts = { 60f, 70f, 80f };
    private float apRatio = 0.4f;
    private float slowAmount = 0.35f;
    private float slowDuration = 2f;

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
        // 전방으로 투사체 발사
        StartCoroutine(FireProjectile());
    }

    System.Collections.IEnumerator FireProjectile()
    {
        int idx = Mathf.Clamp(skillLevel - 1, 0, 2);
        float spellPower = stats != null ? stats.spellPower : 40f;
        float totalDmg = damages[idx] + spellPower * apRatio;

        Vector3 pos = transform.position;
        Vector3 dir = transform.forward;
        float maxRange = 8f;
        float traveled = 0f;

        while (traveled < maxRange)
        {
            float step = projectileSpeed * Time.deltaTime;
            pos += dir * step;
            traveled += step;

            // 적 감지
            Collider[] hits = Physics.OverlapSphere(
                pos, 0.4f, enemyLayer);

            foreach (Collider hit in hits)
            {
                EnemyHealth eh = hit.GetComponent<EnemyHealth>();
                if (eh != null)
                {
                    eh.TakeDamage((int)totalDmg);
                    Debug.Log($"암력 투창 적중! 데미지: {totalDmg}");
                }

                EnemyStatus status = hit.GetComponent<EnemyStatus>();
                if (status != null)
                    status.ApplySlow(slowAmount, slowDuration);

                yield break; // 첫 적중 후 종료
            }

            yield return null;
        }
    }
}
