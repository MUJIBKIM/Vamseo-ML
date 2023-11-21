using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MeleeAtk : MonoBehaviour
{
    [SerializeField] private Image coolDownTimer;
    private BoxCollider2D boxCollider;
    private float cooldownTimer;
    public float cooldown = 60f;
    public GameObject player;
    private RectTransform rect;
    public GameObject skillEffect;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.enabled = false;
        rect = coolDownTimer.GetComponent<RectTransform>();
        skillEffect.SetActive(false);
    }

    void Update()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        coolDownTimer.fillAmount = 1 - (cooldownTimer / cooldown);

        if (Input.GetKeyDown(KeyCode.LeftShift) && cooldownTimer <= 0)
        {
            cooldownTimer = cooldown;
            boxCollider.enabled = true;
            skillEffect.SetActive(true);
        }
    }
    void FixedUpdate()
    {
        if (player != null)  // 플레이어가 할당되어 있을 때만 실행
        {
            Vector3 newPosition = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
            rect.position = newPosition;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Monster"))
        {
            collision.GetComponent<Monster>().Dead(1f, 100);
            boxCollider.enabled = false;
        }
    }
}
