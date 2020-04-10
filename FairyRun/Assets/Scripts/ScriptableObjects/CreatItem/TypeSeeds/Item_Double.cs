using System;
using UnityEngine;

[Serializable]
public class Item_Double : ScriptableObject
{
    [Tooltip("アイテム名"), Space(15)]
    public string itemName;
    [Tooltip("Double型数値入力欄"), Space(15)]
    public double itemEffect;
    [TextArea(3, 10), Tooltip("説明文"), Space(15)]
    public string message;
}