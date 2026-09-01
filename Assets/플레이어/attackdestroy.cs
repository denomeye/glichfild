using UnityEngine;

public class AttackDestroy : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 0.5f);
    }
}