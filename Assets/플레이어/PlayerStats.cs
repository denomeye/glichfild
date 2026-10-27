using UnityEngine;
using UnityEngine.AI;

// Lv.1 값과 레벨당 성장을 한 쌍으로 보관 (캐릭터 시트의 "Lv.1 / 성장" 표와 1:1 대응)
[System.Serializable]
public struct StatGrowth
{
    public float baseValue; // Lv.1 값
    public float growth;    // 레벨당 성장 (공격 속도는 비율: 0.035 = 3.5%)

    public StatGrowth(float baseValue, float growth)
    {
        this.baseValue = baseValue;
        this.growth = growth;
    }

    // 가산 성장: Lv.N = Lv.1 + 성장 x (N-1)
    public float Additive(int level) => baseValue + growth * (level - 1);

    // 비율 성장 (공격 속도): Lv.N = Lv.1 x (1 + 성장% x (N-1))
    public float Percent(int level) => baseValue * (1f + growth * (level - 1));
}

public class PlayerStats : MonoBehaviour
{
    public const int MaxLevel = 10;
    const float WorldUnit = 100f; // 시트 단위 -> 월드 단위 (이동속도 340 -> 3.4, 사거리 175 -> 1.75)

    [Header("레벨")]
    [Range(1, MaxLevel)] public int level = 1;

    [Header("기초 스탯 (Lv.1 값 / 레벨당 성장) - 기본값은 카카루 알파 수치")]
    public StatGrowth hpStat = new StatGrowth(620f, 88f);
    public StatGrowth hpRegenStat = new StatGrowth(8f, 0.8f);
    public StatGrowth attackStat = new StatGrowth(64f, 4.5f);
    public StatGrowth defenseStat = new StatGrowth(34f, 3f);
    public StatGrowth magicResistStat = new StatGrowth(30f, 2.5f);
    public StatGrowth attackSpeedStat = new StatGrowth(0.7f, 0.035f); // 성장은 비율
    public StatGrowth rangeStat = new StatGrowth(175f, 0f);
    public StatGrowth moveSpeedStat = new StatGrowth(340f, 0f);

    [Header("마나 (마나를 쓰지 않는 캐릭터는 체크 해제)")]
    public bool useMana = true;
    public StatGrowth manaStat = new StatGrowth(400f, 58f);
    public StatGrowth manaRegenStat = new StatGrowth(3.6f, 0.4f);

    [Header("탄창 (탄창을 쓰는 캐릭터만, 0이면 RangedAttackController의 값을 그대로 사용)")]
    public int magazineSize = 0;

    [Header("실제 동작에 연결 (끄면 해당 컴포넌트에 직접 넣은 값을 그대로 사용)")]
    public bool applyMoveSpeed = true;   // NavMeshAgent 이동속도
    public bool applyAttackRange = true; // PlayerController / 원거리 공격 사거리
    public bool applyAttackSpeed = true; // PlayerAttack 공격 쿨타임 (= 1 / 공격속도)

    [Header("현재 값 (실행 시 자동 계산, 직접 입력 불필요)")]
    public float maxHp;
    public float currentHp;
    public float hpRegen;     // 초당 회복량
    public float maxMana;
    public float currentMana;
    public float manaRegen;   // 초당 회복량
    public float attackPower;
    public float defense;
    public float magicResist;

    [Header("주문력 (기초 스탯 아님, 아이템 등으로만 증가)")]
    public float spellPower;

    [Header("상태")]
    public bool isAttacking;

    // 레벨 기준 기본값 (버프/아이템이 붙기 전 값)
    public float baseAttackPower => attackStat.Additive(level);
    public float baseDefense => defenseStat.Additive(level);
    public float baseMagicResist => magicResistStat.Additive(level);

    // 공격 속도 / 사거리 / 이동속도 (시트 단위 값)
    public float attackSpeed => attackSpeedStat.Percent(level);
    public float baseAttackCooldown => 1f / Mathf.Max(attackSpeed, 0.01f);
    public float attackRange => rangeStat.Additive(level);
    public float moveSpeed => moveSpeedStat.Additive(level);

    void Awake()
    {
        // 다른 스크립트의 Start()보다 먼저 값이 채워지도록 Awake에서 초기화
        level = Mathf.Clamp(level, 1, MaxLevel);

        maxHp = hpStat.Additive(level);
        currentHp = maxHp;
        hpRegen = hpRegenStat.Additive(level);

        maxMana = useMana ? manaStat.Additive(level) : 0f;
        currentMana = maxMana;
        manaRegen = useMana ? manaRegenStat.Additive(level) : 0f;

        attackPower = baseAttackPower;
        defense = baseDefense;
        magicResist = baseMagicResist;

        ApplyToComponents();
    }

    void Update()
    {
        if (currentHp > 0f && currentHp < maxHp)
            currentHp = Mathf.Min(maxHp, currentHp + hpRegen * Time.deltaTime);

        if (useMana && currentMana < maxMana)
            currentMana = Mathf.Min(maxMana, currentMana + manaRegen * Time.deltaTime);
    }

    // 이동속도, 사거리, 공격속도를 실제로 쓰는 컴포넌트에 넣어 줌
    void ApplyToComponents()
    {
        if (applyMoveSpeed)
        {
            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            if (agent != null) agent.speed = moveSpeed / WorldUnit;
        }

        RangedAttackController ranged = GetComponent<RangedAttackController>();

        if (applyAttackRange)
        {
            float range = attackRange / WorldUnit;

            PlayerController controller = GetComponent<PlayerController>();
            if (controller != null) controller.attackRange = range;
            if (ranged != null) ranged.attackRange = range;
        }

        if (applyAttackSpeed)
        {
            PlayerAttack playerAttack = GetComponent<PlayerAttack>();
            if (playerAttack != null) playerAttack.attackCooldown = baseAttackCooldown;
            if (ranged != null) ranged.attackCooldown = baseAttackCooldown;
        }

        // 탄창 크기 (RangedAttackController.Start에서 currentAmmo를 채우므로 Awake 단계에서 넣어야 함)
        if (ranged != null && magazineSize > 0)
            ranged.maxAmmo = magazineSize;
    }

    // 레벨 변경 (레벨업 시스템에서 호출). 버프/아이템으로 붙은 추가분은 유지하고 증가분만 더함
    public void SetLevel(int newLevel)
    {
        newLevel = Mathf.Clamp(newLevel, 1, MaxLevel);
        if (newLevel == level) return;

        float hpGain = hpStat.Additive(newLevel) - hpStat.Additive(level);
        float manaGain = useMana ? manaStat.Additive(newLevel) - manaStat.Additive(level) : 0f;
        float atkGain = attackStat.Additive(newLevel) - attackStat.Additive(level);
        float defGain = defenseStat.Additive(newLevel) - defenseStat.Additive(level);
        float mrGain = magicResistStat.Additive(newLevel) - magicResistStat.Additive(level);

        level = newLevel;

        maxHp += hpGain;
        currentHp += hpGain;
        hpRegen = hpRegenStat.Additive(level);

        maxMana += manaGain;
        currentMana += manaGain;
        manaRegen = useMana ? manaRegenStat.Additive(level) : 0f;

        attackPower += atkGain;
        defense += defGain;
        magicResist += mrGain;

        ApplyToComponents();
    }

    public void Heal(float amount)
    {
        currentHp = Mathf.Min(maxHp, currentHp + amount);
    }

    public void ChargeMana(float amount)
    {
        if (!useMana) return;
        currentMana = Mathf.Min(maxMana, currentMana + amount);
    }
}
