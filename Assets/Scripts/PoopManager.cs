using UnityEngine;

public class PoopManager : MonoBehaviour
{
    [SerializeField] private GameObject poopPrefab;
    [SerializeField] private float spawnInterval = 1.0f;

    [Header("Spawn Bounds (Camera-based)")]
    [SerializeField] private float xPadding = 0.5f;   // 화면 가장자리에서 안쪽으로
    [SerializeField] private float yOffset = 1.0f;    // 화면 위에서 추가로 더 위로

    private float timer;
    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
        if (cam == null)
            Debug.LogError("Main Camera not found. Tag your camera as MainCamera.");
    }

    private void Update()
    {
        if (cam == null) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnPoop();
        }
    }

    private void SpawnPoop()
    {
        // 화면 좌/우 끝 월드 좌표 얻기 (y는 아무 값이나 상관없음)
        Vector3 left = cam.ViewportToWorldPoint(new Vector3(0f, 0.5f, cam.nearClipPlane));
        Vector3 right = cam.ViewportToWorldPoint(new Vector3(1f, 0.5f, cam.nearClipPlane));

        float minX = left.x + xPadding;
        float maxX = right.x - xPadding;

        // 화면 위쪽 월드 좌표
        Vector3 top = cam.ViewportToWorldPoint(new Vector3(0.5f, 1f, cam.nearClipPlane));
        float spawnY = top.y + yOffset;

        float spawnX = Random.Range(minX, maxX);
        Instantiate(poopPrefab, new Vector3(spawnX, spawnY, 0f), Quaternion.identity);
    }
}
