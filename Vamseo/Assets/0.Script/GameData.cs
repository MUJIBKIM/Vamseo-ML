using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Status", menuName = "Scriptable Object/GameData")]
public class GameData : ScriptableObject
{
    public enum ItemType { Ammo, Damage, Stun, Reloadtime, None }
    public enum StatType { Hp, Def, Speed, Reloadspeed, None }
        
    /*[Header("#Weapon Info")]
    public ItemType itemType;
    public int itemId;
    public string itemName;
    public string itemDesc;
    public Sprite itemIcon;*/

    [Header("#User Data")]
    public StatType statType;
    public int baseValue;
    public int count;
    
    /*public float baseDamage;
    public int baseCount;
    public float[] damages;
    public int[] counts;*/

    /*public int userHp;
    public int userDef;
    public int userSpeed;
    public int userReloadspeed;*/

    [Header("#Weapon")]
    public GameObject projectile;
}