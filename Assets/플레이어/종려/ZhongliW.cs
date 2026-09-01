using UnityEngine;
using System.Collections;

public class ZhongliW : SkillBase
{
    [Header("스킬 수치")]
    public int skillLevel = 1;
    public LayerMask enemyLayer;

    private float[] dmgReductions = { 20f, 25f, 30f }; // %
    private float[] expDamages = { 100f, 150f, 200f };
    private float[] cooldowns = { 12f, 11f, 10f };
    private float[] manaCosts = { 70f, 80f, 90f };
    private float apRatio = 0.6f;
    private float duration = 3f;
    private float knockbackForce = 2f;
    private float explosionRange = 3f;

    private PlayerStats stats;
    private bool isActive = false;

    public bool IsActive => isActive;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
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
        StartCoroutine(ShieldRoutine());
    }

    IEnumerator ShieldRoutine()
    {
        isActive = true;
        int idx = Mathf.Clamp(skillLevel - 1, 0, 2);

        Debug.Log($"옥홀 방패! 피해 감소 {dmgReductions[idx]}% {duration}초");

        yield return new WaitForSeconds(duration);

        // 종료 시 폭발
        Explode(idx);
        isActive = false;
    }

    void Explode(int idx)
    {
        float spellPower = stats != null ? stats.spellPower : 40f;
        float totalDamage = expDamages[idx] + spellPower * apRatio;

        Collider[] hits = Physics.OverlapSphere(
            transform.position, explosionRange, enemyLayer);

        foreach (Collider hit in hits)
        {
            EnemyHealth eh = hit.GetComponent<EnemyHealth>();
            if (eh != null)
            {
                eh.TakeDamage((int)totalDamage);
                Debug.Log($"옥홀 방패 폭발 적중! 데미지: {totalDamage}");
            }

            // 넉백
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 dir = (hit.transform.position
                    - transform.position).normalized;
                rb.AddForce(dir * knockbackForce, ForceMode.Impulse);
            }
        }
    }

    // 현재 피해 감소치 반환 (PlayerHealth에서 참조)
    public float GetDmgReduction()
    {
        if (!isActive) return 0f;
        int idx = Mathf.Clamp(skillLevel - 1, 0, 2);
        return dmgReductions[idx] / 100f;
    }

    // 쿨타임 50% 즉시 단축 (외부 호출용, 현재 R에서는 미사용)
    public void ReduceCooldownHalf()
    {
        lastUsedTime += cooldown * 0.5f;
        Debug.Log("W 쿨타임 50% 단축!");
    }
}
