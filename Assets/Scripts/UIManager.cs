using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Panels / Buttons")]
    public GameObject settingsPanel;
    public GameObject[] mainButtons;

    [Header("Audio Settings UI")]
    [SerializeField] private Toggle bgmToggle;
    [SerializeField] private Slider bgmVolumeSlider;

    [Header("Display Settings UI")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    [Header("Debug")]
    [SerializeField] private bool debugLog = true;

    private const string PREF_BGM_ON = "BGM_ON";
    private const string PREF_BGM_VOL = "BGM_VOL";
    private const string PREF_FULLSCREEN = "FULLSCREEN";

    private Resolution[] resolutions;

    private void Start()
    {
        // 1) PlayerPrefs 기본값 보장 (AudioManager 없어도 수행)
        EnsureDefaultPrefs();

        // 2) 저장값 로드
        bool bgmOn = PlayerPrefs.GetInt(PREF_BGM_ON, 1) == 1;
        float vol = Mathf.Clamp01(PlayerPrefs.GetFloat(PREF_BGM_VOL, 0.7f));
        bool fullscreen = PlayerPrefs.GetInt(PREF_FULLSCREEN, 1) == 1;

        // 3) UI에 반영(이벤트 튐 방지)
        if (bgmToggle != null) bgmToggle.SetIsOnWithoutNotify(bgmOn);
        if (bgmVolumeSlider != null) bgmVolumeSlider.SetValueWithoutNotify(vol);
        if (fullscreenToggle != null) fullscreenToggle.SetIsOnWithoutNotify(fullscreen);

        // 4) 화면 반영 (AudioManager와 무관)
        Screen.fullScreen = fullscreen;
        SetupResolutionDropdown();

        // 5) 오디오 반영은 AudioManager 준비될 때까지 기다렸다가 적용
        StartCoroutine(ApplyAudioWhenReady());

        if (debugLog)
            Debug.Log($"[UIManager.Start] scene={UnityEngine.SceneManagement.SceneManager.GetActiveScene().name} bgmOn={bgmOn}, vol={vol}, fullscreen={fullscreen}");
    }

    private void EnsureDefaultPrefs()
    {
        bool changed = false;

        if (!PlayerPrefs.HasKey(PREF_BGM_ON))
        {
            PlayerPrefs.SetInt(PREF_BGM_ON, 1);
            changed = true;
        }

        if (!PlayerPrefs.HasKey(PREF_BGM_VOL))
        {
            PlayerPrefs.SetFloat(PREF_BGM_VOL, 0.7f);
            changed = true;
        }

        if (!PlayerPrefs.HasKey(PREF_FULLSCREEN))
        {
            PlayerPrefs.SetInt(PREF_FULLSCREEN, 1);
            changed = true;
        }

        if (changed) PlayerPrefs.Save();
    }

    private IEnumerator ApplyAudioWhenReady()
    {
        // 최대 2초 정도만 기다려보고(대부분 즉시 생김), 그래도 없으면 경고만
        float t = 0f;
        while (AudioManager.Instance == null && t < 2f)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("[UIManager] AudioManager.Instance가 없습니다. (씬에 AudioManager가 있나 확인)");
            yield break;
        }

        bool bgmOn = PlayerPrefs.GetInt(PREF_BGM_ON, 1) == 1;
        float vol = Mathf.Clamp01(PlayerPrefs.GetFloat(PREF_BGM_VOL, 0.7f));

        AudioManager.Instance.SetVolume(vol);
        AudioManager.Instance.SetMuted(!bgmOn);

        // 씬 진입 시 BGM 시작은 항상 보장 (OFF면 mute라서 소리만 안 남)
        TryPlaySceneBGM();

        if (debugLog)
        {
            var clipName = AudioManager.Instance.CurrentClip ? AudioManager.Instance.CurrentClip.name : "NULL";
            Debug.Log($"[UIManager] Audio applied. muted={!bgmOn}, vol={AudioManager.Instance.CurrentVolume}, clip={clipName}, playing={AudioManager.Instance.IsBgmPlaying}");
        }
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

    // -----------------------------
    // Audio UI Events
    // -----------------------------

    // Toggle의 OnValueChanged(bool)에 연결
    public void OnBgmToggleChanged(bool isOn)
    {
        PlayerPrefs.SetInt(PREF_BGM_ON, isOn ? 1 : 0);
        PlayerPrefs.Save();

        if (AudioManager.Instance == null)
        {
            if (debugLog) Debug.LogWarning("[OnBgmToggleChanged] AudioManager.Instance == null");
            return;
        }

        // Stop/Play 하지 말고 "뮤트"만
        AudioManager.Instance.SetMuted(!isOn);

        // ON인데 아직 클립/재생이 없으면 시작만 보장
        if (isOn && (!AudioManager.Instance.HasBgmClip || !AudioManager.Instance.IsBgmPlaying))
            TryPlaySceneBGM();

        if (debugLog)
            Debug.Log($"[OnBgmToggleChanged] isOn={isOn}, muted={AudioManager.Instance.IsMuted}, vol={AudioManager.Instance.CurrentVolume}");
    }

    // Slider의 OnValueChanged(float)에 연결 (0~1)
    public void OnBgmVolumeChanged(float value)
    {
        value = Mathf.Clamp01(value);

        PlayerPrefs.SetFloat(PREF_BGM_VOL, value);
        PlayerPrefs.Save();

        if (AudioManager.Instance == null)
        {
            if (debugLog) Debug.LogWarning("[OnBgmVolumeChanged] AudioManager.Instance == null");
            return;
        }

        AudioManager.Instance.SetVolume(value);

        if (debugLog)
            Debug.Log($"[OnBgmVolumeChanged] value={value}, effectiveVol={AudioManager.Instance.CurrentVolume}, muted={AudioManager.Instance.IsMuted}");
    }

    private void TryPlaySceneBGM()
    {
        var sceneBgm = FindFirstObjectByType<SceneBGM>();
        if (sceneBgm != null) sceneBgm.Play();
        else AudioManager.Instance.PlayCurrentBGM();

        if (debugLog)
            Debug.Log($"[TryPlaySceneBGM] found={(sceneBgm != null)}");
    }

    // -----------------------------
    // Quit
    // -----------------------------
    public void OnClickQuit()
    {
        Debug.Log("Quit button clicked");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // -----------------------------
    // Resolution / Fullscreen
    // -----------------------------
    private void SetupResolutionDropdown()
    {
        if (resolutionDropdown == null)
        {
            if (debugLog) Debug.LogWarning("resolutionDropdown이 연결되지 않았습니다. (Inspector에서 할당 필요)");
            return;
        }

        var raw = Screen.resolutions;

        // 같은 width x height 중복 제거
        List<Resolution> unique = new List<Resolution>();
        HashSet<string> seen = new HashSet<string>();

        for (int i = 0; i < raw.Length; i++)
        {
            string key = raw[i].width + "x" + raw[i].height;
            if (seen.Add(key))
                unique.Add(raw[i]);
        }

        resolutions = unique.ToArray();

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            options.Add($"{resolutions[i].width} x {resolutions[i].height}");

            // 현재 GameView(에디터)에서는 Screen.width/height 기준이 더 안전
            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
                currentIndex = i;
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.SetValueWithoutNotify(currentIndex);
        resolutionDropdown.RefreshShownValue();
    }

    // Dropdown의 OnValueChanged(int)에 연결
    public void OnResolutionChanged(int index)
    {
        if (resolutions == null || resolutions.Length == 0) return;

        index = Mathf.Clamp(index, 0, resolutions.Length - 1);
        Resolution r = resolutions[index];

        Screen.SetResolution(r.width, r.height, Screen.fullScreen);

        if (debugLog)
            Debug.Log($"[OnResolutionChanged] {r.width}x{r.height}, fullscreen={Screen.fullScreen}");
    }

    // Toggle의 OnValueChanged(bool)에 연결
    public void OnFullscreenToggleChanged(bool isOn)
    {
        PlayerPrefs.SetInt(PREF_FULLSCREEN, isOn ? 1 : 0);
        PlayerPrefs.Save();

        Screen.fullScreen = isOn;

        if (debugLog)
            Debug.Log($"[OnFullscreenToggleChanged] {isOn}");
    }
}
