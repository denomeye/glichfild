using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CenterureichiR : SkillBase
{
    [Header("스킬 수치")]
    public int skillLevel = 1;

    private float[] atkBonus = { 15f, 15f, 15f };
    private float moveBonus = 15f;
    private float duration = 8f;
    private float[] cooldowns = { 110f, 90f, 75f };
    private float manaCostFixed = 0f;

    // 처치 관여 시 2초 연장 (최대 2회)
    private int extendCount = 0;
    private int maxExtend = 2;
    private float extendSeconds = 2f;

    public bool IsActive { get; private set; }

    private PlayerStats stats;
    private NavMeshAgent navAgent;
    private CenterureichiAttack attack;

    private float originalAtk;
    private float originalSpeed;
    private Coroutine activeCoroutine;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
        navAgent = GetComponent<NavMeshAgent>();
        attack = GetComponent<CenterureichiAttack>();
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
        if (IsActive) return;

        if (activeCoroutine != null)
            StopCoroutine(activeCoroutine);

        activeCoroutine = StartCoroutine(ActiveRoutine());
    }

    // 처치 관여 시 외부에서 호출
    public void OnKillAssist()
    {
        if (!IsActive || extendCount >= maxExtend) return;
        extendCount++;
        Debug.Log($"처치 관여! {extendSeconds}초 연장 ({extendCount}/{maxExtend})");
    }

    IEnumerator ActiveRoutine()
    {
        IsActive = true;
        extendCount = 0;

        int idx = Mathf.Clamp(skillLevel - 1, 0, 2);

        originalAtk = stats != null ? stats.attackPower : 62f;
        originalSpeed = navAgent != null ? navAgent.speed : 5f;

        if (stats != null)
            stats.attackPower = originalAtk * (1f + atkBonus[idx] / 100f);
        if (navAgent != null)
            navAgent.speed = originalSpeed * (1f + moveBonus / 100f);

        Debug.Log("화력 전개 발동!");

        float elapsed = 0f;
        while (elapsed < duration + extendCount * extendSeconds)
        {
            elapsed += Time.deltaTime;
            // 연장 반영을 위해 매 프레임 종료 조건 재확인
            if (elapsed >= duration + extendCount * extendSeconds) break;
            yield return null;
        }

        if (stats != null) stats.attackPower = originalAtk;
        if (navAgent != null) navAgent.speed = originalSpeed;

        IsActive = false;
        activeCoroutine = null;

        Debug.Log("화력 전개 종료");
    }
}