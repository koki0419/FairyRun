using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectPool))]
public class ScoreItemManager : MonoBehaviour
{
    [SerializeField]
    private int spawnCount;
    private GameObject spawnObj_0 = null;
    private GameObject spawnObj_1 = null;
    private GameObject spawnObj_2 = null;
    [SerializeField]
    private int drawObjCount;
    private ObjectPool pool;
    private ObjectPool pool_1;
    private ObjectPool pool_2;
    // スコアポイント
    // 0 100p
    // 1  50p
    // 2  30p
    [SerializeField]
    private Item_Int[] item = null;

    [SerializeField]
    private AudioClip soundOnGetScoreItem = null;
    // 再生する「スコアアイテム」獲得seを取得するパス
    private const string LOAD_SE_GETSCOREITEM = "Sound/GetScoreItem";
    private const string LOAD_FILEPATH = "Datas/scoreItem/";
    // スコアアイテム（金）
    private const string LOAD_GAMEOBJECT_SPAWNNAME_0 = "ScoreItem_Gold_Old_Variant";
    // スコアアイテム（銀）
    private const string LOAD_GAMEOBJECT_SPAWNNAME_1 = "ScoreItem_Slver_Old_Variant";
    // スコアアイテム（銅）
    private const string LOAD_GAMEOBJECT_SPAWNNAME_2 = "ScoreItem_Bronze_Old_Variant";
    private const string LOAD_ITEM_INT_SCOREITEMDATANAME = "ScoreItem_";
    // Start is called before the first frame update
    private void Awake()
    {
        soundOnGetScoreItem = Resources.Load(LOAD_SE_GETSCOREITEM) as AudioClip;
        spawnObj_0 = Resources.Load(LOAD_GAMEOBJECT_SPAWNNAME_0) as GameObject;
        spawnObj_1 = Resources.Load(LOAD_GAMEOBJECT_SPAWNNAME_1) as GameObject;
        spawnObj_2 = Resources.Load(LOAD_GAMEOBJECT_SPAWNNAME_2) as GameObject;
        pool = GetComponent<ObjectPool>();
        pool_1 = new ObjectPool();
        pool_2 = new ObjectPool();
        pool.CreatePool(spawnObj_0, spawnCount,this.transform);
        pool_1.CreatePool(spawnObj_1, spawnCount, this.transform);
        pool_2.CreatePool(spawnObj_2, spawnCount, this.transform);
        item = new Item_Int[3];
        for (int i = 0; i < 3; i++)
        {
            item[i] = Resources.Load(LOAD_FILEPATH + LOAD_ITEM_INT_SCOREITEMDATANAME + i.ToString()) as Item_Int;
        }
    }

    public GameObject GetScoreItem(int index)
    {
        GameObject getItem = null;
        switch (index)
        {
            case 0:
                getItem = pool.GetPoolObj(this.transform);
                break;
            case 1:
                getItem = pool_1.GetPoolObj(this.transform);
                break;
            case 2:
                getItem = pool_2.GetPoolObj(this.transform);
                break;
            default:
                break;
        }
        getItem.GetComponent<ScoreItem>().Initialize(item[index], soundOnGetScoreItem);
        return getItem;
    }
}
