using System.Collections;
using UnityEngine;

public class MidBossDash : MonoBehaviour
{
    [SerializeField] BoxCollider2D boxCollider2D;
    [SerializeField] MidBossMonster midBossMonster;
    public float ghostDelay;
    private float ghostDelayTime;
    public GameObject ghostPrefab;
    public bool makeGhost;
    private Rigidbody2D rBody2d;
    private float dashDistance = 5f;
    private float dashCooldown = 5f;
    private float lastDashTime;

    void Start()
    {
        boxCollider2D.enabled = true;
        ghostDelayTime = ghostDelay;
        rBody2d = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        Debug.Log(boxCollider2D.enabled);
    }
    public IEnumerator DashAndCreateGhost(Vector2 dashDirection)
    {
        if (midBossMonster.hp <= 0)
        {
            yield break;
        }
        Vector2 dashForce = dashDirection * dashDistance / 0.5f; 

        makeGhost = true;
        rBody2d.AddForce(dashForce, ForceMode2D.Impulse); 
        yield return new WaitForSeconds(0.5f);
        if (rBody2d != null)
        {
            rBody2d.velocity = Vector2.zero;
        }
        else
        {
            yield break;
        }
        makeGhost = false;
        boxCollider2D.enabled = false;
        StartCoroutine(Timer(3f));
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!boxCollider2D.enabled)
            return;
        if (collision.gameObject.tag == "PlayerBullet" && collision.gameObject.layer == LayerMask.NameToLayer("PlayerBullet"))
        {
            Vector3 bulletDirection = collision.transform.position - transform.position;
            bulletDirection.Normalize();
            if (Random.value < 0.5f)
            {
                bulletDirection *= -1;
            }
            StartCoroutine(DashAndCreateGhost(Vector2.Perpendicular(bulletDirection)));
        }
    }
    void FixedUpdate()
    {
        if (makeGhost && ghostDelayTime <= 0)
        {
            GameObject currentGhost = Instantiate(ghostPrefab, transform.position, transform.rotation);
            Sprite currentSprite = GetComponent<SpriteRenderer>().sprite;
            currentGhost.transform.localScale = transform.localScale;
            currentGhost.GetComponent<SpriteRenderer>().sprite = currentSprite;
            ghostDelayTime = ghostDelay;
            Destroy(currentGhost, 0.3f);
        }
        ghostDelayTime -= Time.deltaTime;
    }
    IEnumerator Timer(float delay)
    {
        yield return new WaitForSeconds(delay);
        boxCollider2D.enabled = true;
    }
}
