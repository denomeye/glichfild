using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CenterureichiR : SkillBase
{
    [Header("스킬 수치")]
    public int skillLevel = 1;

    private float[] atkBonus = { 15f, 15f };
    private float moveBonus = 15f;
    private float duration = 8f;
    private float[] cooldowns = { 110f, 90f };
    private float manaCostFixed = 0f;

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
        int idx = Mathf.Clamp(skillLevel - 1, 0, 1);
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

    IEnumerator ActiveRoutine()
    {
        IsActive = true;

        int idx = Mathf.Clamp(skillLevel - 1, 0, 1);

        originalAtk = stats != null ? stats.attackPower : 62f;
        originalSpeed = navAgent != null ? navAgent.speed : 5f;

        if (stats != null)
            stats.attackPower = originalAtk * (1f + atkBonus[idx] / 100f);
        if (navAgent != null)
            navAgent.speed = originalSpeed * (1f + moveBonus / 100f);

        Debug.Log("화력 전개 발동!");

        yield return new WaitForSeconds(duration);

        if (stats != null) stats.attackPower = originalAtk;
        if (navAgent != null) navAgent.speed = originalSpeed;

        IsActive = false;
        activeCoroutine = null;

        Debug.Log("화력 전개 종료");
    }
}
