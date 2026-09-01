using UnityEngine;
using System.Collections;

public class AmiyaR : SkillBase
{
    public int skillLevel = 1;
    public LayerMask enemyLayer;

    private float[] cooldowns = { 80f, 70f };
    private float[] lostHpRatios = { 0.08f, 0.10f };
    private float apRatio = 0.6f;
    private float range = 2.5f;
    private float manaCostFixed = 100f;

    // 처치 후 재발동 가능 시간
    private float reactivateWindow = 1.5f;
    private float lastKillTime = -999f;
    private bool canReactivate = false;

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
        // 재발동 조건 체크
        if (canReactivate &&
            Time.time - lastKillTime <= reactivateWindow)
        {
            StartCoroutine(BlastRoutine(free: true));
            canReactivate = false;
            return;
        }

        StartCoroutine(BlastRoutine(free: false));
    }

    IEnumerator BlastRoutine(bool free)
    {
        yield return new WaitForSeconds(0.3f);

        int idx = Mathf.Clamp(skillLevel - 1, 0, 1);
        float spellPower = stats != null ? stats.spellPower : 55f;

        Collider[] hits = Physics.OverlapSphere(
            transform.position, range, enemyLayer);

        bool killedAny = false;

        foreach (Collider hit in hits)
        {
            EnemyHealth eh = hit.GetComponent<EnemyHealth>();
            if (eh == null) continue;

            // 잃은 체력 비례 데미지
            float lostHp = eh.maxHp - eh.currentHp;
            float totalDmg = lostHp * lostHpRatios[idx]
                           + spellPower * apRatio;

            eh.TakeDamage((int)totalDmg);
            Debug.Log($"스컬 슈레딩 적중! 데미지: {totalDmg}");

            if (eh.currentHp <= 0)
            {
                killedAny = true;
                lastKillTime = Time.time;
                Debug.Log("스컬 슈레딩 처치! 재발동 가능");
            }
        }

        // 처치 시 재발동 활성화 (마나 무료)
        if (killedAny)
        {
            canReactivate = true;
            // 쿨타임 초기화해서 즉시 재사용 가능하게
            lastUsedTime = -999f;
        }
    }
}
