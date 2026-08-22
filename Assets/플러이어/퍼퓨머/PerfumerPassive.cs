using UnityEngine;

public class PerfumerPassive : MonoBehaviour
{
    [Header("패시브 수치")]
    public float healPerSecond = 35f;    // 초당 힐
    public float range = 6f;     // 반경 600
    public float manaChargeRate = 0.02f; // 마나 2% 충전

    private PlayerStats playerStats;
    private float healTimer = 0f;

    void Start()
    {
        playerStats = FindObjectOfType<PlayerStats>();
    }

    void Update()
    {
        if (playerStats == null) return;

        healTimer += Time.deltaTime;

        if (healTimer >= 1f)
        {
            healTimer = 0f;

            // 범위 내 플레이어 힐
            float dist = Vector3.Distance(
                transform.position, playerStats.transform.position);

            if (dist <= range)
            {
                playerStats.Heal(healPerSecond);

                // 플레이어가 공격 중이면 마나 충전
                if (playerStats.isAttacking)
                    playerStats.ChargeMana(
                        playerStats.maxMana * manaChargeRate);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}