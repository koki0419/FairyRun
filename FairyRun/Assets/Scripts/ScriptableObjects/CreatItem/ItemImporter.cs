using System;
using UnityEngine;

[CreateAssetMenu(menuName = "MyScriptable/Create ItemImporter")]
public class ItemImporter : ScriptableObject
{
    [Tooltip("アイテム名"), Space(15)]
    public string itemName = null;
    public ValueType type;
    [TextArea(3, 10), Tooltip("説明文"), Space(15)]
    public string message = null;
}
[Serializable]
public enum ValueType
{
    BOOL,
    STRING,
    INT,
    FLOAT,
    DOUBLE,
}
