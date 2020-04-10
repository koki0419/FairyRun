using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemEntry
{
    public int id;
    public string itemName;
    public int point;
    public Vector3 position;
    public float rotate;
}

public class Items : ScriptableObject
{
    public List<ItemEntry> itemList;
}
