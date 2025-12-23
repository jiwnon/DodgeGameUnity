using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource bgmSource;

    // 유저가 설정한 볼륨(슬라이더 값)
    private float userVolume = 1f;

    // BGM ON/OFF 토글은 "뮤트"로 처리
    private bool isMuted = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // bgmSource를 인스펙터에 안 넣었을 때도 안전하게 보정
        if (bgmSource == null)
            bgmSource = GetComponent<AudioSource>();

        ApplyBgmVolume();
    }

    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource == null) return;
        if (clip == null) return;

        // 같은 클립이면 재시작하지 않음(중요!)
        if (bgmSource.clip == clip)
        {
            // 멈춰있기만 하면 다시 재생
            if (!bgmSource.isPlaying)
                bgmSource.Play();
            return;
        }

        // 다른 곡이면 교체 후 재생
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    public void PlayCurrentBGM()
    {
        if (bgmSource == null) return;
        if (bgmSource.clip == null) return;

        if (!bgmSource.isPlaying)
            bgmSource.Play();
    }

    // Stop은 "곡 자체를 끊고 싶을 때"만 (토글 OFF에는 사용하지 말 것)
    public void StopBGM()
    {
        if (bgmSource == null) return;
        bgmSource.Stop();
    }

    // 슬라이더(0~1)
    public void SetVolume(float volume)
    {
        userVolume = Mathf.Clamp01(volume);
        ApplyBgmVolume();
    }

    // 토글 ON/OFF는 뮤트로 처리
    public void SetMuted(bool muted)
    {
        isMuted = muted;
        ApplyBgmVolume();
    }

    public bool IsMuted => isMuted;
    public bool HasBgmClip => (bgmSource != null && bgmSource.clip != null);
    public bool IsBgmPlaying => (bgmSource != null && bgmSource.isPlaying);

    // 디버깅용으로 유용
    public AudioClip CurrentClip => bgmSource != null ? bgmSource.clip : null;
    public float CurrentVolume => bgmSource != null ? bgmSource.volume : 0f;

    private void ApplyBgmVolume()
    {
        if (bgmSource == null) return;
        bgmSource.volume = isMuted ? 0f : userVolume;
    }
}
