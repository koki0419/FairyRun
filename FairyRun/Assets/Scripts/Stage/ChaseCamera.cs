using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunGame.Stage
{
    /// <summary>
    /// 追尾カメラを表します。
    /// </summary>
    public class ChaseCamera : MonoBehaviour
    {
        // 追尾対象(プレイヤー)
        Transform target;

        //プレイヤーのスクリプト(レーン中心座標取得用)
        Player player;

        [SerializeField, Tooltip("カメラのRotationX")]
        float cameraRotationX;
        //カメラのRotationY
        float cameraRotationY = 0;
        [SerializeField, Tooltip("カメラ回転速度")]
        float cameraRotateSpeed;
        //カメラ回転中か
        bool isCameraRotate = false;
        //カメラ回転進行度
        float cameraRotateProgress = 1;
        //カメラ回転前角度
        float prevRotate;
        //カメラ回転後角度
        [SerializeField]
        float endRotate;

        float groundRotation = 0;

        [SerializeField, Tooltip("カメラのPositionY")]
        float cameraPositionY;

        [SerializeField, Tooltip("プレイヤーからの距離")]
        float cameraRange;

        // Start is called before the first frame update
        void Start() {
            // 他のゲームオブジェクトを参照
            target = GameObject.FindGameObjectWithTag("Player").transform;

            // 追尾対象が未指定の場合は"Player"タグのオブジェクトで設定
            if (target == null) {
                target = GameObject.FindGameObjectWithTag("Player").transform;
            }

            //プレイヤーのスクリプトを参照
            player = target.GetComponent<Player>();
        }

        // Update is called once per frame
        void Update() {

           // Debug.Log(groundRotation);
            if (target != null) {

                //プレイヤーの回転を感知
                if(groundRotation != player.groundRotation)
                {
                    if(groundRotation % 2 == player.groundRotation % 2)
                    {
                        groundRotation = player.groundRotation;
                        return;
                    }

                    isCameraRotate = true;
                    cameraRotateProgress = 0;
                    prevRotate = cameraRotationY;

                    if (player.failedCurve)
                    {
                        if(player.nextCurve == 2)
                        {
                            endRotate += 90;
                            groundRotation++;
                            if (groundRotation > 3)
                            {
                                groundRotation = 0;
                            }
                        }
                        else
                        {
                            endRotate -= 90;
                            groundRotation--;
                            if (groundRotation < 0)
                            {
                                groundRotation = 3;
                            }
                        }
                    }
                    else
                    {
                        if(groundRotation == 3 && player.groundRotation == 0)
                        {
                            endRotate += 90;
                        }
                        else if ((groundRotation > player.groundRotation) || (groundRotation == 0 && player.groundRotation == 3))
                        {
                            endRotate -= 90;
                        }
                        else
                        {
                            endRotate += 90;
                        }
                        groundRotation = player.groundRotation;
                    }
                }

                //カメラの回転
                if(isCameraRotate)
                {
                    cameraRotateProgress += cameraRotateSpeed * Time.deltaTime;

                    if(cameraRotateProgress > 1)
                    {
                        cameraRotateProgress = 1;
                        isCameraRotate = false;
                    }

                    cameraRotationY = Mathf.Lerp(prevRotate, endRotate, cameraRotateProgress);
                }

                //Rotation Yを変更
                Camera.main.transform.rotation = Quaternion.Euler(cameraRotationX, cameraRotationY, 0);

                var position = Camera.main.transform.position;

                //カメラの向きに合わせてカメラ位置変更
                Vector3 forward = Camera.main.transform.forward;

                if (groundRotation == 0 || groundRotation == 2)
                {
                    position.x = player.curretCenter + forward.x * -cameraRange;
                    position.z = target.position.z + forward.z * -cameraRange;
                }
                else
                {
                    position.x = target.position.x + forward.x * -cameraRange;
                    position.z = player.curretCenter + forward.z * -cameraRange;
                }

                position.y = cameraPositionY;

                Camera.main.transform.position = position;
            }
        }
    }
}