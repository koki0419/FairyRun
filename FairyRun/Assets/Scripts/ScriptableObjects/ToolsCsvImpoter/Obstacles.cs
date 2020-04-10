using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ObstacleEntry
{
    public int id;
    public string obstacleName;
    public Vector3 position;
    public float rotate;
}

public class Obstacles : ScriptableObject
{
    public List<ObstacleEntry> ObstacleList;
}
