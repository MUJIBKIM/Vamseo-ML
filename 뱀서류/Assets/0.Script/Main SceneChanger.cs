using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainSceneChanger : MonoBehaviour
{
    public Button btn;

    public void Onclick()
    {
        switch (btn.name)
        {
            case "start btn":
                Debug.Log("start btn");
                SceneManager.LoadScene("Game");
                break;
            case "upgrade btn":
                Debug.Log("upgrade btn");
                SceneManager.LoadScene("Upgrade");
                break;
            
        }
    }
}
