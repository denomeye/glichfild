using UnityEngine;
using System;

public class SkillController : MonoBehaviour
{
    public SkillBase skillQ;
    public SkillBase skillW;
    public SkillBase skillE;
    public SkillBase skillR;

    public event Action OnSkillUsed;

    private float currentMana = 999f;

    void Update()
    {
        // Ctrl+Q/W/E/R은 스킬 포인트 사용 키이므로 시전하지 않음
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) return;

        if (Input.GetKeyDown(KeyCode.Q) && skillQ != null)
            if (skillQ.TryUse(currentMana))
                OnSkillUsed?.Invoke();

        if (Input.GetKeyDown(KeyCode.W) && skillW != null)
            if (skillW.TryUse(currentMana))
                OnSkillUsed?.Invoke();

        if (Input.GetKeyDown(KeyCode.E) && skillE != null)
            if (skillE.TryUse(currentMana))
                OnSkillUsed?.Invoke();

        if (Input.GetKeyDown(KeyCode.R) && skillR != null)
            if (skillR.TryUse(currentMana))
                OnSkillUsed?.Invoke();
    }
}
