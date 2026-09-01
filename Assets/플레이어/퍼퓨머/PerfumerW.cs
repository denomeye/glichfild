using UnityEngine;
using System.Collections;

public class PerfumerW : SkillBase
{
    [Header("스킬 수치")]
    public int skillLevel = 1;

    private float[] tenacities = { 30f, 35f, 40f }; // 강인함 %
    private float[] cooldowns = { 18f, 16f, 14f };
    private float[] manaCosts = { 110f, 110f, 110f };

    private float duration = 3f;
    private float auraRange = 5f; // 반경 500

    // 강인함 오라 활성화 정보 (EnemyStatus에서 참조)
    public bool IsActive { get; private set; }
    public float Tenacity { get; private set; }

    void Start()
    {
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
        StartCoroutine(TenacityRoutine());
    }

    IEnumerator TenacityRoutine()
    {
        int idx = Mathf.Clamp(skillLevel - 1, 0, 2);
        Tenacity = tenacities[idx];
        IsActive = true;

        Debug.Log($"에게의 키스! 강인함 +{Tenacity}% {duration}초");

        yield return new WaitForSeconds(duration);

        IsActive = false;
        Tenacity = 0f;

        Debug.Log("에게의 키스 종료");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, auraRange);
    }
}
