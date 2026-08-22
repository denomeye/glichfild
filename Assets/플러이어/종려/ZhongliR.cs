using UnityEngine;
using System.Collections;

public class ZhongliR : SkillBase
{
    [Header("스킬 수치")]
    public int skillLevel = 1;
    public LayerMask enemyLayer;

    private float[] damages = { 300f, 450f, 600f };
    private float[] cooldowns = { 110f, 90f, 75f };
    private float apRatio = 0.8f;
    private float manaCostFixed = 120f;

    // 석화 시간
    private float[] stunTimes = { 1.5f, 2f, 2f };

    // 범위
    private float[] ranges = { 5f, 5f, 6.5f }; // 9레벨 30% 증가

    private PlayerStats stats;
    private ZhongliW zhongliW;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
        zhongliW = GetComponent<ZhongliW>();
        ApplyLevel();
    }

    public void ApplyLevel()
    {
        int idx = Mathf.Clamp(skillLevel - 1, 0, 2);
        cooldown = cooldowns[idx];
        manaCost = manaCostFixed;
    }

    protected override void OnUse()
    {
        StartCoroutine(MeteorRoutine());
    }

    IEnumerator MeteorRoutine()
    {
        // 운석 낙하 연출 딜레이
        Debug.Log("천애만상 발동! 운석 낙하 중...");
        yield return new WaitForSeconds(0.5f);

        int idx = Mathf.Clamp(skillLevel - 1, 0, 2);
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
                Debug.Log($"천애만상 적중! 피해: {totalDmg}");
            }

            // 석화 (기절로 구현)
            EnemyStatus status = hit.GetComponent<EnemyStatus>();
            if (status != null)
                status.ApplyStun(stunTime);
        }

        // 9레벨: W 쿨타임 50% 감소
        if (skillLevel >= 3 && zhongliW != null)
            zhongliW.ReduceCooldownHalf();
    }
}