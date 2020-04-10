using System;
using UnityEngine;

[Serializable]
public class Item_Bool : ScriptableObject
{
    [Tooltip("アイテム名"), Space(15)]
    public string itemName;
    public bool itemEffect;
    [TextArea(3, 10), Tooltip("説明文"), Space(15)]
    public string message;
}
