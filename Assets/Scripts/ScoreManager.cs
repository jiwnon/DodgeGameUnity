using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText; // ✅ 추가

    public float score = 0f;

    private bool isGameOver = false;

    private void Awake()
    {
        // ✅ 씬에서 찾지 않고 전역 접근(싱글톤)
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        if (isGameOver) return;

        score += Time.deltaTime * 10f;
        scoreText.text = "Score: " + Mathf.FloorToInt(score).ToString();
    }

    public void GameOver()
    {
        if (isGameOver) return; // 중복 호출 방지
        isGameOver = true;

        gameOverPanel.SetActive(true);
        finalScoreText.text = "Final Score: " + Mathf.FloorToInt(score).ToString();

        Time.timeScale = 0f;
    }
}
