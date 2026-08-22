using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHp = 100f;
    public float currentHp;          // 추가

    void Start()
    {
        currentHp = maxHp;
    }

    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        Debug.Log($"적 피해: {damage} / 남은 체력: {currentHp}");

        if (currentHp <= 0)
        {
            Debug.Log("적 사망");
            Destroy(gameObject);
        }
    }
}