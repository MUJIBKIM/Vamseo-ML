using System.Collections;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    public float ghostDelay;
    private float ghostDelayTime;
    public GameObject ghostPrefab;
    public bool makeGhost;

    private Rigidbody2D rBody2d;
    private float dashDistance = 5f;

    void Start()
    {
        ghostDelayTime = ghostDelay;
        rBody2d = GetComponent<Rigidbody2D>();
    }

    public IEnumerator DashAndCreateGhost(Vector2 dashDirection)
    {
        Vector2 dashTargetPosition = (Vector2)transform.position + dashDirection * dashDistance;

        makeGhost = true;
        float timeElapsed = 0f;
        float dashDuration = 1;

        while (timeElapsed < dashDuration)
        {
            transform.position = Vector2.Lerp(transform.position, dashTargetPosition, timeElapsed / dashDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        makeGhost = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !makeGhost)
        {
            Vector2 dashDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            if (dashDirection != Vector2.zero)
            {
                StartCoroutine(DashAndCreateGhost(dashDirection));
            }
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
}
