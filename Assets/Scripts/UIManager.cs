using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Panels / Buttons")]
    public GameObject settingsPanel;
    public GameObject[] mainButtons;

    [Header("Audio Settings UI")]
    [SerializeField] private Toggle bgmToggle;
    [SerializeField] private Slider bgmVolumeSlider;

    private const string PREF_BGM_ON = "BGM_ON";
    private const string PREF_BGM_VOL = "BGM_VOL";

    private void Start()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("AudioManager 인스턴스가 없습니다. Audio 관련 호출이 무시됩니다.");
            return;
        }

        // 기본값 보장
        if (!PlayerPrefs.HasKey(PREF_BGM_ON))
            PlayerPrefs.SetInt(PREF_BGM_ON, 1);

        if (!PlayerPrefs.HasKey(PREF_BGM_VOL))
            PlayerPrefs.SetFloat(PREF_BGM_VOL, 0.7f);

        PlayerPrefs.Save();

        // 저장값 로드
        bool bgmOn = PlayerPrefs.GetInt(PREF_BGM_ON, 1) == 1;
        float vol = Mathf.Clamp01(PlayerPrefs.GetFloat(PREF_BGM_VOL, 0.7f));

        // UI에 반영(이벤트 튐 방지)
        if (bgmToggle != null)
            bgmToggle.SetIsOnWithoutNotify(bgmOn);

        if (bgmVolumeSlider != null)
            bgmVolumeSlider.SetValueWithoutNotify(vol);

        // 오디오에 반영
        AudioManager.Instance.SetVolume(vol);
        AudioManager.Instance.SetMuted(!bgmOn);

        // 씬 진입 시 BGM은 "항상" 시작(트랙 선택/전환은 SceneBGM이 담당)
        // OFF 상태라면 이미 SetMuted(true)라서 소리만 안 나고, 재생 위치는 유지됨
        TryPlaySceneBGM();
    }

    public void ToggleSettings()
    {
        bool isPanelActive = !settingsPanel.activeSelf;
        settingsPanel.SetActive(isPanelActive);

        bool shouldButtonsBeActive = !isPanelActive;
        foreach (GameObject button in mainButtons)
            if (button != null) button.SetActive(shouldButtonsBeActive);

        Time.timeScale = isPanelActive ? 0f : 1f;
    }

    // Toggle의 OnValueChanged(bool)에 연결
    public void OnBgmToggleChanged(bool isOn)
    {
        PlayerPrefs.SetInt(PREF_BGM_ON, isOn ? 1 : 0);
        PlayerPrefs.Save();

        // ✅ Stop/Play 하지 말고 "뮤트"만
        AudioManager.Instance.SetMuted(!isOn);

        // ON으로 켰는데 아직 클립이 없거나 재생이 안 되고 있으면 시작만 보장
        if (isOn && (!AudioManager.Instance.HasBgmClip || !AudioManager.Instance.IsBgmPlaying))
            TryPlaySceneBGM();
    }

    // Slider의 OnValueChanged(float)에 연결 (0~1)
    public void OnBgmVolumeChanged(float value)
    {
        value = Mathf.Clamp01(value);

        PlayerPrefs.SetFloat(PREF_BGM_VOL, value);
        PlayerPrefs.Save();

        AudioManager.Instance.SetVolume(value);
    }

    private void TryPlaySceneBGM()
    {
        var sceneBgm = FindFirstObjectByType<SceneBGM>();
        if (sceneBgm != null) sceneBgm.Play();
        else AudioManager.Instance.PlayCurrentBGM();
    }
}
