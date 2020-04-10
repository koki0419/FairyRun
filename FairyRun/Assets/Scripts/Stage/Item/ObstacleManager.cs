using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectPool))]
public class ObstacleManager : MonoBehaviour
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
    private Obstacles obstaclesData;
    // Pool本体を取得
    private ObjectPool pool;
    // 表示されている数をカウントする
    private int drawCount = 0;

    // ******* Resourceからロードするときの名前 **********
    private const string LOAD_FILEPATH = "Datas/obstacleData/";
    private const string LOAD_GAMEOBJECT_SPAWNNAME = "Obstacle Variant";
    private const string LOAD_OBSTACLES_DATANAME = "ObstaclesPosData_";
    private void Awake()
    {
        var stageNum = RunGame.Stage.SceneController.StageNo + 1;
        obstaclesData = Resources.Load(LOAD_FILEPATH + LOAD_OBSTACLES_DATANAME + stageNum.ToString()) as Obstacles;
        spawnObj = Resources.Load(LOAD_GAMEOBJECT_SPAWNNAME) as GameObject;
        // デバック用
        var count = obstaclesData.ObstacleList.Count;
        spawnCount = count;
        pool = GetComponent<ObjectPool>();
        pool.CreatePool(spawnObj, spawnCount);
    }
    // Start is called before the first frame update
    void Start()
    {
        var obstacleDatas = this.obstaclesData.ObstacleList;
        //for (int index = 0; index < drawObjCount; index++)
        for (int index = 0; index < obstacleDatas.Count; index++)
        {
            var getObj = pool.GetPoolObj();
            // var fairyItem = getObj.GetComponent<FairyItem>();
            // fairyItem.Initialize(itemDatas[index]);
            var setPos = obstacleDatas[index].position;
            var setRotate = obstacleDatas[index].rotate;
            var pos = getObj.transform.position;
            pos.x = setPos.x;
            pos.y = 0;
            pos.z = setPos.z;
            getObj.transform.position = pos;
            getObj.transform.Rotate(0, setRotate, 0);
            drawCount++;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
