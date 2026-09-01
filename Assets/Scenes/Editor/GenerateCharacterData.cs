using UnityEditor;
using UnityEngine;

// Assets/Editor 폴더에 넣을 것. Unity 메뉴에 "Tools/캐릭터 데이터 생성" 항목이 추가됨.
public class GenerateCharacterData
{
    private class CharInfo
    {
        public string fileName;
        public string characterName;
        public string role;
        public string passiveDesc;
        public string qDesc;
        public string wDesc;
        public string eDesc;
        public string rDesc;
    }

    [MenuItem("Tools/캐릭터 데이터 생성")]
    public static void Generate()
    {
        string folder = "Assets/Resources/CharacterData";
        if (!AssetDatabase.IsValidFolder(folder))
        {
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");
            AssetDatabase.CreateFolder("Assets/Resources", "CharacterData");
        }

        CharInfo[] list = new CharInfo[]
        {
            new CharInfo
            {
                fileName = "Zhongli",
                characterName = "종려",
                role = "탱커/마법사",
                passiveDesc = "잃은 체력에 비례해 방어력 증가 (최대 35%).",
                qDesc = "전방 투사체. 마법 피해 80/120/160 (+주문력 40%). 둔화 35% 2초. 쿨타임 6/6/5초. 마나 60/70/80.",
                wDesc = "3초간 피해 감소 20/25/30%. 종료 시 광역 마법 피해 100/150/200 (+주문력 60%) + 넉백. 쿨타임 12/11/10초. 마나 70/80/90.",
                eDesc = "지정 위치에 암주 소환 2초. 소환 시 반경 넉백 + 마법 피해 60/95/130 (+주문력 35%). 쿨타임 10/9/8초. 마나 80/90/100.",
                rDesc = "광역 운석. 마법 피해 300/450 (+주문력 80%). 석화 1.5초. 쿨타임 110/90초. 마나 120.",
            },
            new CharInfo
            {
                fileName = "Kakaru",
                characterName = "카카루",
                role = "전사 (신규)",
                passiveDesc = "스킬 사용 시 다음 기본 공격이 2회 발동. 쿨타임 10초. 각 공격은 온힛 효과 적용.",
                qDesc = "반경 300 원형 베기. 물리 피해 65/100/135 (+공격력 60%). 쿨타임 8/7/6초. 마나 55/65/75.",
                wDesc = "돌진 500. 물리 피해 70/105/140 (+공격력 70%) + 기절 1초. 쿨타임 14/13/12초. 마나 70/80/90.",
                eDesc = "반경 350 물리 피해 55/85/115 (+공격력 50%) + 둔화 15% 3초. 쿨타임 12/11/10초. 마나 60/70/80.",
                rDesc = "10초간 방어 관통 20%, 스킬 후 이속 20% 1초, 패시브 쿨타임 50% 감소. 쿨타임 90/75초. 마나 110.",
            },
            new CharInfo
            {
                fileName = "Jinchuu",
                characterName = "진천우",
                role = "암살자/전사",
                passiveDesc = "평타 적중 시 스택 획득 (최대 8스택, 5초 유지). 스택당 공격력 +1.5, 공격속도 +2.5%.",
                qDesc = "3초간 공격력 15/20/25% 증가, 이속 20% 증가. 쿨타임 10/9/8초.",
                wDesc = "대시 500. 경로 상 적에게 물리 피해 60/90/120 (+공격력 70%). 평타 판정. 쿨타임 8/7/6초.",
                eDesc = "다음 평타 사거리 +50. 추가 물리 피해 40/70/100 + 에어본 0.5초. 최소 쿨타임 5초.",
                rDesc = "환영 상태로 6회 공격. 피해 50/80 (+공격력 40%). 단일 대상 시 15% 추가 피해. 쿨타임 100/85초.",
            },
            new CharInfo
            {
                fileName = "Centereichi",
                characterName = "센터우레이시",
                role = "원거리",
                passiveDesc = "탄환 12발, 재장전 1.0초. 기본 공격 5회 적중 시 접대 상태: 고정 피해 30 (+추가 공격력 20%).",
                qDesc = "2발 연사. 물리 피해 40/65/90 (+공격력 60%). 탄환 2발. 쿨타임 6초.",
                wDesc = "다음 평타 강화. 물리 피해 70/110/150 (+공격력 80%) + 둔화 35% 1초. 탄환 1발. 쿨타임 9/8/7초.",
                eDesc = "근처 적 물리 피해 60/100/140 (+공격력 50%) + 밀쳐내기. 쿨타임 12/11/10초.",
                rDesc = "8초간 공격력 15%, 이속 15% 증가. 쿨타임 110/90초.",
            },
            new CharInfo
            {
                fileName = "Amiya",
                characterName = "아미야",
                role = "마법사/암살자",
                passiveDesc = "스킬 적중 시 마나 충전 (최대 마나의 1.5%).",
                qDesc = "5초간 공격속도 30/40/50% 증가, 평타 100% 마법 피해 전환. 마나 소모 80/90/100. 쿨타임 10초.",
                wDesc = "최근 3초 내 피격 대상에게 시전 가능. 마법 피해 + 체력 회복. 마나 60/80/100. 쿨타임 8/7/6초.",
                eDesc = "일직선 10회 연속 마법탄. 발당 주문력 25% 피해. 5발 이상 명중 시 쿨타임 20% 반환. 쿨타임 10/9/8초.",
                rDesc = "좁은 원형 범위 폭발. 잃은 체력 비례 마법 피해 8/10% + 주문력 60%. 처치 시 1.5초 내 무료 재발동. 쿨타임 80/70초.",
            },
            new CharInfo
            {
                fileName = "Perfumer",
                characterName = "퍼퓨머",
                role = "서포터",
                passiveDesc = "반경 600 내 아군에게 초당 35 회복. 공격 중이면 마나 2% 추가 충전.",
                qDesc = "아군 1명 즉시 회복 80/120/160 (+주문력 60%). 쿨타임 5/5/4초. 마나 100.",
                wDesc = "3초간 반경 500 강인함 오오라. CC 지속시간 30/35/40% 감소. 쿨타임 18/16/14초. 마나 110.",
                eDesc = "전방 부채꼴 70도. 마법 피해 60/90/120 (+주문력 40%) + 둔화 25% 1.5초. 쿨타임 12/11/10초. 마나 95.",
                rDesc = "10초간 초당 회복 35 + 주문력 15%. 종료 시 잃은 체력 10% 회복. 쿨타임 120/100초. 마나 130.",
            },
        };

        foreach (CharInfo info in list)
        {
            string path = $"{folder}/{info.fileName}.asset";

            CharacterData data = AssetDatabase.LoadAssetAtPath<CharacterData>(path);
            bool isNew = data == null;
            if (isNew)
                data = ScriptableObject.CreateInstance<CharacterData>();

            data.characterName = info.characterName;
            data.role = info.role;
            data.passiveDesc = info.passiveDesc;
            data.qDesc = info.qDesc;
            data.wDesc = info.wDesc;
            data.eDesc = info.eDesc;
            data.rDesc = info.rDesc;
            // portrait, prefab은 직접 인스펙터에서 연결

            if (isNew)
                AssetDatabase.CreateAsset(data, path);
            else
                EditorUtility.SetDirty(data);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"CharacterData {list.Length}개 생성/갱신 완료: {folder}");
    }
}
