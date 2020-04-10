using System;
using UnityEngine;

[CreateAssetMenu(menuName = "MyScriptable/Create Substance")]
public class CreateSubstance : ScriptableObject
{
    public Items itemData;
    public Vector3 errorRange_Item = Vector3.zero;
    public Enemys enemyData;
    public Vector3 errorRange_Enemy = Vector3.zero;
    public Obstacles obstacleData;
    public Vector3 errorRange_Obstacle = Vector3.zero;
    public CreateType type;
    public GameObject[] substanceItemTip = null;
    public GameObject[] substanceEnemyTip = null;
    public GameObject[] substanceObstacleTip = null;
}
[Serializable]
public enum CreateType
{
    ITEM,
    ENEMY,
    OBSTACLE,
}

