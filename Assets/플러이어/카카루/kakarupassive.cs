using UnityEngine;
using System.Collections;

public class KakaruPassive : MonoBehaviour
{
    [Header("패시브 수치")]
    public float passiveCooldown = 10f;  // 쿨타임 10초

    private bool isReady = true;
    private bool isEmpowered = false; // 강화 상태
    private float lastUsedTime = -999f;

    private PlayerAttack playerAttack;

    public bool IsEmpowered => isEmpowered;

    void Start()
    {
        playerAttack = GetComponent<PlayerAttack>();

        // SkillController에서 스킬 사용 감지
        SkillController sc = GetComponent<SkillController>();
        if (sc != null)
            sc.OnSkillUsed += OnSkillUsed;
    }

    void OnDestroy()
    {
        SkillController sc = GetComponent<SkillController>();
        if (sc != null)
            sc.OnSkillUsed -= OnSkillUsed;
    }

    void OnSkillUsed()
    {
        // 쿨타임 체크
        if (Time.time - lastUsedTime < passiveCooldown) return;

        lastUsedTime = Time.time;
        isEmpowered = true;

        Debug.Log("사냥꾼의 검술 발동! 다음 공격 2회");
    }

    // PlayerAttack에서 호출
    public void OnAttackHit(GameObject target)
    {
        if (!isEmpowered) return;

        isEmpowered = false;
        StartCoroutine(DoubleAttackRoutine(target));
    }

    IEnumerator DoubleAttackRoutine(GameObject target)
    {
        // 첫 번째 추가 공격
        FireExtraAttack(target);

        yield return new WaitForSeconds(0.15f);

        // 두 번째 추가 공격
        FireExtraAttack(target);

        Debug.Log("사냥꾼의 검술 2회 공격 완료");
    }

    void FireExtraAttack(GameObject target)
    {
        if (target == null) return;

        EnemyHealth eh = target.GetComponent<EnemyHealth>();
        if (eh == null) return;

        // PlayerAttack의 프리팹으로 투사체 발사
        if (playerAttack != null)
            playerAttack.DoAttack(target);
    }
}