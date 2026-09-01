using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AmiyaQ : SkillBase
{
    public int skillLevel = 1;

    private float[] atkSpdBonus = { 30f, 40f, 50f };
    private float[] cooldowns = { 10f, 10f, 10f };
    private float[] manaCosts = { 80f, 90f, 100f };
    private float duration = 5f;

    private NavMeshAgent navAgent;
    private PlayerAttack playerAttack;

    private float originalCooldown;
    private bool isActive = false;

    public bool IsActive => isActive;

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        playerAttack = GetComponent<PlayerAttack>();
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
        if (isActive) return;
        StartCoroutine(BuffRoutine());
    }

    IEnumerator BuffRoutine()
    {
        isActive = true;

        int idx = Mathf.Clamp(skillLevel - 1, 0, 2);
        float bonusRatio = atkSpdBonus[idx] / 100f;

        originalCooldown = playerAttack != null
            ? playerAttack.attackCooldown : 0.5f;

        if (playerAttack != null)
            playerAttack.attackCooldown =
                originalCooldown / (1f + bonusRatio);

        yield return new WaitForSeconds(duration);

        if (playerAttack != null)
            playerAttack.attackCooldown = originalCooldown;

        isActive = false;
    }
}
