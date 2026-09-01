using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject attackPrefab;
    public Transform attackPoint;
    public float attackCooldown = 0.5f;

    private float lastAttackTime;
    private KakaruPassive kakaruPassive;
    private JinchuuPassive jinchuuPassive;
    private JinchuuE jinchuuE;
    private AmiyaPassive amiyaPassive;
    private AmiyaW amiyaW;

    void Start()
    {
        kakaruPassive = GetComponent<KakaruPassive>();
        jinchuuPassive = GetComponent<JinchuuPassive>();
        jinchuuE = GetComponent<JinchuuE>();
        amiyaPassive = GetComponent<AmiyaPassive>();
        amiyaW = GetComponent<AmiyaW>();
    }

    public void DoAttack(GameObject target)
    {
        if (Time.time - lastAttackTime < attackCooldown) return;
        lastAttackTime = Time.time;
        if (target == null) return;

        GameObject atk = Instantiate(
            attackPrefab, attackPoint.position, Quaternion.identity);
        Vector3 dir = (target.transform.position
            - attackPoint.position).normalized;
        atk.transform.forward = dir;

        Bullet bullet = atk.GetComponent<Bullet>();
        if (bullet != null) bullet.SetTarget(target);

        if (kakaruPassive != null) kakaruPassive.OnAttackHit(target);
        if (jinchuuPassive != null) jinchuuPassive.OnHit();
        if (jinchuuE != null && jinchuuE.IsEmpowered)
            jinchuuE.OnEmpoweredHit(target);
        if (amiyaPassive != null) amiyaPassive.OnHit();
        if (amiyaW != null) amiyaW.RegisterHit(target);
    }
}