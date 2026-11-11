using UnityEngine;

public abstract class SkillBase : MonoBehaviour
{
    public float cooldown;        // 쿨타임
    public float manaCost;        // 마나 소모
    public KeyCode skillKey;      // 어떤 키인지 (Q/W/E/R)

    protected float lastUsedTime = -999f; // 처음엔 바로 쓸 수 있게

    // 0이면 미습득. PlayerExperience가 없으면 기본값 1이라 기존처럼 항상 사용 가능
    public int LearnedLevel { get; private set; } = 1;

    // 쿨타임 남은 시간
    public float CooldownRemaining =>
        Mathf.Max(0f, cooldown - (Time.time - lastUsedTime));

    // 사용 가능한지 체크
    public bool CanUse(float currentMana)
    {
        return LearnedLevel > 0 && CooldownRemaining <= 0f && currentMana >= manaCost;
    }

    public void ResetCooldown()
    {
        lastUsedTime = -999f;
    }

    // 스킬 사용 시도
    public bool TryUse(float currentMana)
    {
        if (!CanUse(currentMana)) return false;
        lastUsedTime = Time.time;
        OnUse();
        return true;
    }

    // 각 스킬 스크립트의 skillLevel 필드와 ApplyLevel()을 이름으로 찾아 갱신
    public void SetLearnedLevel(int level)
    {
        LearnedLevel = level;

        var field = GetType().GetField("skillLevel");
        if (field == null)
        {
            Debug.LogWarning($"{GetType().Name}: skillLevel 필드가 없어 레벨을 반영하지 못함");
            return;
        }

        field.SetValue(this, Mathf.Max(level, 1));
        if (level > 0) GetType().GetMethod("ApplyLevel")?.Invoke(this, null);
    }

    // 각 스킬마다 다르게 구현할 부분
    protected abstract void OnUse();
}
