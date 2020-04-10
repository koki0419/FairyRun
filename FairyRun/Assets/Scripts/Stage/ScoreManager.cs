using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// スコアやポイント全般を管理します
/// </summary>
public class ScoreManager : MonoBehaviour
{
    // スコアポイント
    public static int scoerPoint = 0;
    // 妖精攻撃ポイント
    public static int fairyPoint = 0;
    private void Start()
    {
        scoerPoint = 0;
        fairyPoint = 0;
    }
    private void Update()
    {
      //  Debug.Log("scoerPoint : " + scoerPoint);
       // Debug.Log("fairyPoint : " + fairyPoint);
    }
}
