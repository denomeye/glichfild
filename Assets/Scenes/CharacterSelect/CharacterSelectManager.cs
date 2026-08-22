using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CharacterSelectManager : MonoBehaviour
{
    [Header("데이터")]
    public CharacterData[] characters; // 6인 데이터 연결

    [Header("UI 연결")]
    public Transform buttonContainer;  // 버튼 나열할 부모 오브젝트
    public GameObject buttonPrefab;    // 캐릭터 버튼 프리팹

    // 선택 정보 표시 패널
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI roleText;
    public TextMeshProUGUI passiveText;
    public TextMeshProUGUI qText;
    public TextMeshProUGUI wText;
    public TextMeshProUGUI eText;
    public TextMeshProUGUI rText;
    public Image portraitImage;

    public Button confirmButton; // 선택 완료 버튼

    private CharacterData selected;

    void Start()
    {
        confirmButton.interactable = false;
        GenerateButtons();
    }

    void GenerateButtons()
    {
        foreach (CharacterData data in characters)
        {
            GameObject btn = Instantiate(buttonPrefab, buttonContainer);

            // 버튼 텍스트 설정
            btn.GetComponentInChildren<TextMeshProUGUI>().text
                = data.characterName;

            // 초상화 설정
            Image img = btn.GetComponent<Image>();
            if (data.portrait != null && img != null)
                img.sprite = data.portrait;

            // 클릭 이벤트
            CharacterData captured = data;
            btn.GetComponent<Button>().onClick.AddListener(() =>
                OnCharacterSelected(captured));
        }
    }

    void OnCharacterSelected(CharacterData data)
    {
        selected = data;

        // 정보 패널 업데이트
        nameText.text = data.characterName;
        roleText.text = data.role;
        passiveText.text = "패시브: " + data.passiveDesc;
        qText.text = "Q: " + data.qDesc;
        wText.text = "W: " + data.wDesc;
        eText.text = "E: " + data.eDesc;
        rText.text = "R: " + data.rDesc;

        if (portraitImage != null && data.portrait != null)
            portraitImage.sprite = data.portrait;

        confirmButton.interactable = true;

        Debug.Log($"선택: {data.characterName}");
    }

    public void OnConfirmButton()
    {
        if (selected == null) return;
        SelectedCharacter.Name = selected.characterName;
        SelectedCharacter.Prefab = selected.prefab;
        SceneManager.LoadScene("SampleScene");
    }
}