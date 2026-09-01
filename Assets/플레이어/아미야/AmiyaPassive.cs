using UnityEngine;

public class AmiyaPassive : MonoBehaviour
{
    private PlayerStats stats;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
    }

    public void OnHit()
    {
        if (stats == null) return;
        stats.ChargeMana(stats.maxMana * 0.015f);
    }
}