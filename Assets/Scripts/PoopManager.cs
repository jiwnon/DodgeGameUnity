using UnityEngine;

public class PoopManager : MonoBehaviour
{
    // 1. 유니티 에디터에서 'Poop' 프리팹을 여기에 넣어줄 거에요.
    public GameObject poopPrefab;

    // 2. 똥이 떨어질 간격 (예: 1초마다)
    public float interval = 1f;

    void Start()
    {
        // 3. Start 후 0초부터 interval(1초)마다 PoopDrop 함수를 반복해서 실행합니다.
        InvokeRepeating("PoopDrop", 0f, interval);
    }

    void PoopDrop()
    {
        // 4. 화면의 가장자리 범위 (Unity에서는 대략 -9 ~ 9)
        float maxLeft = -8f;
        float maxRight = 8f;

        // 5. maxLeft와 maxRight 사이에서 무작위 x 위치를 결정합니다.
        float randomX = Random.Range(maxLeft, maxRight);

        // 6. 똥이 생성될 위치 (무작위 x, 화면 위쪽 y=5)
        Vector3 spawnPosition = new Vector3(randomX, 5f, 0);

        // 7. poopPrefab을 spawnPosition에 생성(Instantiate)합니다.
        Instantiate(poopPrefab, spawnPosition, Quaternion.identity);
    }
}