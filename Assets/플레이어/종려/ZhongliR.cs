using UnityEngine;
using System.Collections;

public class ZhongliR : SkillBase
{
    [Header("스킬 수치")]
    public int skillLevel = 1;
    public LayerMask enemyLayer;

    private float[] damages = { 300f, 450f };
    private float[] cooldowns = { 110f, 90f };
    private float apRatio = 0.8f;
    private float manaCostFixed = 120f;

    // 석화 시간
    private float[] stunTimes = { 1.5f, 2f };

    // 범위
    private float[] ranges = { 5f, 5f };

    private PlayerStats stats;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
        ApplyLevel();
    }

    public void ApplyLevel()
    {
        int idx = Mathf.Clamp(skillLevel - 1, 0, 1);
        cooldown = cooldowns[idx];
        manaCost = manaCostFixed;
    }

    protected override void OnUse()
    {
        StartCoroutine(MeteorRoutine());
    }

    IEnumerator MeteorRoutine()
    {
        // 낙하 예고 딜레이
        Debug.Log("천애만상 발동! 낙하 예고 중...");
        yield return new WaitForSeconds(0.5f);

        int idx = Mathf.Clamp(skillLevel - 1, 0, 1);
        float spellPower = stats != null ? stats.spellPower : 40f;
        float totalDmg = damages[idx] + spellPower * apRatio;
        float range = ranges[idx];
        float stunTime = stunTimes[idx];

        Collider[] hits = Physics.OverlapSphere(
            transform.position, range, enemyLayer);

        foreach (Collider hit in hits)
        {
            EnemyHealth eh = hit.GetComponent<EnemyHealth>();
            if (eh != null)
            {
                eh.TakeDamage((int)totalDmg);
                Debug.Log($"천애만상 적중! 데미지: {totalDmg}");
            }

            // 석화 (스턴으로 구현)
            EnemyStatus status = hit.GetComponent<EnemyStatus>();
            if (status != null)
                status.ApplyStun(stunTime);
        }
    }
}
