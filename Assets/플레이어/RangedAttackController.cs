using UnityEngine;

public class RangedAttackController : MonoBehaviour
{
    [Header("탄환 설정")]
    public int maxAmmo = 12;
    public float reloadTime = 1.0f;
    public float attackCooldown = 0.5f;
    public float attackRange = 4f;
    public GameObject projectilePrefab;
    public Transform firePoint;

    protected int currentAmmo;
    protected bool isReloading = false;
    protected float lastAttackTime;

    public bool IsReloading => isReloading;
    public int CurrentAmmo => currentAmmo;

    protected virtual void Start()
    {
        currentAmmo = maxAmmo;
    }

    public virtual bool TryFire(GameObject target)
    {
        if (isReloading) return false;
        if (Time.time - lastAttackTime < attackCooldown) return false;
        if (currentAmmo <= 0)
        {
            StartCoroutine(ReloadRoutine());
            return false;
        }

        Fire(target);
        return true;
    }

    protected virtual void Fire(GameObject target)
    {
        lastAttackTime = Time.time;
        currentAmmo--;

        GameObject proj = Instantiate(
            projectilePrefab, firePoint.position, Quaternion.identity);
        Vector3 dir = (target.transform.position
            - firePoint.position).normalized;
        proj.transform.forward = dir;

        Bullet bullet = proj.GetComponent<Bullet>();
        if (bullet != null) bullet.SetTarget(target);

        if (currentAmmo <= 0)
            StartCoroutine(ReloadRoutine());
    }

    protected System.Collections.IEnumerator ReloadRoutine()
    {
        isReloading = true;
        Debug.Log("재장전 시작");
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
        Debug.Log("재장전 완료");
    }
}