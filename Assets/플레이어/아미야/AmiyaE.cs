using UnityEngine;
using System.Collections;

public class AmiyaE : SkillBase
{
    public int skillLevel = 1;
    public LayerMask enemyLayer;

    private float[] cooldowns = { 10f, 9f, 8f };
    private float[] manaCosts = { 80f, 90f, 100f };
    private float apRatio = 0.25f;
    private int totalShots = 10;
    private float shotInterval = 0.1f;
    private float range = 8f;
    private float width = 0.4f;

    // 다중 타겟 Fall-off: 추가 대상마다 피해 20% 감소
    private float falloffRatio = 0.8f;

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
        StartCoroutine(BurstRoutine());
    }

    IEnumerator BurstRoutine()
    {
        float spellPower = stats != null ? stats.spellPower : 55f;
        float baseDmg = spellPower * apRatio;
        int hitCount = 0;

        for (int i = 0; i < totalShots; i++)
        {
            // 일직선 캡슐 감지
            RaycastHit[] hits = Physics.CapsuleCastAll(
                transform.position,
                transform.position + transform.forward * range,
                width,
                transform.forward,
                0f,
                enemyLayer);

            float dmg = baseDmg;
            int hitIdx = 0;

            foreach (RaycastHit hit in hits)
            {
                // Fall-off 적용
                float finalDmg = dmg * Mathf.Pow(falloffRatio, hitIdx);
                EnemyHealth eh = hit.collider.GetComponent<EnemyHealth>();
                if (eh != null)
                {
                    eh.TakeDamage((int)finalDmg);
                    hitCount++;
                    hitIdx++;
                }
            }

            yield return new WaitForSeconds(shotInterval);
        }

        // 5발 이상 명중 시 쿨타임 20% 반환
        if (hitCount >= 5)
        {
            float refund = cooldown * 0.2f;
            lastUsedTime -= refund;
            Debug.Log($"스피릿 버스트 5발 이상 적중! 쿨타임 {refund}초 반환");
        }
    }
}