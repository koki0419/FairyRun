using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunGame.Stage
{
    /// <summary>
    /// 『妖精』を表します。
    /// </summary>
    public class Fairy : MonoBehaviour
    {
        // 通常の移動速度を指定します。
        [SerializeField, Header("妖精の挙動関係")]
        private AudioClip soundOnDash = null;

        [SerializeField, Tooltip("妖精のY座標位置")]
        float positionY;
        [SerializeField, Tooltip("プレイヤーからの奥行方向距離")]
        float playerRangeZ;

        /// <summary>
        /// プレイ中の場合はtrue、ステージ開始前またはゲームオーバー時にはfalse
        /// </summary>
        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }
        bool isActive = false;

        // AnimatorのパラメーターID
        static readonly int dashId = Animator.StringToHash("isDash");

        // ダッシュ状態の場合はtrue
        public bool IsDash
        {
            get { return isDash; }
            private set
            {
                isDash = value;
                // ダッシュ状態への遷移時
                if (value)
                {
                    // ダッシュアニメーションへ切り替え
                    animator.SetBool(dashId, true);
                    // サウンド再生
                    if (audioSource.isPlaying)
                    {
                        audioSource.Stop();
                    }
                    audioSource.clip = soundOnDash;
                    audioSource.loop = true;
                    audioSource.Play();
                }
                // 通常状態へ遷移する場合
                else
                {
                    // 通常アニメーションへ切り替え
                    animator.SetBool(dashId, false);
                    // サウンド停止
                    if (audioSource.isPlaying)
                    {
                        audioSource.Stop();
                    }
                }
            }
        }
        bool isDash = false;

        // コンポーネントを事前に参照しておく変数
        Animator animator;
        new Rigidbody rigidbody;
        // サウンドエフェクト再生用のAudioSource
        AudioSource audioSource;
        Player player;

        //プレイヤーから取得した情報
        float curretCenter;
        //現在の地面がどの向きか Zプラス方向=0,Xプラス方向=1,Zマイナス方向=2,Xマイナス方向=3
        int groundRotation = 0;
        float laneMoveRange;

        //現在妖精のいるレーン
        //妖精の向いている方向で見て、左が0、真ん中が1、右が2
        int lane = 1;
        //レーン移動中かどうか
        bool isLaneMoving = false;
        //レーン移動進行度
        float laneMoveProgress = 0;
        //移動前にいたレーン
        int prevLane = 1;
        [SerializeField, Tooltip("レーン移動速度")]
        float laneMoveSpeed;

        [SerializeField, Tooltip("プレイヤー回転処理を開始する、各地面中心位置からの距離"),
                         Header("プレイヤー回転処理関係")]
        float rotationRange;
        [SerializeField, Tooltip("回転後、どのレーンに移動するか判定する間隔")]
        float LaneRange;
        //回転してから次の足場に入るまでの処理中かどうか
        bool rotated = false;

        //被弾した場合のデメリット発生時間
        float speedDownTime = 0;
        [SerializeField, Tooltip("被弾時、スピードが何分の1になるか"), Header("被弾時の処理関係")]
        float speedDownAmount;
        [SerializeField, Tooltip("被弾時、スピードが下がる秒数")]
        float speedDownSecond;
        [SerializeField, Tooltip("点滅させるコンポーネント")]
        SkinnedMeshRenderer flashRenderer;

        [SerializeField, Tooltip("スティック入力を判定する値")
                       , Header("操作関係"), Range(0, 1)]
        float stickAmount;
        [SerializeField, Tooltip("スティック操作の連続受付を防止するためのキャンセル値")
                        , Range(0, 1)]
        float stickCancel;
        //左右入力がされている状態(連続で移動を行わない処理)
        bool stickInput = false;
        //前フレームでのスティック入力値
        float prevStick = 0;

        [SerializeField, Tooltip("妖精の攻撃を行える間隔(秒)"), Header("妖精の攻撃関係")]
        float fairyAttackInterval;
        //妖精が次の攻撃を行えるまでの秒数
        float fairyAttackSecond = 0;
        //妖精の攻撃(弾)
        GameObject fairyBullet;
        const string LOADFAIRYBULLETNAME = "FairyBullet";
        [SerializeField, Tooltip("妖精の攻撃の飛ぶ速度")]
        float fairyAttackSpeed;
        [SerializeField, Tooltip("妖精の攻撃が何秒後に消えるか")]
        float fairyAttackRemain;
        //妖精の攻撃回数取得用
        UiManager uiManager;

        [SerializeField, Tooltip("衛星の最大表示数"), Header("妖精の弾を周囲に表示する処理関係")]
        int orbitMaxCount;
        [SerializeField, Tooltip("衛星の妖精からの距離")]
        float orbitRange;
        [SerializeField, Tooltip("衛星の回転スピード")]
        float orbitSpeed;
        [SerializeField, Tooltip("衛星を表示するY位置")]
        float orbitYpos;
        float orbitRotate = 0;

        //衛星のオブジェクト
        GameObject orbitPrefab;
        //生成した衛星
        List<GameObject> orbits = new List<GameObject>();

        //SE再生用
        SEManager se;

        // Start is called before the first frame update
        void Start()
        {
            // 事前にコンポーネントを参照
            animator = GetComponentInChildren<Animator>();
            rigidbody = GetComponent<Rigidbody>();
            audioSource = GetComponent<AudioSource>();

            //プレイヤーを取得
            player = GameObject.Find("Player").GetComponent<Player>();
            laneMoveRange = player.laneMoveRange;
            //UIマネージャー取得
            uiManager = GameObject.Find("UI Canvas").GetComponent<UiManager>();
            //SEマネージャー
            se = Camera.main.GetComponent<SEManager>();

            //妖精の周りのアイテムのプレハブ設定、生成
            orbitPrefab = Resources.Load("FairyOrbit") as GameObject;
            for (int i = 0; i < orbitMaxCount; i++)
            {
                GameObject orbit = Instantiate(orbitPrefab);
                orbits.Add(orbit);
                orbit.transform.parent = transform;
                orbit.SetActive(false);
            }
            //妖精の攻撃を取得
            fairyBullet = Resources.Load<GameObject>(LOADFAIRYBULLETNAME);
        }

        // Update is called once per frame
        void Update()
        {
            if (SceneController.isPlaying)
            {
                if(!player.IsMoving)
                {
                    rigidbody.velocity = new Vector3(0, 0, 0);
                    return;
                }
                animator.SetBool("Run", true);
                curretCenter = player.curretCenter;
                groundRotation = player.groundRotation;

                //被弾時の点滅処理(プレイヤーに合わせる)
                for (int i = 0; i < player.flashRenderer.Length; i++)
                {
                    if (player.flashRenderer[i].enabled == false && flashRenderer.enabled == true)
                    {
                        flashRenderer.enabled = false;
                    }
                    if (player.flashRenderer[i].enabled == true && flashRenderer.enabled == false)
                    {
                        flashRenderer.enabled = true;
                    }
                }

                //プレイヤーが回転を行った場合、プレイヤーにレーンを合わせて移動
                if (player.rotated)
                {
                    if (!rotated)
                    {
                        rotated = true;
                        player.rotated = false;
                        lane = player.lane;

                        //妖精の向き変更(ショット用))
                        switch (groundRotation)
                        {
                            case 0: transform.rotation = Quaternion.Euler(0, 0, 0); break;
                            case 1: transform.rotation = Quaternion.Euler(0, 90, 0); break;
                            case 2: transform.rotation = Quaternion.Euler(0, 180, 0); break;
                            case 3: transform.rotation = Quaternion.Euler(0, 270, 0); break;
                        }
                    }
                }
                else if (rotated)
                {
                    rotated = false;
                }

                // プレイヤーの位置に合わせて移動
                transform.position = player.transform.position;
                transform.position = new Vector3(transform.position.x, positionY, transform.position.z);
                transform.Translate(0, 0, playerRangeZ);

                //回転後はプレイヤーに追従、レーン移動処理は行わない
                if (rotated)
                {
                    return;
                }

                //横に回転する処理
                //NextLaneRotation();

                //レーン移動
                if (!stickInput && !isLaneMoving)
                {
                    if (Input.GetAxis("Player_2_Move") > 0 && (Input.GetAxis("Player_2_Move") > prevStick))
                    {
                        if (lane <= 1)
                        {
                            animator.SetTrigger("LaneMove");
                            isLaneMoving = true;
                            laneMoveProgress = 0;
                            prevLane = lane;
                            lane++;
                            stickInput = true;
                        }
                    }
                    else if (Input.GetAxis("Player_2_Move") < 0 && (Input.GetAxis("Player_2_Move") < prevStick))
                    {
                        if (lane >= 1)
                        {
                            animator.SetTrigger("LaneMove");
                            isLaneMoving = true;
                            laneMoveProgress = 0;
                            prevLane = lane;
                            lane--;
                            stickInput = true;
                        }
                    }
                }
                else if ((Input.GetAxis("Player_2_Move") > 0 && (Input.GetAxis("Player_2_Move") < prevStick)) ||
                        (Input.GetAxis("Player_2_Move") < 0 && (Input.GetAxis("Player_2_Move") > prevStick))
                        || Input.GetAxis("Player_2_Move") == 0)
                {
                    stickInput = false;
                }

                //今フレームのAxis値を保存
                prevStick = Input.GetAxis("Player_2_Move");

                //現在レーンに横位置を移動
                LaneMove();

                //妖精の攻撃
                FairyAttackInput();

                //妖精の周りにアイテムを回す処理
                FairyOrbitSpin();
            }
            else
            {
                // プレイヤーの位置に合わせて移動
                transform.position = player.transform.position;
                transform.position = new Vector3(transform.position.x, positionY, transform.position.z);
                transform.Translate(0, 0, playerRangeZ);
            }
        }

        /// <summary>
        /// このプレイヤーが他のオブジェクトのトリガー内に侵入した際に
        /// 呼び出されます。
        /// </summary>
        /// <param name="collider">侵入したトリガー</param>
        private void OnTriggerEnter(Collider collider)
        {
            var targetTag = collider.gameObject.tag;
            if (targetTag == "EnemyBullet")
            {
                animator.SetTrigger("Damage");
                // エネミーの攻撃を食らったときの処理
                // スピードDown;
                player.SpeedDown();
                se.PlaySound(SEManager.SEName.HIT);
            }
        }

        /// <summary>
        /// 現在レーンに合わせて横位置を移動
        /// </summary>
        private void LaneMove()
        {
            //回転してから次の足場を取得するまではレーン移動を行わない
            if (rotated)
            {
                return;
            }

            //レーン移動処理
            if (isLaneMoving)
            {
                laneMoveProgress += laneMoveSpeed * Time.deltaTime;
                if (laneMoveProgress > 1)
                {
                    laneMoveProgress = 1;
                    isLaneMoving = false;
                }
            }

            //向いている方向に合わせて横移動を行う
            switch (groundRotation)
            {
                case 0:
                    transform.position =
                    new Vector3(curretCenter + (Mathf.Lerp(prevLane, lane, laneMoveProgress) - 1) * laneMoveRange,
                                transform.position.y, transform.position.z); break;
                case 1:
                    transform.position =
                    new Vector3(transform.position.x, transform.position.y,
                                curretCenter + (((Mathf.Lerp(prevLane, lane, laneMoveProgress) - 1) * -1)) * laneMoveRange); break;
                case 2:
                    transform.position =
                     new Vector3(curretCenter + ((Mathf.Lerp(prevLane, lane, laneMoveProgress) - 1) * -1) * laneMoveRange,
                                  transform.position.y, transform.position.z); break;
                case 3:
                    transform.position =
                     new Vector3(transform.position.x, transform.position.y,
                                 curretCenter + (Mathf.Lerp(prevLane, lane, laneMoveProgress) - 1) * laneMoveRange); break;
                default:
                    transform.position =
                    new Vector3(curretCenter + (Mathf.Lerp(prevLane, lane, laneMoveProgress) - 1) * laneMoveRange,
                                transform.position.y, transform.position.z); break;
            }
        }

        /// <summary>
        /// 回転処理
        /// </summary>
        /*private void NextLaneRotation()
        {
            //連続で回転は行えないようにする
            if (rotated)
            {
                return;
            }

            //カーブに入っている状態か
            if (groundRange > rotationRange &&
               groundRange < rotationRange + LaneRange * 3)
            {
                Debug.Log("InCurve");

                //左ボタンが押された場合
                if (Input.GetAxis("Player_1_Move") < -stickAmount && !stickInput)
                {
                    stickInput = true;
                    //現在位置に応じて次のレーン番号を変更
                    if (groundRange > rotationRange &&
                       groundRange < rotationRange + LaneRange)
                    {
                        lane = 0;
                    }
                    else if (groundRange < rotationRange + LaneRange * 2)
                    {
                        lane = 1;
                    }
                    else
                    {
                        lane = 2;
                    }

                    //移動先レーンに座標を変更
                    CurretLaneMoving(lane, true);

                    //回転
                    transform.Rotate(0, -90, 0);
                    rotated = true;

                    //方向を変更
                    groundRotation--;
                    if (groundRotation < 0)
                    {
                        groundRotation = 3;
                    }
                }
                //右ボタンが押されたとき
                else if (Input.GetAxis("Player_1_Move") > stickAmount && !stickInput)
                {
                    stickInput = true;
                    //現在位置に応じて次のレーン番号を変更
                    if (groundRange > rotationRange &&
                       groundRange < rotationRange + LaneRange)
                    {
                        lane = 2;
                        //移動先レーンに座標を変更
                        CurretLaneMoving(0, false);
                    }
                    else if (groundRange < rotationRange + LaneRange * 2)
                    {
                        lane = 1;
                        //移動先レーンに座標を変更
                        CurretLaneMoving(1, false);
                    }
                    else
                    {
                        lane = 0;
                        //移動先レーンに座標を変更
                        CurretLaneMoving(2, false);
                    }

                    //回転
                    transform.Rotate(0, 90, 0);
                    rotated = true;

                    //方向を変更
                    groundRotation++;
                    if (groundRotation > 3)
                    {
                        groundRotation = 0;
                    }
                }
            }
        }*/

        /// <summary>
        /// レーンに合わせて座標を変更
        /// </summary>
        /// <param name="range">0~2、手前から見てどのレーンに移動するのか</param>
        /// <param name="curveLeft">true = 左に曲がる/false = 右に曲がる</param>
        /*private void CurretLaneMoving(int range, bool curveLeft)
        {
            switch (groundRotation)
            {
                case 0:
                    if (curveLeft)
                    {
                        //ZプラスからXマイナスに曲がる
                        transform.position = new Vector3(transform.position.x, transform.position.y,
                         curretGround.transform.position.z + rotationRange + LaneRange / 2
                         + LaneRange * range);
                        break;
                    }
                    else
                    {
                        //ZプラスからXプラスに曲がる
                        transform.position = new Vector3(transform.position.x, transform.position.y,
                         curretGround.transform.position.z + rotationRange + LaneRange / 2
                         + LaneRange * range);
                        break;
                    }
                case 1:
                    if (curveLeft)
                    {
                        //XプラスからZプラスに曲がる
                        transform.position = new Vector3(curretGround.transform.position.x +
                         rotationRange + LaneRange / 2 + LaneRange * range,
                         transform.position.y, transform.position.z);
                        break;
                    }
                    else
                    {
                        //XプラスからZマイナスに曲がる
                        transform.position = new Vector3(curretGround.transform.position.x +
                         rotationRange + LaneRange / 2 + LaneRange * range,
                         transform.position.y, transform.position.z);
                        break;
                    }
                case 2:
                    if (curveLeft)
                    {
                        //ZマイナスからXプラスに曲がる
                        transform.position = new Vector3(transform.position.x, transform.position.y,
                         curretGround.transform.position.z - rotationRange - LaneRange / 2
                         - LaneRange * range);
                        break;
                    }
                    else
                    {
                        //ZマイナスからXマイナスに曲がる
                        transform.position = new Vector3(transform.position.x, transform.position.y,
                         curretGround.transform.position.z - rotationRange - LaneRange / 2
                         - LaneRange * range);
                        break;
                    }
                case 3:
                    if (curveLeft)
                    {
                        //XマイナスからZマイナスに曲がる
                        transform.position = new Vector3(curretGround.transform.position.x -
                         rotationRange - LaneRange / 2 - LaneRange * range,
                         transform.position.y, transform.position.z);
                        break;
                    }
                    else
                    {
                        //XマイナスからZプラスに曲がる
                        transform.position = new Vector3(curretGround.transform.position.x -
                         rotationRange - LaneRange / 2 - LaneRange * range,
                         transform.position.y, transform.position.z);
                        break;
                    }
            }
        }*/

        /// <summary>
        /// 妖精の攻撃関係の入力取得、妖精の攻撃
        /// </summary>
        private void FairyAttackInput()
        {
            //妖精が攻撃可能な状態の場合
            if (fairyAttackSecond <= 0 && SceneController.fairyPoint > 0)
            {
                //攻撃ボタンが押された
                if (Input.GetButtonDown("Player_2_Attack"))
                {
                    //クールタイム設定
                    fairyAttackSecond = fairyAttackInterval;
                    //残弾数を減らす
                    SceneController.fairyPoint--;
                    //効果音再生
                    //se.PlaySound(SEManager.SEName.ATTACK);

                    //  animaton再生
                    animator.SetTrigger("Attack");
                    //弾を生成
                    var fairyAttack = Instantiate(fairyBullet) as GameObject;
                    Vector3 pos;
                    //弾の座標を設定
                    switch (groundRotation)
                    {
                        case 0:
                            pos = new Vector3(curretCenter + (lane - 1) * laneMoveRange,
                             transform.position.y, transform.position.z); break;
                        case 1:
                            pos = new Vector3(transform.position.x, transform.position.y,
                             curretCenter + (lane - 1) * -laneMoveRange); break;
                        case 2:
                            pos = new Vector3(curretCenter + (lane - 1) * -laneMoveRange,
                          transform.position.y, transform.position.z); break;
                        case 3:
                            pos = new Vector3(transform.position.x, transform.position.y,
                          curretCenter + (lane - 1) * laneMoveRange); break;
                        default:
                            pos = new Vector3(curretCenter + (lane - 1) * laneMoveRange,
                        transform.position.y, transform.position.z); break;
                    }
                    fairyAttack.transform.position = pos;
                    fairyAttack.transform.rotation = transform.rotation;
                    fairyAttack.GetComponent<Bullet>().ShootInit();
                }
            }
            //妖精が攻撃不可能な状態の場合
            else
            {
                fairyAttackSecond -= Time.deltaTime;
            }
        }

        /// <summary>
        /// 妖精の周りにアイテムを回す処理
        /// </summary>
        private void FairyOrbitSpin()
        {
            int attackPoint = SceneController.fairyPoint;

            if (attackPoint > orbitMaxCount)
            {
                attackPoint = orbitMaxCount;
            }

            orbitRotate += orbitSpeed;

            for (int i = 0; i < orbitMaxCount; i++)
            {
                if (i < attackPoint)
                {
                    //表示
                    if (!orbits[i].activeSelf)
                    {
                        orbits[i].SetActive(true);
                    }

                    orbits[i].transform.localPosition = new Vector3(Mathf.Sin(orbitRotate + i) * orbitRange, orbitYpos,
                                                                    Mathf.Cos(orbitRotate + i) * orbitRange);
                }
                else
                {
                    //非表示
                    if (orbits[i].activeSelf)
                    {
                        orbits[i].SetActive(false);
                    }
                }
            }
        }
    }
}