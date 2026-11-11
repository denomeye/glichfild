using UnityEngine;

public class PlayerExperience : MonoBehaviour
{
    [Header("경험치")]
    public float xpPerSecond = 1f;
    // 인덱스 0 = 1->2레벨에 필요한 경험치, 인덱스 8 = 9->10레벨 (초기값은 임의 값, 직접 조절)
    public float[] xpToNextLevel = { 60f, 90f, 120f, 150f, 180f, 210f, 240f, 270f, 300f };
    public float currentXp;
    public bool xpPaused;

    [Header("스킬 포인트")]
    public int skillPoints = 2;

    private PlayerStats stats;
    private SkillBase[] skills; // Q, W, E, R

    void Awake()
    {
        stats = GetComponent<PlayerStats>();

        SkillController sc = GetComponent<SkillController>();
        skills = new SkillBase[] { sc.skillQ, sc.skillW, sc.skillE, sc.skillR };

        foreach (SkillBase s in skills)
            if (s != null) s.SetLearnedLevel(0);
    }

    void Update()
    {
        if (!xpPaused) AddXp(xpPerSecond * Time.deltaTime);

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            if (Input.GetKeyDown(KeyCode.Q)) TryLearn(0);
            if (Input.GetKeyDown(KeyCode.W)) TryLearn(1);
            if (Input.GetKeyDown(KeyCode.E)) TryLearn(2);
            if (Input.GetKeyDown(KeyCode.R)) TryLearn(3);
        }
    }

    public void AddXp(float amount)
    {
        currentXp += amount;

        while (stats.level < PlayerStats.MaxLevel
               && stats.level - 1 < xpToNextLevel.Length
               && currentXp >= xpToNextLevel[stats.level - 1])
        {
            currentXp -= xpToNextLevel[stats.level - 1];
            LevelUp();
        }
    }

    public void LevelUp()
    {
        if (stats.level >= PlayerStats.MaxLevel) return;

        stats.SetLevel(stats.level + 1);
        skillPoints++;
    }

    // slot: 0=Q, 1=W, 2=E, 3=R
    public bool CanLearn(int slot)
    {
        if (skillPoints <= 0) return false;

        SkillBase s = skills[slot];
        if (s == null) return false;

        bool isR = slot == 3;
        int next = s.LearnedLevel + 1;
        if (next > (isR ? 2 : 3)) return false;

        // R: 1단계는 4레벨, 2단계는 8레벨부터
        if (isR && stats.level < (next == 1 ? 4 : 8)) return false;

        return true;
    }

    public bool TryLearn(int slot)
    {
        if (!CanLearn(slot)) return false;

        skillPoints--;
        skills[slot].SetLearnedLevel(skills[slot].LearnedLevel + 1);
        return true;
    }
}
