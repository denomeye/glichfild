using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class EnemyStatus : MonoBehaviour
{
    private NavMeshAgent navAgent;
    private bool isStunned = false;

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
    }

    // ±âÀý
    public void ApplyStun(float duration)
    {
        if (!isStunned)
            StartCoroutine(StunRoutine(duration));
    }

    IEnumerator StunRoutine(float duration)
    {
        isStunned = true;

        if (navAgent != null)
        {
            navAgent.ResetPath();
            navAgent.enabled = false;
        }

        Debug.Log($"±âÀý! {duration}ÃÊ");
        yield return new WaitForSeconds(duration);

        if (navAgent != null)
            navAgent.enabled = true;

        isStunned = false;
    }

    // µÐÈ­
    public void ApplySlow(float amount, float duration)
    {
        StartCoroutine(SlowRoutine(amount, duration));
    }

    IEnumerator SlowRoutine(float amount, float duration)
    {
        if (navAgent == null) yield break;

        float originalSpeed = navAgent.speed;
        navAgent.speed = originalSpeed * (1f - amount);

        Debug.Log($"µÐÈ­! {amount * 100}% {duration}ÃÊ");
        yield return new WaitForSeconds(duration);

        navAgent.speed = originalSpeed;
    }
}