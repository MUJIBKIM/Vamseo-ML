using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public enum GameState { Play, Pause, Stop }
[System.Serializable]
public class UpgradeUI
{
    public Image icon;
    public TMP_Text levelTxt;
    public TMP_Text title;
    public TMP_Text description1;
    public TMP_Text description2;
}
[System.Serializable]
public class UpgradeData
{
    public Sprite sprite;
    public string title;
    public string description1;
    public string description2;
}
public class UI : MonoBehaviour
{
    public static UI instance;
    [HideInInspector] public GameState gameState = GameState.Stop;
    [SerializeField] private UpgradeUI[] upUI;
    [SerializeField] public UpgradeData[] upData;
    [SerializeField] private BoxCollider2D[] boxColls;
    [SerializeField] private Slider sliderExp;
    [SerializeField] private Text txtTime;
    [SerializeField] private Text txtKillCount;
    [SerializeField] private Text txtLv;
    [SerializeField] private RectTransform canvas;
    [SerializeField] private Transform levelUpPopup;
    [SerializeField] private Image hpimg;
    [SerializeField] MonsterSpawnController monsterSpawnController;
    [SerializeField] private Player p;
    [SerializeField] private Bullet bullet;
    public GameObject coinDetector;
    public bool isLevelUpPopupActive = false;
    private float maxExp;
    private float exp;
    private int level = 0;
    private float timer = 0;
    private int killCount = 0;
    private float[] exps;
    private List<UpgradeData> upgradeDatas = new List<UpgradeData>();
    private Dictionary<string, (int current, int max)> upgradeCounters = new Dictionary<string, (int, int)> //현재업그레이드/최대업그레이드 관리
    {
        {"Mag", (0, 5)},//아이템 획득반경 최초0 최대5
        {"Select 7", (0, 3)},//이동속도 최초0 최대3
    };

    public void Awake()
    {
        instance = this;
        exps = new float[100];
        for (int i = 0; i < exps.Length; i++)
        {
            exps[i] = 10 * i;
        }
    }
    public float Exp
    {
        get { return exp; }
        set
        {
            exp = value;
            sliderExp.value = exp / maxExp;

            if (exp >= maxExp)
            {
                SetUpgradeData();
                AudioManager.instance.Play("levelup");
                gameState = GameState.Pause;
                levelUpPopup.gameObject.SetActive(true);
                isLevelUpPopupActive = true;
                level++;
                maxExp = exps[level];
                sliderExp.value = 0f;
                exp = 0f;

                txtLv.text = $"Lv.{level + 1}";
            }
        }
    }
    public int KillCount
    {
        get { return killCount; }
        set
        {
            killCount = value;
            txtKillCount.text = $"{killCount.ToString("000")}";
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        OnGameStart();
        maxExp = exps[level];
        sliderExp.value = 0f;
        for (int i = 0; i < boxColls.Length; i++)
        {
            Vector2 v1 = canvas.sizeDelta;
            if (i < 2)
                v1.y = 5;
            else
                v1.x = 5;
            boxColls[i].size = v1;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            gameState = GameState.Play;
        }
        if (Input.GetKey(KeyCode.F1))
        {
            Exp += 1f;
        }
        if (gameState != GameState.Play)
        {
            monsterSpawnController.StartSpawn(false);
            return;
        }

        timer += Time.deltaTime;
        System.TimeSpan ts = System.TimeSpan.FromSeconds(timer);
        txtTime.text = string.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds);
    }
    public void SetHP(int HP, int maxHP)
    {
        hpimg.fillAmount = (float)HP / maxHP;
    }
    public void OnGameStart()
    {
        monsterSpawnController.StartSpawn(true);
        gameState = GameState.Play;
    }
    void SetUpgradeData()
    {
        List<UpgradeData> datas = new List<UpgradeData>(upData);

        // 최대 업그레이드 회수에 도달한 항목 배제
        foreach (var entry in upgradeCounters)
        {
            if (entry.Value.current >= entry.Value.max)
            {
                datas.RemoveAll(data => data.sprite.name == entry.Key);
            }
        }

        // 업그레이드 데이터 랜덤 설정
        upgradeDatas.Clear();
        for (int i = 0; i < 3; i++)
        {
            if (datas.Count == 0) break;  // 더 이상 선택할 수 있는 업그레이드가 없으면 루프 종료

            int rand = UnityEngine.Random.Range(0, datas.Count);
            upgradeDatas.Add(datas[rand]);
            datas.RemoveAt(rand);
        }

        // UI 업데이트
        for (int i = 0; i < upgradeDatas.Count; i++)
        {
            upUI[i].icon.sprite = upgradeDatas[i].sprite;
            upUI[i].title.text = upgradeDatas[i].title;
            upUI[i].description1.text = upgradeDatas[i].description1;
            upUI[i].description2.text = upgradeDatas[i].description2;
        }
    }
    public void OnUpgrade(int index)
    {
        string upgradeName = upgradeDatas[index].sprite.name;
        switch (upgradeName)
        {
            case "Mag":
                GameObject coinDetector = GameObject.Find("CoinDetector");
                coinDetector.transform.localScale = new Vector3(coinDetector.transform.localScale.x * 1.5f, coinDetector.transform.localScale.y * 1.5f, coinDetector.transform.localScale.z * 1.5f);
                UpgradeCounter(upgradeName);
                break;
            case "Select 5":
                p.BulletHitMaxCount++;
                break;
            case "Select 7":
                p.Speed += 0.5f;
                UpgradeCounter(upgradeName);
                break;
            case "Select 8":
                p.HP = p.MaxHP;
                SetHP(p.HP, p.MaxHP);
                break;
        }
        levelUpPopup.gameObject.SetActive(false);
        isLevelUpPopupActive = false;
    }
    // 업그레이드 카운터 증가 함수  
    void UpgradeCounter(string upgradeName)
    {
        if (upgradeCounters.ContainsKey(upgradeName))
        {
            var currentValue = upgradeCounters[upgradeName];
            upgradeCounters[upgradeName] = (currentValue.current + 1, currentValue.max);
        }
    }
}
