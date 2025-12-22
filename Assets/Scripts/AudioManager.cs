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

        // 안전: 시작 시 현재 상태 반영
        ApplyBgmVolume();
    }

    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource == null) return;
        if (clip == null) return;

        // 같은 클립이면 재시작하지 않음(중요!)
        if (bgmSource.clip == clip)
        {
            // 혹시 멈춰있기만 하면 다시 재생
            if (!bgmSource.isPlaying)
                bgmSource.Play();
            return;
        }

        // 다른 곡이면 교체 후 재생(이건 씬별 BGM 전환을 위해 필요)
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

    // 이제 Stop은 "곡을 끊고 싶을 때"만 사용 (토글 OFF에는 사용하지 말 것)
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

    private void ApplyBgmVolume()
    {
        if (bgmSource == null) return;
        bgmSource.volume = isMuted ? 0f : userVolume;
    }
}
