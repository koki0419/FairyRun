using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RunGame.Stage;

[RequireComponent(typeof(ObjectPool))]
public class EnemyManager : MonoBehaviour
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
    private Enemys enemysData;
    // Pool本体を取得
    private ObjectPool pool;
    // 表示されている数をカウントする
    private int drawCount = 0;
    private ScoreItemManager scoreItemManager = null;
    private SceneController sceneController = null;
    [SerializeField, Tooltip("敵の高さ補正")]
    private float enemyPosY;
    [SerializeField, Header("攻撃高さ")]
    private float attackPos_y = 2;
    [SerializeField, Header("効果音")]
    private AudioClip attack;
    [SerializeField]
    private AudioClip hit;

    // ******* Resourceからロードするときの名前 **********
    private const string LOAD_FILEPATH = "Datas/enemyData/";
    private const string LOAD_GAMEOBJECT_SPAWNNAME = "Enemy";
    private const string LOAD_ENEMYS_NAME = "EnemyPosData_";
    private const string LOAD_SCOREITEMMANAGER_NAME = "ScoreItemManager";
    private void Awake()
    {
        var stageNum = RunGame.Stage.SceneController.StageNo + 1;
        enemysData = Resources.Load(LOAD_FILEPATH + LOAD_ENEMYS_NAME + stageNum.ToString()) as Enemys;
        spawnObj = Resources.Load(LOAD_GAMEOBJECT_SPAWNNAME) as GameObject;
        scoreItemManager = GameObject.Find(LOAD_SCOREITEMMANAGER_NAME).GetComponent<ScoreItemManager>();
        // デバック用
        var count = enemysData.enemyList.Count;
        spawnCount = count;
        pool = GetComponent<ObjectPool>();
        pool.CreatePool(spawnObj, spawnCount);
    }
    // Start is called before the first frame update
    void Start()
    {
        var enemyDatas = this.enemysData.enemyList;
        //for (int index = 0; index < drawObjCount; index++)
        for (int index = 0; index < enemyDatas.Count; index++)
        {
            var getObj = pool.GetPoolObj();
            // var fairyItem = getObj.GetComponent<FairyItem>();
            // fairyItem.Initialize(itemDatas[index]);
            var setPos = enemyDatas[index].position;
            var setRotate = enemyDatas[index].rotate;
            var pos = getObj.transform.position;
            pos.x = setPos.x;
            pos.y = enemyPosY;
            pos.z = setPos.z;
            getObj.transform.position = pos;
            getObj.transform.Rotate(0, setRotate, 0);
            getObj.GetComponent<Enemy>().SetEnemyData(scoreItemManager, attackPos_y,attack,hit);
            drawCount++;
        }
    }
}
