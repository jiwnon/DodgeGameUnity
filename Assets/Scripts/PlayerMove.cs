using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float speed = 10f; // 이동 속도

    void Update()
    {
        // 1. 키보드의 좌우 방향키 입력을 받습니다 (-1 ~ 1 사이의 값)
        float x = Input.GetAxis("Horizontal");

        // 2. 입력받은 만큼 오른쪽/왼쪽으로 이동합니다
        // (Time.deltaTime은 컴퓨터 성능과 상관없이 일정한 속도를 만들어줍니다)
        transform.Translate(x * speed * Time.deltaTime, 0, 0);
    }
}