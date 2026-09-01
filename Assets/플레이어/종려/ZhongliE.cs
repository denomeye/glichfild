using UnityEngine;
using System.Collections;

public class ZhongliE : SkillBase
{
    [Header("스킬 수치")]
    public int skillLevel = 1;
    public LayerMask enemyLayer;
    public GameObject pillarPrefab; // 암주 프리팹

    private float[] damages = { 60f, 95f, 130f };
    private float[] cooldowns = { 10f, 9f, 8f };
    private float[] manaCosts = { 80f, 90f, 100f };
    private float apRatio = 0.35f;
    private float knockback = 2.5f;
    private float spawnRange = 1.5f; // 소환 거리
    private float pillarLife = 2f;   // 암주 지속 2초

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
        float spellPower = stats != null ? stats.spellPower : 40f;
        float totalDmg = damages[idx] + spellPower * apRatio;

        // 전방에 암주 소환
        Vector3 spawnPos = transform.position
            + transform.forward * spawnRange;

        // 암주 프리팹 생성
        if (pillarPrefab != null)
        {
            GameObject pillar = Instantiate(
                pillarPrefab, spawnPos, Quaternion.identity);
            Destroy(pillar, pillarLife);
        }

        // 소환 시 주변 적 피해 + 넉백
        Collider[] hits = Physics.OverlapSphere(
            spawnPos, 0.8f, enemyLayer);

        foreach (Collider hit in hits)
        {
            EnemyHealth eh = hit.GetComponent<EnemyHealth>();
            if (eh != null)
            {
                eh.TakeDamage((int)totalDmg);
                Debug.Log($"지핵 적중! 데미지: {totalDmg}");
            }

            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 dir = (hit.transform.position
                    - spawnPos).normalized;
                rb.AddForce(dir * knockback, ForceMode.Impulse);
            }
        }
    }
}
