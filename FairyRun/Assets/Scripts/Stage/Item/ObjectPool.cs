using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    private GameObject targetObj;
    private int creatPoolObjCount;
    private List<GameObject> pools = new List<GameObject>();
    // Poolを作成
    public void CreatePool(GameObject targetObj, int createCount, Transform parent = null)
    {
        this.targetObj = targetObj;
        for (int index = 0; index < createCount; index++)
        {
            var newObj = CreateNewObj(targetObj);
            pools.Add(newObj);
            if (parent != null)
                newObj.transform.parent = parent;
            else
                newObj.transform.parent = transform;
            newObj.SetActive(false);
        }
    }
    // 新規でオブジェクト作成
    private GameObject CreateNewObj(GameObject targetObj)
    {
        var newObj = Instantiate(targetObj) as GameObject;
        return newObj;
    }
    // poolsのから未使用のObjを呼び出す
    public GameObject GetPoolObj(Transform parent = null)
    {
        foreach (var obj in pools)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }
        var newObj = CreateNewObj(this.targetObj);
        pools.Add(newObj);
        if (parent != null)
            newObj.transform.parent = parent;
        else
            newObj.transform.parent = transform;
        newObj.SetActive(true);
        return newObj;
    }
}
