using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public Camera cam;

    void Update()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);
        RaycastHit hit;
        int layerMask = LayerMask.GetMask("Ground");

        if (Physics.Raycast(ray, out hit, 100f, layerMask))
        {
            Vector3 lookPos = hit.point - transform.position;
            lookPos.y = 0;

            if (lookPos.magnitude > 0.1f) // 👈 이거 추가
            {
                transform.rotation = Quaternion.LookRotation(lookPos);
            }
        }
    }
    }