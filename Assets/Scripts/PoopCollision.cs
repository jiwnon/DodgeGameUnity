using UnityEngine;
using TMPro; // TextMeshPro를 사용하기 위해 추가

public class PoopCollision : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Game Over!");

            // 1. Scene에서 ScoreManager 컴포넌트를 가진 오브젝트를 찾습니다.
            ScoreManager sm = FindFirstObjectByType<ScoreManager>();

            if (sm != null)
            {
                // 2. 현재 점수를 최종 점수에 저장하고, 게임 오버 패널을 켭니다.
                sm.finalScore = sm.score;
                sm.gameOverPanel.SetActive(true);

                // 3. 게임 오버 패널 내의 FinalScoreText를 찾아 점수를 표시합니다.
                // *이름이 FinalScoreText인지 확인해주세요!*
                TextMeshProUGUI finalScoreText = sm.gameOverPanel.transform.Find("FinalScoreText").GetComponent<TextMeshProUGUI>();
                finalScoreText.text = "Final Score: " + Mathf.FloorToInt(sm.finalScore).ToString();
            }

            // 4. 모든 움직임 멈춤 (Time.timeScale = 0f)
            Time.timeScale = 0f;

            Destroy(gameObject);
        }

        // (Ground 태그에 닿으면 똥 삭제 로직은 그대로 둡니다.)
        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}