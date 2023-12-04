using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public enum FieldDropItem
{
    ExpCoin,
    Mag,
}
public class Item : MonoBehaviour
{
    protected FieldDropItem type = FieldDropItem.ExpCoin;
    public FieldDropItem dropItem;
    public bool isPickup = false;
    public Player p;
    public GameObject player;
    public GameObject coinDetectorTarget;
    // Start is called before the first frame update
void Start()
{
    coinDetectorTarget = GameObject.Find("CoinDetector");
    player = GameObject.Find("[[Player]]");
    p = player.GetComponent<Player>();
}

    // Update is called once per frame
    void Update()
    {
        if (UI.instance.gameState != GameState.Play)
            return;
        if (isPickup)
        {
            Vector2 v1 = (coinDetectorTarget.transform.position - transform.position).normalized * Time.deltaTime * 7f; //아이템이 플레이어 따라가는 속도
            transform.Translate(v1);
            if (Vector3.Distance(transform.position, coinDetectorTarget.transform.position) < 1f ) 
            {
                if (FieldDropItem.ExpCoin == dropItem)
                {
                    p.GetExp(1);
                }
                else
                {
                    Item[] items = FindObjectsOfType<Item>();
                    foreach (var item in items)
                    {
                        if (item.dropItem == FieldDropItem.Mag)
                            continue;
                        item.coinDetectorTarget = coinDetectorTarget;
                        item.isPickup = true;
                    }
                }
                Destroy(gameObject);
            }
        }
    }
}
   
