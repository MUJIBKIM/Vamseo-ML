using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.UIElements;
using UnityEngine.SocialPlatforms;
using Unity.VisualScripting;

public class Firearm : MonoBehaviour
{
    [SerializeField] public Player player;
    [SerializeField] public UI ui;
    [SerializeField] private SpriteRenderer weaponSprite;
    [SerializeField] public UnityEngine.UI.Image reloadUi;
    [SerializeField] private Transform front1Transform;
    public float reloadTime = 2f; //- (GameDataMng.Instance.userReloadspeed/100); // 이후 레벨업 등 스펙업 요소 구현 시 어떻게??
    public GameObject bulletPrefab;
    public Text ammoText;
    public int maxAmmo = 10;
    public int currentAmmo;
    public bool isReloading = false;
    public float recoilDuration = 0.1f;
    public float recoilIntensity = 0.1f;
    public Vector3 originalPosition;
    public bool isRecoiling = false;
    public Vector3 worldOriginalPosition;
    public Cam cam;

    void Start()
    {
        ui = UI.instance;
        currentAmmo = maxAmmo;
        originalPosition = transform.localPosition;
        reloadUi.gameObject.SetActive(false);
    }
    void Update()
    {
        if(front1Transform != null && reloadUi != null)
        {
            Vector3 mp = Input.mousePosition;
            mp.z = -Camera.main.transform.position.z;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mp);
            reloadUi.rectTransform.position = worldPosition;
        }
        worldOriginalPosition = transform.parent.TransformPoint(originalPosition);
        // R키를 누르면 재장전
        if (Input.GetKeyDown(KeyCode.R) && !isReloading && ui.gameState == GameState.Play)
        {
            StartCoroutine(ReloadCoroutine());
        }

        // 좌클릭을 누르면 발사
        if (Input.GetMouseButtonDown(0) && !isReloading && !isRecoiling)
        {
            StartCoroutine(Recoil());
            Fire();
        }
        //마우스 커서 위치에 따라 회전(플레이어의 flip과 동일)
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        Vector2 aimDirection = (mousePosition - player.transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        weaponSprite.transform.rotation = Quaternion.Euler(0, 0, angle);

        if (aimDirection.x < 0) //왼쪽볼때
        {
            weaponSprite.flipY = true;
            weaponSprite.transform.localPosition = new Vector3(0.157f, -0.256f, 0);
        }
        else if (aimDirection.x > 0) //오른쪽볼때
        {
            weaponSprite.flipY = false;
            weaponSprite.transform.localPosition = new Vector3(-0.157f, -0.256f, 0);
        }
    }
    void Fire()// 탄환 프리팹 생성 및 발사
    {
        if(ui.gameState != GameState.Play) //레벨업 해서 팝업창 뜰시 함수 비활성화
        {
            return;
        }
        if (currentAmmo > 0)
        {
            GameObject bullet = Instantiate(bulletPrefab, player.firePos.position, player.firePos.rotation * Quaternion.Euler(0, 0, -90f));
            currentAmmo--;
        }
    }
    IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        StartReloadUI(); // 재장전 UI 시작
        float reloadEndTime = Time.time + reloadTime;
        while (Time.time < reloadEndTime)
        {
            reloadUi.fillAmount = (Time.time - (reloadEndTime - reloadTime)) / reloadTime;
            yield return null;
        }
        Reload();
        StopReloadUI(); // 재장전 UI 종료
        isReloading = false;
    }
    void Reload()
    {
        currentAmmo = maxAmmo;
    }
    private IEnumerator Recoil()
    {
        isRecoiling = true;

        float startTime = Time.time;
        float endTime = startTime + recoilDuration;

        Vector3 initialPosition = weaponSprite.transform.localPosition;
        Vector3 targetRecoilPosition = initialPosition + new Vector3(0, -recoilIntensity, 0);

        while (Time.time < endTime)
        {
            float t = (Time.time - startTime) / recoilDuration;
            weaponSprite.transform.localPosition = Vector3.Lerp(initialPosition, targetRecoilPosition, t);
            yield return null;
        }

        weaponSprite.transform.localPosition = initialPosition;
        isRecoiling = false;
    }
    public void StartReloadUI()
    {
        reloadUi.gameObject.SetActive(true);
        reloadUi.fillAmount = 0f;
    }
    public void StopReloadUI()
    {
        reloadUi.gameObject.SetActive(false);
    }
}
