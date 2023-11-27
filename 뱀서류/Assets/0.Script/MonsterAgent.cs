using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAgent : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] private SpriteRenderer sr;
    void Update()
    {
        float x = player.transform.position.x - transform.position.x;
        sr.flipX = x < 0 ? true : x == 0 ? true : false;
        Vector2 v1 = (player.transform.position - transform.position).normalized * Time.deltaTime * 1f;
        transform.Translate(v1);
    }
}
