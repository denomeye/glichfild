using UnityEngine;
using UnityEngine.AI;

// SampleScene에서 캐릭터 선택창에서 고른 캐릭터 하나만 생성한다.
// 씬에는 캐릭터를 미리 배치해 두지 말 것 (배치된 캐릭터는 전부 같은 입력을 받아 같이 움직임)
public class CharacterSpawner : MonoBehaviour
{
    [Header("스폰 설정")]
    public Transform spawnPoint;        // 캐릭터가 생성될 위치 (NavMesh 위 또는 근처)
    public GameObject fallbackPrefab;   // 캐릭터 선택 없이 SampleScene을 바로 실행할 때만 쓰는 테스트용 프리팹

    void Awake()
    {
        GameObject prefab = SelectedCharacter.Prefab != null ? SelectedCharacter.Prefab : fallbackPrefab;
        if (prefab == null)
        {
            Debug.LogWarning("CharacterSpawner: 생성할 프리팹이 없음 (캐릭터 선택을 거치지 않았고 fallbackPrefab도 비어 있음)");
            return;
        }

        Vector3 pos = spawnPoint != null ? spawnPoint.position : transform.position;
        Quaternion rot = spawnPoint != null ? spawnPoint.rotation : transform.rotation;

        // NavMeshAgent는 NavMesh 위에서만 생성되므로 가장 가까운 NavMesh 지점으로 보정
        if (NavMesh.SamplePosition(pos, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            pos = hit.position;

        Instantiate(prefab, pos, rot);
    }
}
