using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPFollow : MonoBehaviour
{
    RectTransform rect;
    public GameObject player;  // 플레이어 객체를 직접 참조하도록 public 변수 추가
    public float xoffset;
    public float yoffset;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    void FixedUpdate()
    {
        if (player != null)  // 플레이어가 할당되어 있을 때만 실행
        {
           Vector3 newPosition = new Vector3(player.transform.position.x, player.transform.position.y - 0.9f, player.transform.position.z);
           rect.position = newPosition;
        }
    }
}
