using System;
using UnityEngine;

[Serializable]
public class Item_String : ScriptableObject
{
    [Tooltip("アイテム名")]
    public string itemName;
    [TextArea(3, 10), Tooltip("テキスト入力欄"), Space(15)]
    public string itemEffect;
    [TextArea(3, 10), Tooltip("説明文"), Space(15)]
    public string message;
}
