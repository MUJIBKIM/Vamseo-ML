using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataMng : MonoBehaviour
{
    public static GameDataMng Instance;

    public int userHp;
    public int userDef;
    public int userSpeed;
    public int userReloadspeed;


    private void Awake()
    {
        
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);


    }
}
