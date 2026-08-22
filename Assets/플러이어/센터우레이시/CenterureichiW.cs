using UnityEngine;

public class CenterureichiW : SkillBase
{
    [Header("스킬 수치")]
    public int skillLevel = 1;
    public LayerMask enemyLayer;

    private float[] damages = { 70f, 110f, 150f, 190f };
    private float[] cooldowns = { 9f, 8f, 7f, 6f };
    private float[] manaCosts = { 0f, 0f, 0f, 0f };
    private float adRatio = 0.8f;
    private float slowAmount = 0.35f;
    private float slowDuration = 1f;

    private PlayerStats stats;
    private CenterureichiAttack attack;
    private bool isEmpowered = false;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
        attack = GetComponent<CenterureichiAttack>();
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
        isEmpowered = true;
        Debug.Log("주케로 스페셜 준비! 다음 평타 강화");
    }

    // CenterureichiAttack.Fire()에서 호출
    public void TryApply(GameObject target)
    {
        if (!isEmpowered) return;
        isEmpowered = false;

        int idx = Mathf.Clamp(skillLevel - 1, 0, 3);
        float atkPower = stats != null ? stats.attackPower : 62f;
        float totalDamage = damages[idx] + atkPower * adRatio;

        EnemyHealth eh = target.GetComponent<EnemyHealth>();
        if (eh != null)
        {
            eh.TakeDamage((int)totalDamage);
            Debug.Log($"주케로 스페셜 적중! 피해: {totalDamage}");
        }

        EnemyStatus status = target.GetComponent<EnemyStatus>();
        if (status != null)
            status.ApplySlow(slowAmount, slowDuration);

        if (attack != null) attack.OnHitLogic(target);
    }
}