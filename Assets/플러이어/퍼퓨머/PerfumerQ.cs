using UnityEngine;

public class PerfumerQ : SkillBase
{
    [Header("스킬 수치")]
    public int skillLevel = 1;

    private float[] heals = { 80f, 120f, 160f, 200f };
    private float[] cooldowns = { 5f, 5f, 4f, 4f };
    private float[] manaCosts = { 100f, 100f, 100f, 100f };
    private float apRatio = 0.6f;

    private PlayerStats playerStats;  // 퍼퓨머 주문력
    private PlayerStats targetStats;  // 힐 받을 대상

    void Start()
    {
        // 퍼퓨머 본인 스탯
        playerStats = GetComponent<PlayerStats>();

        // 힐 대상 (지금은 플레이어 1명)
        targetStats = FindObjectOfType<PlayerStats>();

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
        if (targetStats == null) return;

        int idx = Mathf.Clamp(skillLevel - 1, 0, 3);
        float spellPower = playerStats != null ? playerStats.spellPower : 45f;
        float totalHeal = heals[idx] + spellPower * apRatio;

        targetStats.Heal(totalHeal);
        Debug.Log($"아로마 치료! 회복량: {totalHeal}");
    }
}