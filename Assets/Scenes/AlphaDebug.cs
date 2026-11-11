using UnityEngine;

[RequireComponent(typeof(PlayerExperience))]
public class AlphaDebug : MonoBehaviour
{
    public bool debugEnabled = true;

    private PlayerStats stats;
    private PlayerExperience exp;
    private SkillBase[] skills; // Q, W, E, R
    private bool noCooldown;
    private bool showHelp = true;

    void Awake()
    {
        stats = GetComponent<PlayerStats>();
        exp = GetComponent<PlayerExperience>();

        SkillController sc = GetComponent<SkillController>();
        skills = new SkillBase[] { sc.skillQ, sc.skillW, sc.skillE, sc.skillR };
    }

    void Update()
    {
        if (!debugEnabled) return;

        if (Input.GetKeyDown(KeyCode.F2)) exp.LevelUp();

        if (Input.GetKeyDown(KeyCode.F3))
        {
            stats.Heal(stats.maxHp);
            stats.ChargeMana(stats.maxMana);
        }

        if (Input.GetKeyDown(KeyCode.F4)) ResetCooldowns();
        if (Input.GetKeyDown(KeyCode.F5)) noCooldown = !noCooldown;
        if (Input.GetKeyDown(KeyCode.F6)) exp.xpPaused = !exp.xpPaused;
        if (Input.GetKeyDown(KeyCode.F9)) showHelp = !showHelp;

        if (noCooldown) ResetCooldowns();
    }

    void ResetCooldowns()
    {
        foreach (SkillBase s in skills)
            if (s != null) s.ResetCooldown();
    }

    void OnGUI()
    {
        if (!debugEnabled || !showHelp) return;

        float need = stats.level < PlayerStats.MaxLevel && stats.level - 1 < exp.xpToNextLevel.Length
            ? exp.xpToNextLevel[stats.level - 1] : 0f;

        GUI.Label(new Rect(10, 10, 480, 100),
            $"Lv {stats.level}  XP {exp.currentXp:0}/{need:0}  SP {exp.skillPoints}\n" +
            "F2 레벨업 / F3 체력,마나 회복 / F4 쿨타임 초기화\n" +
            $"F5 쿨타임 무제한 [{(noCooldown ? "ON" : "OFF")}] / F6 경험치 정지 [{(exp.xpPaused ? "ON" : "OFF")}]\n" +
            "F9 이 창 숨기기 / Ctrl+Q,W,E,R 스킬 습득");
    }
}
