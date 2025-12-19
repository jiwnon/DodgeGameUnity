using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // 누락된 함수: 특정 씬으로 이동하는 함수 (StartButton이 호출하는 함수)
    public void LoadTargetScene(string sceneName)
    {
        // 씬 전환 전, 게임이 멈춰있을 경우를 대비해 시간을 다시 흐르게 합니다.
        Time.timeScale = 1f;

        SceneManager.LoadScene(sceneName);
    }

    // 새로 추가: 현재 씬을 다시 로드하여 게임을 재시작하는 함수
    public void RestartGame()
    {
        // 1. 멈췄던 시간을 다시 흐르게 합니다. (필수!)
        Time.timeScale = 1f;

        // 2. 현재 씬의 이름을 가져와서 다시 로드합니다.
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}