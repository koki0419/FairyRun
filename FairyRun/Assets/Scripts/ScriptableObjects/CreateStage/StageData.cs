using System;
using UnityEngine;

[Serializable]
public class StageData : ScriptableObject
{
    public GameObject[] TileTips = null;
    public int width = -1;
    public int hight = -1;
    public int[] internalData;
}
