using System.Collections;
using System.Security.Cryptography;
using UnityEngine;

public class RangedMonster : Monster
{
    [SerializeField] private GameObject bulletPrefab; 
    [SerializeField] private Animator rangedZombieAnimator;
    private float shootRange = 12f; 
    private float shootInterval = 5f; 
    private bool isRangedAttackCooldown = false;
    public float bulletSpeed = 0.1f;

    protected override void Update()
    {
        base.Update(); 

        float distance = Vector2.Distance(p.transform.position, transform.position);

        if (distance <= shootRange && !isRangedAttackCooldown)
        {
            StartCoroutine(ShootAtPlayer());
            isRangedAttackCooldown = true;
            StartCoroutine(RangedAttackCooldownRoutine());
        }
    }
    public override void Dead(float freezeTime, int damage)
    {
        base.Dead(1f, 100);
        if (hp <= 0)
        {
            animator.SetBool("Dead", false);
            animator.SetBool("RangedZombieDead", true);
        }
    }
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }
    public override void SetPlayer(Player p)
    {
        base.SetPlayer(p);
    }
    IEnumerator ShootAtPlayer()
    {
        for (int i = 0; i < 1; i++) // 필요에 따라 수정 가능
        {
            if (hp <= 0)
            {
                yield break;
            }
            Vector2 bulletDirection = (p.transform.position - transform.position).normalized;
            float angle = Mathf.Atan2(bulletDirection.y, bulletDirection.x) * Mathf.Rad2Deg - 90; 
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, angle));
            bullet.GetComponent<Rigidbody2D>().velocity = bulletDirection * bulletSpeed* 0.05f;
            yield return new WaitForSeconds(3f); 
        }
    }
    IEnumerator RangedAttackCooldownRoutine()
    {
        yield return new WaitForSeconds(shootInterval);
        isRangedAttackCooldown = false;
    }
}
