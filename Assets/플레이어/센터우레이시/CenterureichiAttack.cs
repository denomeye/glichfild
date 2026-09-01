using UnityEngine;

public class CenterureichiAttack : RangedAttackController
{
    [Header("접대 설정")]
    public int stacksForService = 5;
    public float serviceBaseDmg = 30f;  // Lv1 기준
    public float serviceAdRatio = 0.20f;

    private int hitStack = 0;
    private PlayerStats stats;
    private CenterureichiR centerR;

    public bool ServiceReady => hitStack >= stacksForService;

    protected override void Start()
    {
        base.Start();
        stats = GetComponent<PlayerStats>();
        centerR = GetComponent<CenterureichiR>();
    }

    protected override void Fire(GameObject target)
    {
        base.Fire(target);

        // W 강화 평타 체크
        CenterureichiW w = GetComponent<CenterureichiW>();
        if (w != null) w.TryApply(target);

        OnHitLogic(target);

    }

    // Q에서도 호출
    public void OnHitLogic(GameObject target)
    {
        hitStack++;

        bool rActive = centerR != null && centerR.IsActive;

        if (hitStack >= stacksForService || rActive)
        {
            if (!rActive) hitStack = 0;
            ApplyService(target);
        }
    }

    void ApplyService(GameObject target)
    {
        if (target == null) return;

        float atkPower = stats != null ? stats.attackPower : 62f;
        float totalFixed = serviceBaseDmg + atkPower * serviceAdRatio;

        EnemyHealth eh = target.GetComponent<EnemyHealth>();
        if (eh != null)
        {
            eh.TakeDamage((int)totalFixed);
            Debug.Log($"접대! 고정 피해: {totalFixed}");
        }
    }
}