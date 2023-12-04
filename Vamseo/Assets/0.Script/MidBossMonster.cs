using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MidBossMonster : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject expPrefab;
    [SerializeField] private GameObject magPrefab;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private CapsuleCollider2D capsuleCollider2d;
    [SerializeField] private BoxCollider2D boxCollider2d;
    public Player p;
    public Bullet bullet;
    public float hp;
    protected float atkTime = 2f;
    protected int power = 6;
    private float atkTimer;
    private float hitFreezeTimer;
    private float speed = 1f; // 움직이는 속도
    private bool isRangedAttackCooldown = false;
    private float rangedAttackCooldown = 5f;
    public float bulletSpeed;

    // Start is called before the first frame update
    public void Start()
    {
        p = GameObject.Find("[[Player]]").GetComponent<Player>();
        hp = 500;
        bullet.GetComponent<Bullet>().speed = bulletSpeed;
    }
    // Update is called once per frame
    protected void Update()
    {
        
        if (UI.instance.gameState != GameState.Play)
            return;
        if (p == null || hp < 0)
            return;
        if (hitFreezeTimer > 0)
        {
            hitFreezeTimer -= Time.deltaTime;
            return;
        }

        float x = p.transform.position.x - transform.position.x;

        sr.flipX = x < 0 ? true : x == 0 ? true : false;

        float distance = Vector2.Distance(p.transform.position, transform.position);

        if (distance > 1)
        {
            transform.position = Vector2.MoveTowards(transform.position, p.transform.position, speed * Time.deltaTime);
        }
        if (distance <= 10 && !isRangedAttackCooldown) //원거리 공격
        {
            int randomAttack = Random.Range(0, 3); // 변경: 0 or 1 or 2
            if (randomAttack == 0)
            {
                StartCoroutine(Shoot360Degrees());
            }
            else
            {
                StartCoroutine(ShootAtPlayer());
            }
            isRangedAttackCooldown = true;
            StartCoroutine(RangedAttackCooldownRoutine());
        }
        else if (distance <= 1) //
        {
            atkTimer += Time.deltaTime;

            if (atkTimer > atkTime)
            {
                atkTimer = 0;
                p.Hit(power);
            }
        }
    }
    public void SetPlayer(Player p)
    {
        this.p = p;
    }
    public void OnTriggerEnter2D(Collider2D collision) // 보스한테 총알 맞출 때 피 닳는 방식
    {
        if (boxCollider2d.enabled)
            return;
        if (collision.gameObject.tag == "PlayerBullet" && collision.gameObject.layer == LayerMask.NameToLayer("PlayerBullet"))
        {
            collision.GetComponent<Bullet>().HitCount++;
            if (collision.GetComponent<Bullet>().HitCount >= collision.GetComponent<Bullet>().HitMaxCount)
            {
                Destroy(collision.gameObject);
            }
            Dead(0.3f, 80);
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
            animator.SetBool("MidBossDead", true);
            StartCoroutine("CDropExp");
            if (UnityEngine.Random.value < 0.05)
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
        Instantiate(expPrefab, transform.position, Quaternion.identity);
    }
    IEnumerator RangedAttackCooldownRoutine()
    {
        yield return new WaitForSeconds(rangedAttackCooldown);
        isRangedAttackCooldown = false;
    }
    IEnumerator ShootAtPlayer()
    {
        for (int i = 0; i < 31; i++)
        {
            if (hp <= 0) 
            {
                yield break;
            }
            Vector2 bulletDirection = (p.transform.position - transform.position).normalized;
            float angle = Mathf.Atan2(bulletDirection.y, bulletDirection.x) * Mathf.Rad2Deg - 90; // 90도 회전
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, angle));
            bullet.GetComponent<Rigidbody2D>().velocity = bulletDirection * bulletSpeed;
            yield return new WaitForSeconds(0.1f);
        }
    }
    IEnumerator Shoot360Degrees()
    {
        for (int i = 0; i < 360; i += 15) // 매 15도 마다
        {
            float angle = i * Mathf.Deg2Rad;
            Vector2 bulletDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, i));
            bullet.GetComponent<Rigidbody2D>().velocity = bulletDirection * bulletSpeed;
        }
        yield return null;
    }
}
