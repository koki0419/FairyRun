using UnityEngine;
using System;

[CreateAssetMenu(menuName = "MyScriptable/Create Map")]
public class CreateMapSubstance : ScriptableObject
{
    public StageData stageData = null;
    public int stageNum = 0;
}
