using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataMng : MonoBehaviour
{
    public static GameDataMng Instance;

    public int userHp =0;
    public int userDef =0;
    public int userSpeed =0;
    public int userReloadspeed =0;


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
