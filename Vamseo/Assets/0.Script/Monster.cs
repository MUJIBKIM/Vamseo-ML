using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField] public Player p;
    [SerializeField] public PlayerAgent playeragent;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] protected Animator animator;
    [SerializeField] protected GameObject expPrefab;
    [SerializeField] protected GameObject magPrefab;
 
    public float hp;
    protected float atkTime = 2f;
    protected int power = 6;
    private float atkTimer;
    private float hitFreezeTimer;
    // Start is called before the first frame update
    protected void Start()
    {
        hp = 100;
        hp = 100;
    }
    // Update is called once per frame
    protected virtual void Update()
    {
        if (UI.instance.gameState != GameState.Play)
            return;
        /*
        if (p == null || hp < 0)
            return;*/
        if(hitFreezeTimer > 0)
        {
            hitFreezeTimer -= Time.deltaTime;
            return;
        }

        //float x = p.transform.position.x - transform.position.x;
        float x = playeragent.transform.position.x - transform.position.x;

        sr.flipX = x < 0 ? true : x == 0 ? true : false;

        //float distance = Vector2.Distance(p.transform.position, transform.position);
        float distance = Vector2.Distance(playeragent.transform.position, transform.position);
        Vector2 v1 = (playeragent.transform.position - transform.position).normalized * Time.deltaTime * 1f;
        transform.Translate(v1);
        /*
        if (distance <= 0.1) // 1에서 0.1로 수정함
        {
            atkTimer += Time.deltaTime;
            //공격
            if (atkTimer > atkTime)
            {
                atkTimer = 0;
                p.Hit(power);
            }
        }
        else
        {
            //이동
            if (true) // hp>0에서 true로 수정함
            {
                Vector2 v1 = (playeragent.transform.position - transform.position).normalized * Time.deltaTime * 1f;
                transform.Translate(v1);
            }
        }*/
    }
    public virtual void SetPlayer(Player p)
    {
        this.p = p;
    }

    public virtual void SetPlayer(PlayerAgent p)
    {
        this.playeragent = p;
    }
    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "PlayerBullet" && collision.GetComponent<Bullet>())
        {
            /*
            collision.GetComponent<Bullet>().HitCount++;
            if(collision.GetComponent <Bullet>().HitCount >= collision.GetComponent<Bullet>().HitMaxCount)
            {
                Destroy(collision.gameObject);
            }*/
            Destroy(collision.gameObject);
            //Dead(1f, 100);
        }
    }
    public virtual void Dead(float freezeTime, int damage)
    {
        hitFreezeTimer = freezeTime;           
        hp -= damage;
        AudioManager.instance.Play("hit1");
        if (hp <= 0)
        {
            Destroy(GetComponent<Rigidbody2D>());
            GetComponent<CapsuleCollider2D>().enabled = false;
            animator.SetBool("Dead", true);
            StartCoroutine("CDropExp");
            if(UnityEngine.Random.value < 0.005)
            {
                Instantiate(magPrefab, transform.position, Quaternion.identity);
            }
        }
    }
    IEnumerator CDropExp()
    {
        UI.instance.KillCount++;
        Instantiate(expPrefab, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }    
    void DropExp()
    {
        Instantiate(expPrefab,transform.position, Quaternion.identity);
    }
}
