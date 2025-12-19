using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    // 새로 추가: 게임 오버 패널 오브젝트를 연결할 변수
    public GameObject gameOverPanel;

    // 최종 점수를 저장할 변수 (PoopCollision에서 접근하여 점수를 읽어갈 때 사용됩니다.)
    [HideInInspector] public float finalScore = 0f;

    public float score = 0f;

    void Update()
    {
        score += Time.deltaTime * 10f;
        scoreText.text = "Score: " + Mathf.FloorToInt(score).ToString();
    }
}