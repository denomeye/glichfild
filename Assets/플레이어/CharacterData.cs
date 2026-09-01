using UnityEngine;

[CreateAssetMenu(
    fileName = "NewCharacter",
    menuName = "GlitchArena/CharacterData")]
public class CharacterData : ScriptableObject
{
    [Header("기본 정보")]
    public string characterName;
    public string role;           // 탱커/전사/암살자 등
    public Sprite portrait;       // 초상화
    public GameObject prefab;     // 스폰할 프리팹

    [Header("스킬 설명")]
    [TextArea] public string passiveDesc;
    [TextArea] public string qDesc;
    [TextArea] public string wDesc;
    [TextArea] public string eDesc;
    [TextArea] public string rDesc;
}