using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class KakaruR : SkillBase
{
    [Header("스킬 수치")]
    public int skillLevel = 1;

    private float[] cooldowns = { 90f, 75f };
    private float manaCostFixed = 110f;

    // R 효과 수치
    private float duration = 10f;   // 지속시간
    private float armorPenBonus = 0.20f; // 방어 관통 보너스 20%
    private float moveSpeedBonus = 0.20f; // 이속 +20%
    private float moveSpeedTime = 1f;    // 이속 지속 1초
    private float cooldownReduction = 0.50f; // 패시브 쿨타임 50% 감소

    // 컴포넌트 캐시
    private PlayerStats stats;
    private NavMeshAgent navAgent;
    private KakaruQ kakaruQ;
    private KakaruW kakaruW;
    private KakaruE kakaruE;

    // 상태
    private bool isActive = false;
    private Coroutine activeCoroutine;

    // 원래 값 저장
    private float originalMoveSpeed;
    private float originalQCooldown;
    private float originalWCooldown;
    private float originalECooldown;

    // R 활성화 중 외부 참조용 (다른 스킬에서 사용)
    public bool IsActive => isActive;
    public float DamageBonus => armorPenBonus;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
        navAgent = GetComponent<NavMeshAgent>();
        kakaruQ = GetComponent<KakaruQ>();
        kakaruW = GetComponent<KakaruW>();
        kakaruE = GetComponent<KakaruE>();
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
        if (isActive) return;

        if (activeCoroutine != null)
            StopCoroutine(activeCoroutine);

        activeCoroutine = StartCoroutine(ActivateRoutine());
    }

    IEnumerator ActivateRoutine()
    {
        isActive = true;
        Debug.Log("환영의 각인 발동!");

        // 원래 값 저장
        originalMoveSpeed = navAgent != null ? navAgent.speed : 5f;
        originalQCooldown = kakaruQ != null ? kakaruQ.cooldown : 8f;
        originalWCooldown = kakaruW != null ? kakaruW.cooldown : 14f;
        originalECooldown = kakaruE != null ? kakaruE.cooldown : 12f;

        // 패시브 쿨타임 50% 감소 적용
        if (kakaruQ != null) kakaruQ.cooldown *= (1f - cooldownReduction);
        if (kakaruW != null) kakaruW.cooldown *= (1f - cooldownReduction);
        if (kakaruE != null) kakaruE.cooldown *= (1f - cooldownReduction);

        Debug.Log($"스킬 쿨타임 50% 감소 적용");

        // 이속 +20% 1초
        StartCoroutine(MoveSpeedRoutine());

        // 10초 대기
        yield return new WaitForSeconds(duration);

        // 종료 - 원래 값 복귀
        if (kakaruQ != null) kakaruQ.cooldown = originalQCooldown;
        if (kakaruW != null) kakaruW.cooldown = originalWCooldown;
        if (kakaruE != null) kakaruE.cooldown = originalECooldown;

        isActive = false;
        activeCoroutine = null;

        Debug.Log("환영의 각인 종료, 원래 값 복귀");
    }

    IEnumerator MoveSpeedRoutine()
    {
        if (navAgent == null) yield break;

        navAgent.speed = originalMoveSpeed * (1f + moveSpeedBonus);
        Debug.Log($"이속 +{moveSpeedBonus * 100}% 적용");

        yield return new WaitForSeconds(moveSpeedTime);

        // 이속 복귀 (단, 아직 R이 활성화 중이면 유지하지 않고 복귀)
        if (isActive)
            navAgent.speed = originalMoveSpeed;
    }
}
