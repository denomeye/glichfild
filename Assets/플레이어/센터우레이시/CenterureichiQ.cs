using UnityEngine;

public class CenterureichiQ : SkillBase
{
    [Header("스킬 수치")]
    public int skillLevel = 1;
    public LayerMask enemyLayer;

    private float[] damages = { 40f, 65f, 90f };
    private float[] cooldowns = { 6f, 6f, 6f };
    private float[] manaCosts = { 0f, 0f, 0f };
    private float adRatio = 0.6f;

    private PlayerStats stats;
    private CenterureichiAttack attack;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
        attack = GetComponent<CenterureichiAttack>();
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
        // 타깃 찾기
        Collider[] hits = Physics.OverlapSphere(
            transform.position, attack != null ? attack.attackRange : 4f,
            enemyLayer);
        if (hits.Length == 0) return;

        // 가장 가까운 적 선택
        GameObject target = hits[0].gameObject;

        int idx = Mathf.Clamp(skillLevel - 1, 0, 2);
        float atkPower = stats != null ? stats.attackPower : 62f;
        float dmgPerShot = damages[idx] + atkPower * adRatio;

        // 2발 연사
        for (int i = 0; i < 2; i++)
        {
            EnemyHealth eh = target.GetComponent<EnemyHealth>();
            if (eh != null)
            {
                eh.TakeDamage((int)dmgPerShot);
                // 기본 공격 적중 취급 - 접대 스택 적립
                if (attack != null) attack.OnHitLogic(target);
                Debug.Log($"열성적인 접대 {i + 1}타! 데미지: {dmgPerShot}");
            }
        }
    }
}
