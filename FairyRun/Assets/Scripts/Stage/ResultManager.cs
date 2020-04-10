using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultManager : MonoBehaviour
{
    // 全ステージ数
    private const int ALLSTAGECOUNT = 3;
    // ステージクリアタイムに応じてのボーナスポイント
    private const int RANK_A_POINT = 1000;// 1000
    private const int RANK_B_POINT = 600;// 600
    private const int RANK_C_POINT = 300;// 300
    private const int RANK_D_POINT = 0;

    private float[,] TIMERUNK =
    {
        // Aランク , Bランク , Cランク
        {100.0f, 110.0f, 150.0f}, // ステージ1
        {180.0f, 190.0f, 240.0f}, // ステージ2
        {170.0f, 180.0f, 230.0f}, // ステージ3
    };
    // ステージクリアタイムのランク別階層タイム
    private const float RANK_A_TIME = 30;
    private const float RANK_B_TIME = 40;
    private const float RANK_C_TIME = 50;
    // private const float RANK_D_TIME = 60;
    // 敵倒した数
    private int enemyDefeatedCount = 0;
    public int EnemyDefeatedCount
    {
        get { return enemyDefeatedCount; }
    }
    // 獲得ポイント
    private int getScorePoint = 0;
    public int GetScorePoint
    {
        get { return getScorePoint; }
    }
    // クリアタイム
    private float clearTime = 0.0f;
    public float ClearTime
    {
        get { return clearTime; }
    }
    // ボーナス加算ポイント
    private int bonusPoint = 0;
    // 現ステージ番号
    private int stageNum = -1;
    public int StageNum
    {
        get { return stageNum; }
    }
    // 総合獲得ポイント
    private int allGetPoint = 0;
    public int AllScorePoint
    {
        get { return allGetPoint; }
    }
    public void SetResultData(int stageNum, float clearTime, int enemyDefeatedCount, int getScorePoint)
    {
        this.stageNum = stageNum;
        this.clearTime = clearTime;
        this.enemyDefeatedCount = enemyDefeatedCount;
        this.getScorePoint = getScorePoint;

        // ---*** ランク決め ***---
        if (clearTime <= TIMERUNK[stageNum,0])
        {
            // ランクA
            Debug.Log("Aランク");
            bonusPoint = RANK_A_POINT;
        }
        else if (clearTime <= TIMERUNK[stageNum, 1])
        {
            // ランクB
            Debug.Log("Bランク");
            bonusPoint = RANK_B_POINT;
        }
        else if (clearTime <= TIMERUNK[stageNum, 2])
        {
            // ランクC
            Debug.Log("Cランク");
            bonusPoint = RANK_C_POINT;
        }
        else
        {
            // ランクD
            Debug.Log("Dランク");
            bonusPoint = RANK_D_POINT;
        }
        allGetPoint = getScorePoint + bonusPoint;
    }
}
