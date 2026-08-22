using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class KakaruE : SkillBase
{
    [Header("스킬 수치")]
    public int skillLevel = 1;
    public float range = 3.5f;
    public LayerMask enemyLayer;

    private float[] damages = { 55f, 85f, 115f, 145f };
    private float[] cooldowns = { 12f, 11f, 10f, 9f };
    private float[] manaCosts = { 60f, 70f, 80f, 90f };

    private float buffMoveSpeed = 0.30f;
    private float buffAttackSpeed = 0.50f;
    private float buffDuration = 3f;

    private PlayerStats stats;
    private PlayerAttack playerAttack;
    private NavMeshAgent navAgent;

    private float originalMoveSpeed;
    private float originalAttackCooldown;
    private bool isBuffActive = false;
    private Coroutine buffCoroutine;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
        playerAttack = GetComponent<PlayerAttack>();
        navAgent = GetComponent<NavMeshAgent>();
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
        int idx = Mathf.Clamp(skillLevel - 1, 0, 3);
        float attackPower = stats != null ? stats.attackPower : 64f;
        float totalDamage = damages[idx] + attackPower * 0.5f;

        // R 활성화 중 방어 관통 보정
        KakaruR r = GetComponent<KakaruR>();
        if (r != null && r.IsActive)
            totalDamage *= (1f + r.DamageBonus);

        Collider[] hits = Physics.OverlapSphere(
            transform.position, range, enemyLayer);

        bool hitAny = false;

        foreach (Collider hit in hits)
        {
            EnemyHealth eh = hit.GetComponent<EnemyHealth>();
            if (eh != null)
            {
                eh.TakeDamage((int)totalDamage);
                Debug.Log($"필수의 수단 적중! 피해: {totalDamage}");
                hitAny = true;
            }

            EnemyStatus status = hit.GetComponent<EnemyStatus>();
            if (status != null)
                status.ApplySlow(0.15f, 3f);
        }

        if (hitAny)
        {
            if (buffCoroutine != null)
                StopCoroutine(buffCoroutine);
            buffCoroutine = StartCoroutine(ApplyBuff());
        }
    }

    IEnumerator ApplyBuff()
    {
        if (!isBuffActive)
        {
            originalMoveSpeed = navAgent != null ? navAgent.speed : 5f;
            originalAttackCooldown = playerAttack != null ? playerAttack.attackCooldown : 0.5f;
        }

        isBuffActive = true;

        if (navAgent != null) navAgent.speed = originalMoveSpeed * (1f + buffMoveSpeed);
        if (playerAttack != null) playerAttack.attackCooldown = originalAttackCooldown / (1f + buffAttackSpeed);

        Debug.Log($"폭주 버프 시작! 이속 +{buffMoveSpeed * 100}% / 공속 +{buffAttackSpeed * 100}%");

        yield return new WaitForSeconds(buffDuration);

        if (navAgent != null) navAgent.speed = originalMoveSpeed;
        if (playerAttack != null) playerAttack.attackCooldown = originalAttackCooldown;

        isBuffActive = false;
        buffCoroutine = null;

        Debug.Log("폭주 버프 종료, 원래 값 복구");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}