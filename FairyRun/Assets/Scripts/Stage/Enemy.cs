using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RunGame.Stage;
public class Enemy : MonoBehaviour
{
    // 妖精の攻撃のタグネーム
    private const string BULLETOBJTAGNAME = "FairyAttack";
    // デバックモードかどうか
    [SerializeField] private bool debug = false;
    // プレイヤー検知エリアの範囲指定
    [SerializeField] private float searchAreaRange = 10.0f;
    // プレイヤー検知エリア「BoxCollider」を取得
    [SerializeField] private BoxCollider searchAreaCollider = null;
    // 攻撃する間隔
    [SerializeField] private float attackTimeRange = 0.0f;
    private float attackTimeRangeMax = 0.0f;
    // プレイヤーがサーチエリア内にいるかどうか
    private bool playerIsInSearchAreaRange = false;
    // 生成する攻撃の弾
    private GameObject enemyBullet = null;
    // インスタンス化する弾のオブジェクトネーム
    private const string LOADENEMYBULLETNAME = "EnemyBullet";
    private ScoreItemManager scoreItemManager = null;
    // 自身のanimatorを取得
    private Animator enemyAnimator = null;
    //妖精の射程内にいる場合、ターゲットを出す
    SpriteRenderer targetRenderer;

    private bool alive = true;

    // 攻撃高さ
    private float attackPos_y = 2f;

    //効果音再生用
    AudioSource audioSource;

    //効果音
    AudioClip attackSound;
    AudioClip hitSound;

    [SerializeField]
    SkinnedMeshRenderer[] meshRenderer = null;

