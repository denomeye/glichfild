using UnityEngine;

public class JinchuuE : SkillBase
{
    [Header("스킬 수치")]
    public int skillLevel = 1;
    public LayerMask enemyLayer;

    private float[] damages = { 40f, 70f, 100f };
    private float[] cooldowns = { 5f, 5f, 5f };
    private float[] manaCosts = { 60f, 70f, 80f };

    private float extraRange = 0.5f; // 사거리 +50
    private float airbornTime = 0.5f;

    private PlayerStats stats;
    private PlayerAttack playerAttack;
    private JinchuuPassive passive;

    // 다음 평타 강화 상태
    private bool isEmpowered = false;

    public bool IsEmpowered => isEmpowered;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
        playerAttack = GetComponent<PlayerAttack>();
        passive = GetComponent<JinchuuPassive>();
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
        isEmpowered = true;
        Debug.Log("귀궁우 준비! 다음 평타 강화 예정");
    }

    // PlayerAttack에서 호출
    public void OnEmpoweredHit(GameObject target)
    {
        if (!isEmpowered) return;
        isEmpowered = false;

        int idx = Mathf.Clamp(skillLevel - 1, 0, 2);
        float atkPower = stats != null ? stats.attackPower : 58f;
        float totalDamage = damages[idx] + atkPower * 0.7f;

        EnemyHealth eh = target.GetComponent<EnemyHealth>();
        if (eh != null)
        {
            eh.TakeDamage((int)totalDamage);
            Debug.Log($"귀궁우 적중! 데미지: {totalDamage}");
        }

        // 에어본 적용
        EnemyStatus status = target.GetComponent<EnemyStatus>();
        if (status != null)
            status.ApplyStun(airbornTime);

        // 명중 시 패시브 스택 적립
        if (passive != null) passive.OnHit();
    }
}
