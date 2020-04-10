using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreItem : MonoBehaviour
{
    public Item_Int item = null;
    private int point = 0;
    private const string player_LayerName = "Player";
    // サウンドエフェクト再生用のAudioSource
    AudioSource audioSource;
    // スコアアイテムを取得した際のサウンドを指定します。
    private AudioClip soundOnGetScoreItem = null;
    private bool playSe = false;

    public void Initialize(Item_Int item_Int, AudioClip getScore)
    {
        item = item_Int;
        point = item.itemEffect;
        soundOnGetScoreItem = getScore;
        audioSource = GetComponent<AudioSource>();
        GetComponent<MeshRenderer>().enabled = true;
        playSe = false;
    }
    private void Update()
    {
        if (playSe) {
            if (!audioSource.isPlaying)
            {
                gameObject.SetActive(false);
            }
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == player_LayerName)
        {
            RunGame.Stage.SceneController.scoerPoint += point;
            audioSource.clip = soundOnGetScoreItem;
            audioSource.Play();
            playSe = true;
            GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
