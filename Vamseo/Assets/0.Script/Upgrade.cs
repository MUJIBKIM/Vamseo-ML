using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class Upgrade : MonoBehaviour
{
    public GameData data;
    
    public int level;
    public bool isthisdata;

    Image icon;
    Text textLevel;

    void Awake()
    {
        //LoadUserData();
        //icon = GetComponentsInChildren<Image>()[1];
        Text[] texts = GetComponentsInChildren<Text>();
        textLevel = texts[0];
        switch (data.statType)
        {
            case GameData.StatType.Hp:
                level = GameDataMng.Instance.userHp;
                break;
            case GameData.StatType.Def:
                level = GameDataMng.Instance.userDef;
                break;
            case GameData.StatType.Speed:
                level = GameDataMng.Instance.userSpeed;
                break;
            case GameData.StatType.Reloadspeed:
                level = GameDataMng.Instance.userReloadspeed;
                break;
        }

    }

    private void LateUpdate()
    {
        textLevel.text = data.statType + "\nLv." + (level);
    }

    public void OnClick()
    {
        switch (data.statType)
        {
            case GameData.StatType.Hp:
                GameDataMng.Instance.userHp = level + 1;
                break;
            case GameData.StatType.Def:
                GameDataMng.Instance.userDef = level + 1;
                break;
            case GameData.StatType.Speed:
                GameDataMng.Instance.userSpeed = level + 1;
                break;
            case GameData.StatType.Reloadspeed:
                GameDataMng.Instance.userReloadspeed = level + 1;
                break;
        }
        level++;
        //SaveUserData();

        
    }

    public void SaveUserData()
    {
        /*FileStream file = new FileStream(Application.persistentDataPath + "/userdata.dat", FileMode.Create);
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        binaryFormatter.Serialize(file, user);
        file.Close();*/
        
    }

    public void LoadUserData()
    {
        /*if (File.Exists(Application.persistentDataPath + "/userdata.dat"))
        {
            FileStream file = new FileStream(Application.persistentDataPath + "/userdata.dat", FileMode.Open);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            user = (UserData)binaryFormatter.Deserialize(file);
            file.Close();
        }
        else
        {
            user = new UserData();
        }*/
    }
    
}
