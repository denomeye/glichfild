using UnityEngine;

public class ShieldController : MonoBehaviour
{
    [Header("½¯µå ¼³Á¤")]
    public float shieldAmount = 0f;
    public float shieldDuration = 0f;

    private float shieldTimer = 0f;
    private bool hasShield = false;

    public bool HasShield => hasShield;

    protected virtual void Update()
    {
        if (!hasShield) return;

        shieldTimer -= Time.deltaTime;
        if (shieldTimer <= 0f)
        {
            shieldAmount = 0f;
            hasShield = false;
            Debug.Log("½¯µå ¼Ò¸ê");
        }
    }

    public virtual void ApplyShield(float amount, float duration)
    {
        shieldAmount = amount;
        shieldDuration = duration;
        shieldTimer = duration;
        hasShield = true;
        Debug.Log($"½¯µå Àû¿ë: {amount} / {duration}ÃÊ");
    }

    // ÇÇÇØ ¹ÞÀ» ¶§ ½¯µå ¸ÕÀú ¼Ò¸ð
    public float AbsorbDamage(float damage)
    {
        if (!hasShield) return damage;

        if (shieldAmount >= damage)
        {
            shieldAmount -= damage;
            if (shieldAmount <= 0f) hasShield = false;
            return 0f;
        }
        else
        {
            float remaining = damage - shieldAmount;
            shieldAmount = 0f;
            hasShield = false;
            return remaining;
        }
    }
}