    public void SetEnemyData(ScoreItemManager scoreItemManager, float attackPos_y,AudioClip attack,AudioClip hit)
    {
        this.scoreItemManager = scoreItemManager;
        this.attackPos_y = attackPos_y;
        attackSound = attack;
        hitSound = hit;
    }
    private void Start()
    {
        // 自身のボックスコライダーを取得
        //searchAreaCollider = GetComponent<BoxCollider>();
        // ボックスコライダーの設定
        SetCollider(searchAreaRange);
        enemyBullet = Resources.Load(LOADENEMYBULLETNAME) as GameObject;
        attackTimeRangeMax = attackTimeRange;
        // 自身の「Animator」を取得
        enemyAnimator = GetComponentInChildren<Animator>();
        //自身のターゲットを取得
        targetRenderer = transform.GetChild(3).GetComponent<SpriteRenderer>();

        audioSource = GetComponent<AudioSource>();
    }
    private void Update()
    {
        //やられている場合、この処理はしない
        if (!alive)
        {
            return;
        }

        // プレイヤーに対して攻撃
        if (playerIsInSearchAreaRange)
        {
            attackTimeRange -= Time.deltaTime;
            if (attackTimeRange <= 0)
            {
                enemyAnimator.SetTrigger("Attack");
                // 攻撃
                var newBullet = Instantiate(enemyBullet, transform.position, transform.rotation);
                var attackPos = newBullet.GetComponent<Transform>().position;
                attackPos.y = attackPos_y;
                newBullet.GetComponent<Transform>().position = attackPos;
                newBullet.GetComponent<Bullet>().ShootInit();
                attackTimeRange = attackTimeRangeMax;

                audioSource.clip = attackSound;
                audioSource.Play();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        var target_Tag = other.gameObject.tag;
        if (target_Tag == BULLETOBJTAGNAME && alive)
        {
            SceneController.Instance.EnemyDefeatedCount++;
            GameObject[] newScoreItemObj = new GameObject[3];

            //現在のレーン情報等を取得するためにプレイヤー取得
            Player player = GameObject.Find("Player").GetComponent<Player>();

            int enemyLane = 0;
            float curretCenter = player.curretCenter;
            enemyAnimator.SetTrigger("Damage");
            //この敵がどこのレーンにいるのか判定
            switch (player.groundRotation)
            {
                case 0: 
                    if(curretCenter == transform.position.x)
                    {
                        enemyLane = 1;
                    }
                    else if(curretCenter < transform.position.x)
                    {
                        enemyLane = 2;
                    }
                    else
                    {
                        enemyLane = 0;
                    }break;
                case 1:
                    if(curretCenter == transform.position.z)
                    {
                        enemyLane = 1;
                    }
                    else if(curretCenter < transform.position.z)
                    {
                        enemyLane = 2;
                    }
                    else
                    {
                        enemyLane = 0;
                    }break;
                case 2:
                    if (curretCenter == transform.position.x)
                    {
                        enemyLane = 1;
                    }
                    else if (curretCenter < transform.position.x)
                    {
                        enemyLane = 2;
                    }
                    else
                    {
                        enemyLane =0;
                    }
                    break;
                case 3:
                    if (curretCenter == transform.position.z)
                    {
                        enemyLane = 1;
                    }
                    else if (curretCenter < transform.position.z)
                    {
                        enemyLane = 2;
                    }
                    else
                    {
                        enemyLane = 0;
                    }
                    break;
            }

            int[] itemLane = new int[3];
            //3つそれぞれのアイテムをどこのレーンに配置するか決定
            switch(enemyLane)
            {
                case 0: itemLane[0] = 2; itemLane[1] = 1; itemLane[2] = 0;break;
                case 1: itemLane[2] = 1; 
                    if(Random.Range(0,2) == 0)
                    {
                        itemLane[0] = 2;itemLane[1] = 0;
                    }
                    else
                    {
                        itemLane[0] = 0; itemLane[1] = 2;
                    }break;
                case 2: itemLane[0] = 0; itemLane[1] = 1; itemLane[2] = 2; break;
            }

            //Z方向に移動しているならTrue/X方向に移動しているならFalse
            bool moveZ;
            if(player.groundRotation % 2 == 0)
            {
                moveZ = true;
            }
            else
            {
                moveZ = false;
            }

            for (int i = 0; i < 3; i++)
            {
                //0=100pt/1=50pt/2=30pt
                newScoreItemObj[i] = scoreItemManager.GetScoreItem(i);
                Vector3 pos;
                if (moveZ)
                {
                    pos = new Vector3(curretCenter + (itemLane[i] - 1) * player.laneMoveRange,
                          0, transform.position.z);
                }
                else
                {
                    pos = new Vector3(transform.position.x,
                          0, curretCenter + (itemLane[i] - 1) * player.laneMoveRange);
                }

                pos += transform.forward * -5;

                newScoreItemObj[i].GetComponent<Transform>().position = pos;
                var rotateY = transform.rotation.eulerAngles.y;
                newScoreItemObj[i].GetComponent<Transform>().Rotate(0, rotateY, 0);
            }
            alive = false;
            GetComponent<CapsuleCollider>().enabled = false;
            for (int i = 0; i < meshRenderer.Length; i++)
            {
                meshRenderer[i].enabled = false;
            }
            //GetComponent<MeshRenderer>().enabled = false;
            //transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
            //transform.GetChild(1).GetComponent<MeshRenderer>().enabled = false;
            //transform.GetChild(2).GetComponent<MeshRenderer>().enabled = false;
            //transform.GetChild(3).GetComponent<MeshRenderer>().enabled = false;
            targetRenderer.enabled = false;
            GetComponentInChildren<ParticleSystem>().Emit(60);
            audioSource.clip = hitSound;
            audioSource.Play();
        }
        else if(target_Tag == "FairyRange")
        {
            targetRenderer.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "FairyRange")
        {
            targetRenderer.enabled = false;
        }
    }

    /// <summary>
    /// ボックスコライダーの設定
    /// </summary>
    /// <param name="searchAreaRange"></param>
    private void SetCollider(float searchAreaRange)
    {
        // ボックスコライダーの設定
        var colliderSize = searchAreaCollider.size;
        var colliderCenter = searchAreaCollider.center;
        colliderSize.x = 12.5f;
        colliderSize.z = searchAreaRange;
        colliderCenter.z = searchAreaRange / 2;
        searchAreaCollider.size = colliderSize;
        searchAreaCollider.center = colliderCenter;
    }

    void OnChildrenTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerIsInSearchAreaRange = true;
        }
    }

    void OnChildrenTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerIsInSearchAreaRange = false;
        }
    }
}
