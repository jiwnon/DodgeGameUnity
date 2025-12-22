using UnityEngine;

public class SceneBGM : MonoBehaviour
{
    [SerializeField] private AudioClip bgmClip;

    private const string PREF_BGM_ON = "BGM_ON";

    private void Start()
    {
        bool bgmOn = PlayerPrefs.GetInt(PREF_BGM_ON, 1) == 1;
        Debug.Log($"SceneBGM Start | scene={UnityEngine.SceneManagement.SceneManager.GetActiveScene().name} | bgmOn={bgmOn} | clip={(bgmClip ? bgmClip.name : "NULL")} | audioMgr={(AudioManager.Instance ? "OK" : "NULL")}");

        if (!bgmOn) return;
        Play();
    }

    public void Play()
    {
        if (bgmClip == null) return;
        AudioManager.Instance?.PlayBGM(bgmClip);
    }
}
