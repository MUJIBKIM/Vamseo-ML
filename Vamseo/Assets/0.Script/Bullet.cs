using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int HitCount { get; set; }
    public int HitMaxCount { get; set; }
    public int damage;
    public float speed = 5f;
    public PlayerAgent agent;
    // Start is called before the first frame update
    void Start()
    {
        HitCount = 0;
        Destroy(gameObject, 2f);
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
    public virtual void SetPlayer(PlayerAgent p)
    {
        this.agent = p;
    }
    public void Initialize(Vector2 direction)
    {
        transform.up = direction;
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Monster"))
        {
            Destroy(other.gameObject);
            agent.hitcheck = 1;
        }
        else if (other.gameObject.CompareTag("Wall"))
        {
            Destroy(this.gameObject);
            agent.wallcheck = 1;
        }
    }
}
