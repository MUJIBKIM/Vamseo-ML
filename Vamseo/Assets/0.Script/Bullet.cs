using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int HitCount { get; set; }
    public int HitMaxCount { get; set; }
    public int damage;
    public float speed = 20f;
    // Start is called before the first frame update
    void Start()
    {
        HitCount = 0;
        Destroy(gameObject, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        damage = 100;
        if (UI.instance.gameState != GameState.Play)
            return;
        transform.Translate(Vector3.up * Time.deltaTime * speed);
    }
    public void SetHitMaxCount(int count)
    {
        HitMaxCount = count;
    }
}
