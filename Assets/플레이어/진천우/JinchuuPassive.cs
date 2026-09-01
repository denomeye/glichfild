using UnityEngine;

public class JinchuuPassive : MonoBehaviour
{
    [Header("패시브 수치")]
    public int maxStacks = 8;
    public float stackDuration = 5f;
    public float atkPerStack = 1.5f;  // 공격력 +1.5
    public float atkSpdPerStack = 0.025f; // 공속 +2.5%

    private int currentStacks = 0;
    private float lastStackTime = -999f;

    private PlayerStats stats;
    private PlayerAttack playerAttack;

    public int CurrentStacks => currentStacks;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
        playerAttack = GetComponent<PlayerAttack>();
    }

    void Update()
    {
        // 5초 지나면 스택 초기화
        if (currentStacks > 0 &&
            Time.time - lastStackTime > stackDuration)
        {
            RemoveStacks();
        }
    }

    // PlayerAttack에서 평타 적중 시 호출
    public void OnHit()
    {
        if (currentStacks >= maxStacks) return;

        currentStacks++;
        lastStackTime = Time.time;
        ApplyStackBonus();

        Debug.Log($"바람의 흔적 {currentStacks}중첩!");
    }

    void ApplyStackBonus()
    {
        if (stats == null || playerAttack == null) return;

        stats.attackPower = stats.baseAttackPower
            + atkPerStack * currentStacks;
        playerAttack.attackCooldown = stats.baseAttackCooldown
            / (1f + atkSpdPerStack * currentStacks);
    }

    void RemoveStacks()
    {
        currentStacks = 0;

        if (stats != null)
            stats.attackPower = stats.baseAttackPower;
        if (playerAttack != null)
            playerAttack.attackCooldown = stats.baseAttackCooldown;

        Debug.Log("바람의 흔적 스택 초기화");
    }
}