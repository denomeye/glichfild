using UnityEngine;

public class ZhongliPassive : MonoBehaviour
{
    [Header("패시브 수치")]
    public float defPerLostHpRatio = 0.007f; // 잃은 체력 2%당 0.7%
    public float maxBonusRatio = 0.35f;  // 상한 35%

    private PlayerStats stats;
    private float baseDefense = 45f;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
        baseDefense = stats != null ? stats.defense : 45f;
    }

    void Update()
    {
        if (stats == null) return;

        float lostHpRatio = 1f - (stats.currentHp / stats.maxHp);
        float bonusRatio = Mathf.Min(
            lostHpRatio * defPerLostHpRatio * 50f,
            maxBonusRatio);

        stats.defense = baseDefense * (1f + bonusRatio);
    }
}