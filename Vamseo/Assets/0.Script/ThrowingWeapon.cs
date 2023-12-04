using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingWeapon : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 100;
    public float lifeTime = 5f;
    public float maxThrowDistance = 10f;
    public GameObject explosionEffectPrefab;
    public float explosionDelay = 2f;
    public float explosionRadius = 10f;
    private Vector3 direction;
    private Player player;
    public Sprite throwingWeaponSprite;
    public GameObject throwingObjectPrefab;

    private bool isDestroyed = false;  //잘되나확인용

    private void Start()
    {
        player = FindObjectOfType<Player>();
    }

    private void Update()
    {
        if (UI.instance.gameState == GameState.Play)
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                ThrowWeapon();
            }

            transform.Translate(direction * speed * Time.deltaTime);
        }
    }

    private void ThrowWeapon()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        Vector3 throwDirection = (mousePos - player.transform.position).normalized;
        float distanceToMouse = Vector3.Distance(player.transform.position, mousePos);

        if (distanceToMouse > maxThrowDistance)
        {
            mousePos = player.transform.position + throwDirection * maxThrowDistance;
        }

        direction = (mousePos - player.transform.position).normalized;

        ThrowingWeapon grenade = Instantiate(throwingObjectPrefab, player.transform.position, Quaternion.identity).GetComponent<ThrowingWeapon>();
        grenade.SetDirection(direction);
        StartCoroutine(grenade.ExplodeAfterDelay(explosionDelay));
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir;
    }

    public IEnumerator ExplodeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (isDestroyed)
            yield break;

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hitCollider in hitColliders)
        {
            Monster monster = hitCollider.GetComponent<Monster>();
            if (monster)
            {
                monster.Dead(0.5f, damage);
            }
        }

        Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        if (!isDestroyed)
        {
            isDestroyed = true;
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Monster monster = collision.GetComponent<Monster>();
        if (monster)
        {
            monster.Dead(0.5f, damage);
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            if (!isDestroyed)
            {
                isDestroyed = true;
                Destroy(gameObject);
            }
        }
    }
}