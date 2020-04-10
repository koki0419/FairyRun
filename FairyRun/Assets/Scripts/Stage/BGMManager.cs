using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    AudioSource bgmAudio;

    [SerializeField, Header("ステージ毎のBGM")]
    AudioClip[] bgmClip;

    void Start()
    {
        bgmAudio = GetComponent<AudioSource>();
    }

    void Update()
    {
        
    }
}
