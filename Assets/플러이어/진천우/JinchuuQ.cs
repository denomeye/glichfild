using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class JinchuuQ : SkillBase
{
    [Header("스킬 수치")]
    public int skillLevel = 1;

    private float[] atkBonus = { 15f, 20f, 25f, 30f }; // 공격력 %
    private float moveBonus = 20f;                      // 이속 +20%
    private float duration = 3f;
    private float[] cooldowns = { 10f, 9f, 8f, 7f };
    private float[] manaCosts = { 65f, 75f, 85f, 95f };

    private PlayerStats stats;
    private NavMeshAgent navAgent;

    private float originalAtk;
    private float originalSpeed;
    private bool isActive = false;
    private Coroutine activeCoroutine;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
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
        if (isActive) return;

        if (activeCoroutine != null)
            StopCoroutine(activeCoroutine);

        activeCoroutine = StartCoroutine(BuffRoutine());
    }

    IEnumerator BuffRoutine()
    {
        isActive = true;

        int idx = Mathf.Clamp(skillLevel - 1, 0, 3);

        originalAtk = stats != null ? stats.attackPower : 58f;
        originalSpeed = navAgent != null ? navAgent.speed : 5f;

        if (stats != null)
            stats.attackPower = originalAtk * (1f + atkBonus[idx] / 100f);
        if (navAgent != null)
            navAgent.speed = originalSpeed * (1f + moveBonus / 100f);

        Debug.Log($"파비하 발동! 공격력 +{atkBonus[idx]}% / 이속 +{moveBonus}%");

        yield return new WaitForSeconds(duration);

        if (stats != null) stats.attackPower = originalAtk;
        if (navAgent != null) navAgent.speed = originalSpeed;

        isActive = false;
        activeCoroutine = null;

        Debug.Log("파비하 종료");
    }
}