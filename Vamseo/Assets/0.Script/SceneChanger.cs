using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SceneChanger : MonoBehaviour
{
    public Image mainimage;
    public Sprite nextsprite;
    public Button upgradebtn;

    public void Start()
    {
        
    }
    public void OnClick()
    {
        Debug.Log("btn clicked");
        
        switch(upgradebtn.name)
        {
            case "Main weapon btn":
                Debug.Log("Main weapon btn");
                mainimage.GetComponent<Image>().sprite = nextsprite;
                break;
            case "Hero btn":
                Debug.Log("Hero btn");
                mainimage.GetComponent<Image>().sprite = nextsprite;
                break;
            case "Melee weapon btn":
                Debug.Log("Melee weapon btn");
                break;
            case "Dron btn":
                Debug.Log("Dron btn");
                break;
            case "Back btn":
                Debug.Log("Dron btn");
                SceneManager.LoadScene("Main");
                break;
        }
        
    }
}
