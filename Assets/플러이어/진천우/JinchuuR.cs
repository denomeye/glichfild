using UnityEngine;
using System.Collections;

public class JinchuuR : SkillBase
{
    [Header("스킬 수치")]
    public int skillLevel = 1;
    public LayerMask enemyLayer;

    private int[] attackCounts = { 6, 6, 9 };   // 5/7/9레벨
    private float[] damages = { 50f, 80f, 110f };
    private float[] cooldowns = { 100f, 85f, 70f };
    private float adRatio = 0.4f;
    private float singleBonus = 1.15f;  // 단일 대상 15% 추가
    private float cooldownRefund = 0.30f; // 처치 관여 시 30% 반환

    private float duration = 2f;     // 환영 상태 지속
    private float searchRange = 6f;     // 타겟 탐색 범위

    private PlayerStats stats;
    private bool isActive = false;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
        ApplyLevel();
    }

    public void ApplyLevel()
    {
        int idx = Mathf.Clamp(skillLevel - 1, 0, 2);
        cooldown = cooldowns[idx];
        manaCost = 100f;
    }

    protected override void OnUse()
    {
        if (isActive) return;
        StartCoroutine(PhantomRoutine());
    }

    IEnumerator PhantomRoutine()
    {
        isActive = true;
        Debug.Log("예풍상 발동! 환영 상태 시작");

        int idx = Mathf.Clamp(skillLevel - 1, 0, 2);
        float atkPower = stats != null ? stats.attackPower : 58f;
        float baseDmg = damages[idx] + atkPower * adRatio;
        int count = attackCounts[idx];

        // 범위 내 적 탐색
        Collider[] hits = Physics.OverlapSphere(
            transform.position, searchRange, enemyLayer);

        // 단일 대상 여부 확인
        bool isSingle = hits.Length == 1;
        if (isSingle) baseDmg *= singleBonus;

        float interval = duration / count;
        bool gotKill = false;

        for (int i = 0; i < count; i++)
        {
            // 살아있는 적 중 랜덤 타겟
            hits = Physics.OverlapSphere(
                transform.position, searchRange, enemyLayer);

            if (hits.Length == 0) break;

            Collider target = hits[Random.Range(0, hits.Length)];
            EnemyHealth eh = target.GetComponent<EnemyHealth>();

            if (eh != null)
            {
                eh.TakeDamage((int)baseDmg);
                Debug.Log($"예풍상 {i + 1}타! 피해: {baseDmg}");

                // 처치 확인
                if (eh.currentHp <= 0)
                    gotKill = true;
            }

            yield return new WaitForSeconds(interval);
        }

        // 처치 관여 시 쿨타임 30% 반환
        if (gotKill)
        {
            float refund = cooldown * cooldownRefund;
            lastUsedTime -= refund;
            Debug.Log($"처치 관여! 쿨타임 {refund}초 반환");
        }

        isActive = false;
        Debug.Log("예풍상 종료");
    }
}