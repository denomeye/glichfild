using UnityEngine;

public class AmiyaW : SkillBase
{
    public int skillLevel = 1;
    public LayerMask enemyLayer;

    private float[] damages = { 80f, 120f, 160f };
    private float[] heals = { 40f, 60f, 80f };
    private float[] cooldowns = { 8f, 7f, 6f };
    private float[] manaCosts = { 60f, 80f, 100f };
    private float apRatio = 0.5f;

    private PlayerStats stats;

    // 최근 3초 내 피격한 대상 추적
    private GameObject lastHitTarget;
    private float lastHitTime = -999f;

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

    // PlayerAttack에서 피격 시 호출
    public void RegisterHit(GameObject target)
    {
        lastHitTarget = target;
        lastHitTime = Time.time;
    }

    protected override void OnUse()
    {
        // 3초 내 피격 대상 없으면 사용 불가
        if (lastHitTarget == null ||
            Time.time - lastHitTime > 3f)
        {
            Debug.Log("감정 흡수: 유효한 대상 없음");
            // 쿨타임 소모 안 되게 복구
            lastUsedTime = -999f;
            return;
        }

        int idx = Mathf.Clamp(skillLevel - 1, 0, 2);
        float spellPower = stats != null ? stats.spellPower : 55f;
        float totalDmg = damages[idx] + spellPower * apRatio;
        float totalHeal = heals[idx];

        EnemyHealth eh = lastHitTarget.GetComponent<EnemyHealth>();
        if (eh != null)
        {
            eh.TakeDamage((int)totalDmg);
            Debug.Log($"감정 흡수 적중! 피해: {totalDmg}");
        }

        if (stats != null)
        {
            stats.currentHp = Mathf.Min(
                stats.currentHp + totalHeal, stats.maxHp);
            Debug.Log($"감정 흡수 회복: {totalHeal}");
        }
    }
}