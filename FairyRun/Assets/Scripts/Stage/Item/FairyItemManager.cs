using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectPool))]
public class FairyItemManager : MonoBehaviour
{
    // pool内に初期生成数
    [SerializeField]
    private int spawnCount = 0;
    // 生成対象のオブジェクトを取得
    private GameObject spawnObj = null;
    // 画面に表示させるオブジェクト
    [SerializeField]
    private int drawObjCount = 0;
    // アイテムデータを取得
    private Items itemsData;
    // Pool本体を取得
    private ObjectPool pool;
    // 表示されている数をカウントする
    private int drawCount = 0;

    // ******* Resourceからロードするときの名前 **********
    private const string LOAD_FILEPATH = "Datas/fairyItemData/";
    private const string LOAD_GAMEOBJECT_SPAWNNAME = "FairyItem";
    private const string LOAD_ITEMS_NAME = "FairyItemPosData_";
    // Start is called before the first frame update
    private void Awake()
    {
        var stageNum = RunGame.Stage.SceneController.StageNo + 1;
        itemsData = Resources.Load(LOAD_FILEPATH + LOAD_ITEMS_NAME + stageNum.ToString()) as Items;
        spawnObj = Resources.Load(LOAD_GAMEOBJECT_SPAWNNAME) as GameObject;
        // デバック用
        var count = itemsData.itemList.Count;
        spawnCount = count;
        pool = GetComponent<ObjectPool>();
        pool.CreatePool(spawnObj, spawnCount);
    }
    void Start()
    {
        var itemDatas = this.itemsData.itemList;
        //for (int index = 0; index < drawObjCount; index++)
        for (int index = 0; index < itemDatas.Count; index++)
        {
            var getObj = pool.GetPoolObj();
            var fairyItem = getObj.GetComponent<FairyItem>();
            fairyItem.Initialize(itemDatas[index]);
            var setPos = itemDatas[index].position;
            var setRotate = itemDatas[index].rotate;
            var pos = getObj.transform.position;
            pos.x = setPos.x;
            pos.y = 1; ;
            pos.z = setPos.z;
            getObj.transform.position = pos;
            getObj.transform.Rotate(0, setRotate, 0);
            drawCount++;
        }
    }
}
