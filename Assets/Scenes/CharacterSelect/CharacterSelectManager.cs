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

    [Header("스킬 아이콘 (클릭 시 해당 설명만 표시)")]
    public Button passiveIconButton;
    public Button qIconButton;
    public Button wIconButton;
    public Button eIconButton;
    public Button rIconButton;

    public Button confirmButton; // 선택 완료 버튼

    private CharacterData selected;

    void Start()
    {
        confirmButton.interactable = false;
        GenerateButtons();
        WireSkillIcons();
        HideAllSkillTexts();
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

    // 스킬 아이콘 클릭 시 해당 설명 텍스트만 토글 (Start에서 한 번만 연결)
    void WireSkillIcons()
    {
        if (passiveIconButton != null)
            passiveIconButton.onClick.AddListener(() => ToggleSkillText(passiveText));
        if (qIconButton != null)
            qIconButton.onClick.AddListener(() => ToggleSkillText(qText));
        if (wIconButton != null)
            wIconButton.onClick.AddListener(() => ToggleSkillText(wText));
        if (eIconButton != null)
            eIconButton.onClick.AddListener(() => ToggleSkillText(eText));
        if (rIconButton != null)
            rIconButton.onClick.AddListener(() => ToggleSkillText(rText));
    }

    void ToggleSkillText(TextMeshProUGUI target)
    {
        if (target == null) return;

        bool willShow = !target.gameObject.activeSelf;
        HideAllSkillTexts();
        target.gameObject.SetActive(willShow);
    }

    void HideAllSkillTexts()
    {
        if (passiveText != null) passiveText.gameObject.SetActive(false);
        if (qText != null) qText.gameObject.SetActive(false);
        if (wText != null) wText.gameObject.SetActive(false);
        if (eText != null) eText.gameObject.SetActive(false);
        if (rText != null) rText.gameObject.SetActive(false);
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

        // 캐릭터가 바뀌면 열려있던 스킬 설명은 닫고 새로 클릭하게 함
        HideAllSkillTexts();

        if (portraitImage != null && data.portrait != null)
            portraitImage.sprite = data.portrait;

        // 스킬 아이콘 이미지 적용 (아이콘 미지정 시 기존 이미지 유지)
        SetIconSprite(passiveIconButton, data.passiveIcon);
        SetIconSprite(qIconButton, data.qIcon);
        SetIconSprite(wIconButton, data.wIcon);
        SetIconSprite(eIconButton, data.eIcon);
        SetIconSprite(rIconButton, data.rIcon);

        confirmButton.interactable = true;

        Debug.Log($"선택: {data.characterName}");
    }

    void SetIconSprite(Button iconButton, Sprite icon)
    {
        if (iconButton == null || icon == null) return;
        Image img = iconButton.GetComponent<Image>();
        if (img != null) img.sprite = icon;
    }

    public void OnConfirmButton()
    {
        if (selected == null) return;
        SelectedCharacter.Name = selected.characterName;
        SelectedCharacter.Prefab = selected.prefab;
        SceneManager.LoadScene("SampleScene");
    }
}
