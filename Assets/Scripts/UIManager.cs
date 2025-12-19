using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject settingsPanel;

    // 새로 추가: 숨길 메인 화면 버튼들을 연결할 변수
    public GameObject[] mainButtons; // 여러 버튼을 한 번에 관리하기 위해 배열 사용

    public void ToggleSettings()
    {
        bool isPanelActive = !settingsPanel.activeSelf;
        settingsPanel.SetActive(isPanelActive);

        // **새로 추가된 로직:**
        // 설정창이 열리면(isPanelActive=true) 메인 버튼들을 숨기고, 닫히면 다시 보여줍니다.
        bool shouldButtonsBeActive = !isPanelActive;

        foreach (GameObject button in mainButtons)
        {
            if (button != null)
            {
                button.SetActive(shouldButtonsBeActive);
            }
        }

        // 기존 시간 정지 로직
        if (isPanelActive)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
}