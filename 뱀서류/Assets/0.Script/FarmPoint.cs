using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class FarmPoint : MonoBehaviour
{
    [SerializeField]
    GameObject waterPointObject;
    private WaterPoint waterPoint;
    SpriteRenderer spriteRenderer;
    Coroutine fillCoroutine;
    float arc = 0;
    void Start()
    {
        waterPoint = waterPointObject.GetComponent<WaterPoint>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player") 
        {
            if(waterPoint.waterState == true)
            {
                fillCoroutine = StartCoroutine("FillGauge");
            }
        }
        else
        {   
            return;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            if (fillCoroutine != null)
            {
                StopCoroutine(fillCoroutine);
                fillCoroutine = null;
            }
        }
    }
    IEnumerator FillGauge()
    {
        while (true)
        {
            arc += 0.2f;
            spriteRenderer.material.SetFloat("_Arc1", arc);
            Debug.Log("FillGauge 실행 중");
            yield return null;
        }
    }
}
