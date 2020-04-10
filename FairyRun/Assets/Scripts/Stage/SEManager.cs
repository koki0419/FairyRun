using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SEManager : MonoBehaviour
{
    public enum SEName
    {
        SELECT, //決定
        JUMP,   //ジャンプ
        ATTACK, //妖精の攻撃
        HIT,    //敵の攻撃が妖精にヒット
        ITEM,   //アイテム取得時
        CLEAR,  //ステージクリア時
        KEY,    //南京錠破壊時
    }

    [SerializeField, Header("再生するSE")]
    AudioClip[] se;

    List<AudioSource> seObjects = new List<AudioSource>();

    private void Update()
    {
        //生成したSE再生用オブジェクトの中で、SEの再生が終わったものを破壊
        for (int i = 0; i < seObjects.Count;i++)
        {
            if(!seObjects[i].isPlaying)
            {
                Destroy(seObjects[i].gameObject);
                seObjects.RemoveAt(i);
            }
        }
    }

    public void PlaySound(SEName seName)
    {
        //オブジェクトを生成し、子にする
        GameObject audio = new GameObject("SESource");
        audio.transform.parent = transform;

        //AudioSourceを追加
        AudioSource audioSource = audio.AddComponent<AudioSource>();

        //どのSEを再生するか
        switch(seName)
        {
            case SEName.SELECT: audioSource.clip = se[0];break;
            case SEName.JUMP: audioSource.clip = se[1];break;
            case SEName.ATTACK: audioSource.clip = se[2];break;
            case SEName.HIT: audioSource.clip = se[3];break;
            case SEName.ITEM: audioSource.clip = se[4];break;
            case SEName.CLEAR: audioSource.clip = se[5];break;
            case SEName.KEY: audioSource.clip = se[6];break;
        }

        //再生
        audioSource.Play();

        //リスト追加
        seObjects.Add(audioSource);
    }
}
