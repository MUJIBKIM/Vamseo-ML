using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MonsterSpawnController : MonoBehaviour
{
    [SerializeField] private Player p;
    [SerializeField] private Monster monster;
    [SerializeField] private Transform parent;
    [SerializeField] private BoxCollider2D[] boxColls;
    [SerializeField] private GameObject MidBossPrefab;
    [SerializeField] private RangedMonster rangedMonster;
    IEnumerator createMonster;
    int range = 10;
    void Awake()
    {
        createMonster = CreateMonster(5f);
        StartCoroutine(createMonster);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            Instantiate(MidBossPrefab);
        }
    }
    IEnumerator CreateMonster(float time)
    {
        while (true)
        {
            yield return new WaitForSeconds(time);
            int rand = Random.Range(0, boxColls.Length);
            Vector2 v = RandomPosition(rand);
            float spawnProbability = Random.Range(0f, 1f);
            Monster spawnedMonster;

            if (spawnProbability <= 0.05f) // n% 확률로 원거리 몹 생성
            {
                spawnedMonster = Instantiate(rangedMonster, v, Quaternion.identity);
            }
            else 
            {
                spawnedMonster = Instantiate(monster, v, Quaternion.identity);
            }

            spawnedMonster.SetPlayer(p);
            spawnedMonster.transform.SetParent(parent);
        }
    }
    Vector2 RandomPosition(int index)
    {

        RectTransform pos = boxColls[index].GetComponent<RectTransform>();

        Vector3 randPos = Vector3.zero;
        // Top = 0 , Bottom = 1
        if (index == 0 || index == 1)
        {
            randPos = new Vector2(pos.position.x + Random.Range(-range, range), pos.position.y);
        }
        // 나머지
        else
        {
            randPos = new Vector2(pos.position.x, pos.position.y + Random.Range(-range, range));
        }

        return randPos;
    }
    public void StartSpawn(bool start)
    {
        if (start)
            StartCoroutine(createMonster);
        else
            StopCoroutine(createMonster);
    }
}
