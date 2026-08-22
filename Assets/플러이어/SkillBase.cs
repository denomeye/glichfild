using UnityEngine;

public abstract class SkillBase : MonoBehaviour
{
    public float cooldown;        // 쿨타임
    public float manaCost;        // 마나 소모
    public KeyCode skillKey;      // 어떤 키인지 (Q/W/E/R)

    protected float lastUsedTime = -999f; // 처음엔 바로 쓸 수 있게

    // 쿨타임 남은 시간
    public float CooldownRemaining =>
        Mathf.Max(0f, cooldown - (Time.time - lastUsedTime));

    // 사용 가능한지 체크
    public bool CanUse(float currentMana)
    {
        return CooldownRemaining <= 0f && currentMana >= manaCost;
    }

    // 스킬 사용 시도
    public bool TryUse(float currentMana)
    {
        if (!CanUse(currentMana)) return false;
        lastUsedTime = Time.time;
        OnUse();
        return true;
    }

    // 각 스킬마다 다르게 구현할 부분
    protected abstract void OnUse();
}