using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public Camera cam;

    void Start()
    {
        // 프리팹으로 생성된 캐릭터는 씬의 카메라를 미리 연결해 둘 수 없으므로 메인 카메라를 사용
        if (cam == null) cam = Camera.main;
    }

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
