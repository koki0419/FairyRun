using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunGame.Stage
{
    /// <summary>
    /// 『プレイヤー』を表します。
    /// </summary>
    public class Player : MonoBehaviour
    {
        // 通常の移動速度を指定します。
        [SerializeField, Header("プレイヤーの挙動関係")]
        private float speed = 4;
        // ダッシュ時の移動速度を指定します。
        [SerializeField]
        private float dashSpeed = 8;
        // ジャンプの力を指定します。
        [SerializeField]
        private float jumpPower = 150;
        // 設置判定の際に判定対象となるレイヤーを指定します。
        [SerializeField]
        private LayerMask groundLayer = 0;
        /// <summary>
        /// プレイ中の場合はtrue、ステージ開始前またはゲームオーバー時にはfalse
        /// </summary>
        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }
        bool isActive = false;

        // 着地している場合はtrue、ジャンプ中はfalse
        [SerializeField]
        bool isGrounded = false;
        //ジャンプ後一定フレームはジャンプできないようにする
        int jumpCount = 0;

        // コンポーネントを事前に参照しておく変数
        Animator animator;
        new Rigidbody rigidbody;
        //現在プレイヤーがいる地面の中心座標(XorZ)
        [HideInInspector]
        public float curretCenter = 0;
        //現在の地面がどの向きか Zプラス方向=0,Xプラス方向=1,Zマイナス方向=2,Xマイナス方向=3
        [HideInInspector]
        public int groundRotation = 0;
        [SerializeField]
        bool isMoving = true;
        public bool IsMoving
        {
            set { isMoving = value; }
            get { return isMoving; }
        }
        //現在プレイヤーのいるレーン
        //プレイヤーの向いている方向で見て、左が0、真ん中が1、右が2
        [HideInInspector]
        public int lane = 1;
        [SerializeField, Tooltip("プレイヤーがレーンを移動する際の横移動の距離")]
        public float laneMoveRange;
        //レーン移動中かどうか
        bool isLaneMoving = false;
        //レーン移動進行度
        float laneMoveProgress = 0;
        //移動前にいたレーン
        int prevLane = 1;
        [SerializeField, Tooltip("レーン移動速度")]
        float laneMoveSpeed;

        [SerializeField, Tooltip("回転後、どのレーンに移動するか判定する間隔"), Header("プレイヤー回転処理関係")]
        float LaneRange;
        //回転してから次の足場に入るまでの処理中かどうか
        [HideInInspector]
        public bool rotated = false;
        //カーブ可能かどうか
        bool inCurve = false;
        //カーブ時、足元にある地面(座標取得用)
        GameObject curveGround;
        //カーブ可能になった座標
        float curveInPosition = 0;
        //次のカーブをどちらに曲がるか
        [HideInInspector]
        public int nextCurve;
        //カーブに失敗したか
        [HideInInspector]
        public bool failedCurve = false;
        //カーブに失敗して壁に当たった
        bool hitFailWall = false;

        //被弾した場合のデメリット発生時間
        float speedDownTime = 0;
        [SerializeField, Tooltip("被弾時、スピードが何分の1になるか"), Header("被弾時の処理関係")]
        float speedDownAmount;
        [SerializeField, Tooltip("被弾時、スピードが下がる秒数")]
        float speedDownSecond;
        [SerializeField, Tooltip("被弾時の点滅の間隔")]
        float flashInterval;
        float flashTimer = 0;
        [SerializeField, Tooltip("点滅させるコンポーネント")]
        public SkinnedMeshRenderer[] flashRenderer;

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

        SEManager se;

        enum PlayerType
        {
            None,
            Tutorial,
            Play,
            Ending,
        }
        private PlayerType playerType = PlayerType.None;
        enum EndingScene
        {
            None,
            FirstPhase,
            SecondPhase,
            ThirdPhase,
        }
        private EndingScene endingScene = EndingScene.None;

        Vector3 firstPoint = Vector3.zero;
        Vector3 thirdPoint = Vector3.zero;
        [SerializeField]
        float firstPosDistance = 2.0f;
        [SerializeField]
        float thirdPosDistance = 2.0f;

        private PlayerAnimationChecker animationChecker = null;
        [SerializeField]
        private float endingMoveSpeed = 0.5f;
        // Start is called before the first frame update
        void Start()
        {
            // 事前にコンポーネントを参照
            animator = GetComponentInChildren<Animator>();
            rigidbody = GetComponent<Rigidbody>();

            se = Camera.main.GetComponent<SEManager>();
            isMoving = true;
            playerType = PlayerType.Play;
            animationChecker = GetComponentInChildren<PlayerAnimationChecker>();
        }

        // Update is called once per frame
        void Update()
        {
            //Debug.Log(nextCurve);
            if (SceneController.isPlaying)
            {
                switch (playerType)
                {
                    case PlayerType.Play:
                        //移動中なら
                        if (isMoving)
                        {
                            animator.SetBool("Run", true);
                            if (transform.position.y < 0)
                            {
                                transform.position = new Vector3(transform.position.x, 0, transform.position.z);
                                rigidbody.velocity = new Vector3(0, 0, 0);
                            }
                            if (jumpCount > 0) jumpCount--;
                            // ジャンプボタンが押された場合はジャンプ処理
                            if (Input.GetButtonDown("Player_1_Jump") && isGrounded && jumpCount == 0 && transform.position.y <= 0.1f)
                            {
                                Debug.Log("InputJump");
                                if (transform.position.y >= 0.5f)
                                {
                                    Debug.Log("SkyJump");
                                }
                                jumpCount = 25;
                                rigidbody.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);
                                // ジャンプ状態に設定
                                isGrounded = false;
                                animator.SetTrigger("Jump");
                                se.PlaySound(SEManager.SEName.JUMP);
                            }
                            // 向いている方向への移動
                            //var velocity = rigidbody.velocity;
                            float movingSpeed = speed;
                            if (speedDownTime > 0)
                            {
                                movingSpeed = speed / speedDownAmount;

                                //減速中の点滅処理
                                flashTimer -= Time.deltaTime;
                                if (flashTimer <= 0)
                                {
                                    flashTimer = flashInterval;
                                    for (int i = 0; i < flashRenderer.Length; i++)
                                    {
                                        if (flashRenderer[i].enabled)
                                        {
                                            flashRenderer[i].enabled = false;
                                        }
                                        else
                                        {
                                            flashRenderer[i].enabled = true;
                                        }
                                    }
                                }

                                ////減速中の場合、スピードダウン
                                speedDownTime -= Time.deltaTime;
                                if (speedDownTime < 0)
                                {
                                    speedDownTime = 0;
                                }
                            }
                            else
                            {
                                for (int i = 0; i < flashRenderer.Length; i++)
                                {
                                    if (!flashRenderer[i].enabled)
                                    {
                                        flashRenderer[i].enabled = true;
                                    }
                                }
                            }

                            //ジャンプ等のY方向ベロシティを保存
                            float rigidY = rigidbody.velocity.y;
                            switch (groundRotation)
                            {
                                //向きに合わせて移動
                                case 0: rigidbody.velocity = new Vector3(0, rigidY, movingSpeed); break;
                                case 1: rigidbody.velocity = new Vector3(movingSpeed, rigidY, 0); break;
                                case 2: rigidbody.velocity = new Vector3(0, rigidY, -movingSpeed); break;
                                case 3: rigidbody.velocity = new Vector3(-movingSpeed, rigidY, 0); break;
                            }

                            //横に回転する処理
                            NextLaneRotation();

                            //レーン移動
                            if (!stickInput && !isLaneMoving && !inCurve)
                            {
                                if (Input.GetAxis("Player_1_Move") > 0 && (Input.GetAxis("Player_1_Move") > prevStick))
                                {
                                    if (lane <= 1)
                                    {
                                        isLaneMoving = true;
                                        laneMoveProgress = 0;
                                        prevLane = lane;
                                        lane++;
                                        stickInput = true;
                                    }
                                }
                                else if (Input.GetAxis("Player_1_Move") < 0 && (Input.GetAxis("Player_1_Move") < prevStick))
                                {
                                    if (lane >= 1)
                                    {
                                        isLaneMoving = true;
                                        laneMoveProgress = 0;
                                        prevLane = lane;
                                        lane--;
                                        stickInput = true;
                                    }
                                }
                            }
                            else if ((Input.GetAxis("Player_1_Move") > 0 && (Input.GetAxis("Player_1_Move") < prevStick)) ||
                                    (Input.GetAxis("Player_1_Move") < 0 && (Input.GetAxis("Player_1_Move") > prevStick))
                                    || Input.GetAxis("Player_1_Move") == 0)
                            {
                                stickInput = false;
                            }

                            //今フレームのAxis値を保存
                            prevStick = Input.GetAxis("Player_1_Move");

                            //現在レーンに横位置を移動
                            LaneMove();
                        }
                        else
                        {
                            rigidbody.velocity = new Vector3(0, 0, 0);
                        }
                        break;
                    case PlayerType.Ending:
                        var pos = transform.position;
                        switch (endingScene)
                        {
                            case EndingScene.None:
                                rigidbody.velocity = new Vector3(0, 0, endingMoveSpeed);
                                break;
                            case EndingScene.FirstPhase:
                                if (SceneController.playEnding)
                                {
                                    animator.SetTrigger("Ending");
                                    rigidbody.velocity = new Vector3(0, 0, endingMoveSpeed);
                                    // 指定したポジションに着いたら「バイバイ」を再生
                                    if (firstPoint.z <= gameObject.transform.position.z)
                                    {
                                        endingScene = EndingScene.SecondPhase;
                                        thirdPoint = transform.position;
                                        thirdPoint.z += thirdPosDistance;
                                        transform.Rotate(0, 180, 0);
                                    }
                                }
                                else
                                {
                                    rigidbody.velocity = new Vector3(0, 0, 0);
                                }
                                break;
                            case EndingScene.SecondPhase:
                                animator.SetTrigger("EndingNext_2");
                                if (animationChecker.GetBaibaiFinish)
                                {
                                    animationChecker.GetBaibaiFinish = false;
                                    endingScene = EndingScene.ThirdPhase;
                                    transform.Rotate(0, 180, 0);
                                    return;
                                }
                                break;
                            case EndingScene.ThirdPhase:
                                rigidbody.velocity = new Vector3(0, 0, endingMoveSpeed);
                                if (thirdPoint.z <= gameObject.transform.position.z)
                                {
                                    SceneController.isEnding = true;
                                    endingScene = EndingScene.None;
                                    return;
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                }
            }
            else
            {
                rigidbody.velocity = new Vector3(0, 0, 0);
            }
        }

        /// <summary>
        /// このプレイヤーが他のオブジェクトのトリガー内に侵入した際に
        /// 呼び出されます。
        /// </summary>
        /// <param name="collider">侵入したトリガー</param>
        private void OnTriggerEnter(Collider collider)
        {
            // ゴール判定
            if (collider.tag == "Finish")
            {
                isMoving = false;
                if (SceneController.ending)
                {
                    playerType = PlayerType.Ending;
                    endingScene = EndingScene.FirstPhase;
                    firstPoint = transform.position;
                    firstPoint.z += firstPosDistance;
                }
                else
                {
                    playerType = PlayerType.Play;
                    endingScene = EndingScene.None;
                }
                SceneController.Instance.StageClear();
            }
            // アイテムを取得
            else if (collider.tag == "Item")
            {
                // 取得したアイテムを削除
                collider.gameObject.SetActive(false);
                //Destroy(collider.gameObject);
            }
            else if (collider.tag == "GroundStartPoint")
            {
                rotated = false;
            }
            else if (collider.tag == "Obstacle" || collider.tag == "Enemy")
            {
                //障害物に当たった
                animator.SetTrigger("Hit");
                speedDownTime = speedDownSecond;
            }
            else if (collider.tag == "CurvePoint")
            {
                //カーブ可能になった
                CurvePoint curve = collider.gameObject.GetComponent<CurvePoint>();
                nextCurve = curve.GetNextCurve();

                hitFailWall = false;
                inCurve = true;
                curveGround = collider.gameObject.transform.parent.gameObject;

                //座標を記録
                if (groundRotation == 0 || groundRotation == 2)
                {
                    curveInPosition = transform.position.z;
                }
                else
                {
                    curveInPosition = transform.position.x;
                }
            }
            else if (collider.tag == "FailWall")
            {
                if (hitFailWall)
                {
                    return;
                }

                //カーブの壁に当たった
                rotated = true;
                inCurve = false;
                hitFailWall = true;
                //デメリットで減速
                speedDownTime = speedDownSecond;

                if (failedCurve)
                {
                    //間違えた方向にカーブしていた場合、180度回転
                    failedCurve = false;

                    //180度回転
                    if (lane == 0)
                    {
                        lane = 2;
                    }
                    else if (lane == 2)
                    {
                        lane = 0;
                    }

                    if (nextCurve == 0)
                    {
                        groundRotation++;
                        if (groundRotation > 3)
                        {
                            groundRotation = 0;
                        }
                    }
                    else
                    {
                        groundRotation--;
                        if (groundRotation < 0)
                        {
                            groundRotation = 3;
                        }
                    }

                    //移動先レーンに座標を変更
                    if (nextCurve == 0)
                    {
                        CurretLaneMoving(lane, true);
                    }
                    else
                    {
                        CurretLaneMoving(lane, false);
                    }

                    //回転
                    transform.Rotate(0, -180, 0);
                    //rotated = true;

                    if (nextCurve == 0)
                    {
                        groundRotation++;
                        if (groundRotation > 3)
                        {
                            groundRotation = 0;
                        }
                    }
                    else
                    {
                        groundRotation--;
                        if (groundRotation < 0)
                        {
                            groundRotation = 3;
                        }
                    }
                }
                else
                {
                    //カーブ時にキー入力をしなかった
                    //カーブ方向に応じて次のレーン番号を変更
                    if (nextCurve == 0)
                    {
                        lane = 2;
                    }
                    else
                    {
                        lane = 0;
                    }

                    //カーブ後のレーンの中心を取得
                    if (groundRotation == 0 || groundRotation == 2)
                    {
                        curretCenter = curveGround.transform.position.z;
                    }
                    else
                    {
                        curretCenter = curveGround.transform.position.x;
                    }

                    //移動先レーンに座標を変更、回転
                    if (nextCurve == 0)
                    {
                        CurretLaneMoving(lane, true);

                        transform.Rotate(0, -90, 0);

                        groundRotation--;
                        if (groundRotation < 0)
                        {
                            groundRotation = 3;
                        }
                    }
                    else
                    {
                        CurretLaneMoving(lane, false);

                        transform.Rotate(0, 90, 0);

                        groundRotation++;
                        if (groundRotation > 3)
                        {
                            groundRotation = 0;
                        }
                    }

                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Ground")
            {
                isGrounded = true;
            }
        }

        /// <summary>
        /// 現在レーンに合わせて横位置を移動
        /// </summary>
        private void LaneMove()
        {
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

            //Mathf.Lerp(prevLane,lane,laneMoveProgress)-1

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
        /// 回転のキー受け付け処理
        /// </summary>
        private void NextLaneRotation()
        {
            //連続で回転は行えないようにする
            if (rotated)
            {
                return;
            }

            //カーブに入っている状態か
            if (inCurve)
            {
                //カーブ開始になってからの距離をもとめる
                float curretPosition;
                //現在のXorZ座標を取得
                if (groundRotation == 0 || groundRotation == 2)
                {
                    curretPosition = transform.position.z;
                }
                else
                {
                    curretPosition = transform.position.x;
                }
                float inCurveRange;
                //カーブ可能ポイントに入った座標と現在座標の差をもとめる
                if (curveInPosition > curretPosition)
                {
                    inCurveRange = curveInPosition - curretPosition;
                }
                else
                {
                    inCurveRange = curretPosition - curveInPosition;
                }

                //左ボタンが押された場合
                if (Input.GetAxis("Player_1_Move") < -stickAmount && !stickInput)
                {
                    stickInput = true;
                    //カーブ成功
                    if (nextCurve == 0)
                    {
                        Curve(0, inCurveRange, true);
                    }
                    //カーブ失敗
                    else
                    {
                        Curve(0, inCurveRange, false);
                    }

                }
                //右ボタンが押されたとき
                else if (Input.GetAxis("Player_1_Move") > stickAmount && !stickInput)
                {
                    stickInput = true;
                    //カーブ成功
                    if (nextCurve == 2)
                    {
                        Curve(1, inCurveRange, true);
                    }
                    //カーブ失敗
                    else
                    {
                        Curve(1, inCurveRange, false);
                    }
                }
            }
        }



        /// <summary>
        /// カーブを行う処理
        /// </summary>
        /// <param name="rotation">0=左,1=右</param>
        /// <param name="groundRange">どのレーンに移動するか判定用の距離</param>
        /// <param name="success">正しい方向にカーブしたか</param>
        private void Curve(int rotation, float groundRange, bool success)
        {
            inCurve = false;
            //正しい方向にカーブしたなら
            if (success)
            {
                rotated = true;
                if (rotation == 0)
                {
                    //現在位置に応じて次のレーン番号を変更
                    if (groundRange > 0 &&
                       groundRange < LaneRange)
                    {
                        lane = 0;
                    }
                    else if (groundRange < LaneRange * 2)
                    {
                        lane = 1;
                    }
                    else
                    {
                        lane = 2;
                    }

                    //カーブ後のレーンの中心を取得
                    if (groundRotation == 0 || groundRotation == 2)
                    {
                        curretCenter = curveGround.transform.position.z;
                    }
                    else
                    {
                        curretCenter = curveGround.transform.position.x;
                    }

                    //移動先レーンに座標を変更
                    CurretLaneMoving(lane, true);

                    //回転
                    transform.Rotate(0, -90, 0);
                    //rotated = true;

                    //方向を変更
                    groundRotation--;
                    if (groundRotation < 0)
                    {
                        groundRotation = 3;
                    }
                }
                else
                {
                    int nextLaneNumber;
                    //現在位置に応じて次のレーン番号を変更
                    if (groundRange > 0 &&
                       groundRange < LaneRange)
                    {
                        lane = 2;
                        nextLaneNumber = 0;
                    }
                    else if (groundRange < LaneRange * 2)
                    {
                        lane = 1;
                        nextLaneNumber = 1;
                    }
                    else
                    {
                        lane = 0;
                        nextLaneNumber = 2;
                    }

                    //カーブ後のレーンの中心を取得
                    if (groundRotation == 0 || groundRotation == 2)
                    {
                        curretCenter = curveGround.transform.position.z;
                    }
                    else
                    {
                        curretCenter = curveGround.transform.position.x;
                    }
                    //移動先レーンに座標を変更
                    CurretLaneMoving(nextLaneNumber, true);

                    //回転
                    transform.Rotate(0, 90, 0);
                    //rotated = true;

                    //方向を変更
                    groundRotation++;
                    if (groundRotation > 3)
                    {
                        groundRotation = 0;
                    }
                }
            }
            //カーブ失敗の場合、回転のみ行う
            else
            {
                //カーブ失敗後に壁にぶつかったら180度回転させる処理
                failedCurve = true;
                Debug.Log("CurveFailed");
                if (rotation == 0)
                {
                    //回転
                    transform.Rotate(0, -90, 0);
                    //rotated = true;

                    //カーブ後のレーンの中心を取得
                    if (groundRotation == 0 || groundRotation == 2)
                    {
                        curretCenter = curveGround.transform.position.z;
                    }
                    else
                    {
                        curretCenter = curveGround.transform.position.x;
                    }

                    //方向を変更
                    groundRotation--;
                    if (groundRotation < 0)
                    {
                        groundRotation = 3;
                    }
                }
                else
                {
                    //回転
                    transform.Rotate(0, 90, 0);
                    //rotated = true;

                    //カーブ後のレーンの中心を取得
                    if (groundRotation == 0 || groundRotation == 2)
                    {
                        curretCenter = curveGround.transform.position.z;
                    }
                    else
                    {
                        curretCenter = curveGround.transform.position.x;
                    }

                    //方向を変更
                    groundRotation++;
                    if (groundRotation > 3)
                    {
                        groundRotation = 0;
                    }
                }
            }
        }

        /// <summary>
        /// レーンに合わせて座標を変更
        /// </summary>
        /// <param name="range">0~2、手前から見てどのレーンに移動するのか</param>
        /// <param name="curveLeft">true = 左に曲がる/false = 右に曲がる</param>
        private void CurretLaneMoving(int range, bool curveLeft)
        {
            switch (groundRotation)
            {
                case 0:
                    if (curveLeft)
                    {
                        //ZプラスからXマイナスに曲がる
                        transform.position = new Vector3(transform.position.x, transform.position.y,
                         curretCenter + (range - 1) * laneMoveRange);
                        break;
                    }
                    else
                    {
                        //ZプラスからXプラスに曲がる
                        transform.position = new Vector3(transform.position.x, transform.position.y,
                         curretCenter + (range - 1) * laneMoveRange);
                        break;
                    }
                case 1:
                    if (curveLeft)
                    {
                        //XプラスからZプラスに曲がる
                        transform.position = new Vector3(curretCenter +
                        (range - 1) * laneMoveRange,
                         transform.position.y, transform.position.z);
                        break;
                    }
                    else
                    {
                        //XプラスからZマイナスに曲がる
                        transform.position = new Vector3(curretCenter +
                         (range - 1) * laneMoveRange,
                         transform.position.y, transform.position.z);
                        break;
                    }
                case 2:
                    if (curveLeft)
                    {
                        //ZマイナスからXプラスに曲がる
                        transform.position = new Vector3(transform.position.x, transform.position.y,
                         curretCenter - (range - 1) * laneMoveRange);
                        break;
                    }
                    else
                    {
                        //ZマイナスからXマイナスに曲がる
                        transform.position = new Vector3(transform.position.x, transform.position.y,
                         curretCenter - (range - 1) * laneMoveRange);
                        break;
                    }
                case 3:
                    if (curveLeft)
                    {
                        //XマイナスからZマイナスに曲がる
                        transform.position = new Vector3(curretCenter -
                         (range - 1) * laneMoveRange,
                         transform.position.y, transform.position.z);
                        break;
                    }
                    else
                    {
                        //XマイナスからZプラスに曲がる
                        transform.position = new Vector3(curretCenter -
                         (range - 1) * laneMoveRange,
                         transform.position.y, transform.position.z);
                        break;
                    }
            }
        }

        //移動速度を下げる
        public void SpeedDown()
        {
            speedDownTime = speedDownSecond;
        }
    }
}