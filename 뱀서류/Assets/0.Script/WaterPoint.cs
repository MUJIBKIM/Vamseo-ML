using System.Collections;
using UnityEngine;

public class WaterPoint : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Coroutine fillCoroutine;
    float arc = 0;
    public bool waterState = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            fillCoroutine = StartCoroutine("FillGauge");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            // 플레이어가 영역을 벗어나면 코루틴 중지
            if (fillCoroutine != null)
            {
                StopCoroutine(fillCoroutine);
                fillCoroutine = null;
            }
            // 필요에 따라 arc 값을 재설정할 수 있습니다.
            // arc = 0; // 필요하다면 arc를 초기화
        }

    }

    IEnumerator FillGauge()
    {
        // 무한 루프 대신 arc 값이 1.0에 도달할 때까지로 조건을 변경할 수 있습니다.
        while (true)
        {
            arc += 0.2f;
            spriteRenderer.material.SetFloat("_Arc1", arc);
            yield return null;
            if (arc > 360f)
            {
                waterState = true;
            }
        }
    }
}
