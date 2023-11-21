using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [System.Serializable]
    public class MyClip
    {
        public string name;
        public AudioClip audioClip;
    }
    [SerializeField] private List<MyClip> myClips;
    [SerializeField] private AudioSource audioSource;
    private void Awake()
    {
        instance = this;
    }
    public void Play(string name)
    {
        foreach (var item in myClips)
        {
            if(item.name == name)
            {
                audioSource.clip = item.audioClip;
                audioSource.Play();
                break;
            }
        }
    }
}