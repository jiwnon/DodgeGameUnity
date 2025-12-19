using UnityEngine;

public class SceneBGM : MonoBehaviour
{
    [SerializeField] private AudioClip bgmClip;

    private void Start()
    {
        AudioManager.Instance?.PlayBGM(bgmClip);
    }
}
