using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("기본 스탯")]
    public float attackPower = 58f;
    public float baseAttackPower = 58f;
    public float baseAttackCooldown = 0.5f;
    public float spellPower = 40f;
    public float defense = 45f;  // 추가
    public float maxHp = 1000f;
    public float currentHp;
    public float maxMana = 400f;
    public float currentMana;
    public bool isAttacking = false;

    void Start()
    {
        currentMana = maxMana;
        currentHp = maxHp;
        baseAttackPower = attackPower;
    }

    public void Heal(float amount)
    {
        currentHp = Mathf.Min(currentHp + amount, maxHp);
        Debug.Log($"힐 적용: {amount} / 현재 체력: {currentHp}");
    }

    public void ChargeMana(float amount)
    {
        currentMana = Mathf.Min(currentMana + amount, maxMana);
    }
}