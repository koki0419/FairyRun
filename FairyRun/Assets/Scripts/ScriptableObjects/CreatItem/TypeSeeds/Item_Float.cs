using System;
using UnityEngine;

[Serializable]
public class Item_Float : ScriptableObject
{
    [Tooltip("アイテム名"), Space(15)]
    public string itemName;
    [Tooltip("Float型数値入力欄"), Space(15)]
    public float itemEffect;
    [TextArea(3, 10), Tooltip("説明文"), Space(15)]
    public string message;
}
