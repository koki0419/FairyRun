using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyEntry
{
    public int id;
    public string enemyName;
    public Vector3 position;
    public float rotate;
}

public class Enemys : ScriptableObject
{
    public List<EnemyEntry> enemyList;
}
