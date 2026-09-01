using UnityEngine;
using System.Collections;

public class PerfumerR : SkillBase
{
    [Header("스킬 수치")]
    public int skillLevel = 1;

    private float[] cooldowns = { 120f, 100f };
    private float manaCostFixed = 130f;

    private float duration = 10f;
    private float baseHeal = 35f;
    private float apRatio = 0.15f;

    public bool IsActive { get; private set; }

    private PlayerStats stats;
    private PlayerStats targetStats;

    private Coroutine activeCoroutine;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
        targetStats = FindObjectOfType<PlayerStats>();
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
        if (IsActive) return;

        if (activeCoroutine != null)
            StopCoroutine(activeCoroutine);

        activeCoroutine = StartCoroutine(BlendingRoutine());
    }

    IEnumerator BlendingRoutine()
    {
        IsActive = true;
        Debug.Log("블랜딩 발동! 지속 힐 시작");

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // 초당 힐 적용
            if (elapsed % 1f < Time.deltaTime)
            {
                float spellPower = stats != null ? stats.spellPower : 45f;
                float healAmount = baseHeal + spellPower * apRatio;

                if (targetStats != null)
                    targetStats.Heal(healAmount);

                Debug.Log($"블랜딩 힐: {healAmount}");
            }

            yield return null;
        }

        // 종료 시 잃은 체력 10% 회복
        if (targetStats != null)
        {
            float lostHp = targetStats.maxHp - targetStats.currentHp;
            float finalHeal = lostHp * 0.10f;
            targetStats.Heal(finalHeal);
            Debug.Log($"블랜딩 종료 회복: {finalHeal}");
        }

        IsActive = false;
        activeCoroutine = null;

        Debug.Log("블랜딩 종료");
    }
}
