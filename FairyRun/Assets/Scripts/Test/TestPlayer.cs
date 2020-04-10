using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private GameObject fairyAttackObj = null;
    [SerializeField]
    private Animator playerAnimator = null;

    private PlayerAnimationChecker animationChecker = null;
    bool ending = false;

    enum PlayerType
    {
        None,
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
    // Start is called before the first frame update
    void Start()
    {
        playerType = PlayerType.Play;
        animationChecker = GetComponentInChildren<PlayerAnimationChecker>();
    }
    Vector3 firstPoint = Vector3.zero;
    Vector3 thirdPoint = Vector3.zero;
    [SerializeField]
    float firstPosDistance = 2.0f;
    [SerializeField]
    float thirdPosDistance = 2.0f;
    // Update is called once per frame
    void Update()
    {
        var pos = gameObject.transform.position;
        switch (playerType)
        {
            case PlayerType.Play:
                pos.z += speed * Time.deltaTime;
                gameObject.transform.position = pos;

                if (Input.GetKeyDown(KeyCode.L))
                {
                    var newObj = Instantiate(fairyAttackObj, transform);
                    newObj.SetActive(true);
                    newObj.GetComponent<Bullet>().ShootInit();
                }
                if (Input.GetKeyDown(KeyCode.I))
                {
                    playerType = PlayerType.Ending;
                    endingScene = EndingScene.FirstPhase;
                    firstPoint = transform.position;
                    firstPoint.z += firstPosDistance;
                    return;
                }
                break;
            case PlayerType.Ending:
                switch (endingScene)
                {
                    case EndingScene.None:
                        break;
                    case EndingScene.FirstPhase:
                        playerAnimator.SetTrigger("Ending");
                        pos.z += speed * Time.deltaTime;
                        gameObject.transform.position = pos;
                        // 指定したポジションに着いたら「バイバイ」を再生
                        if(firstPoint.z <= gameObject.transform.position.z)
                        {
                            endingScene = EndingScene.SecondPhase;
                            thirdPoint = transform.position;
                            thirdPoint.z += thirdPosDistance;
                            transform.Rotate(0, 180, 0);
                        }
                        break;
                    case EndingScene.SecondPhase:
                        playerAnimator.SetTrigger("EndingNext_2");
                        if (animationChecker.GetBaibaiFinish)
                        {
                            animationChecker.GetBaibaiFinish = false;
                            endingScene = EndingScene.ThirdPhase;
                            transform.Rotate(0, 180, 0);
                            return;
                        }
                        break;
                    case EndingScene.ThirdPhase:
                        pos.z += speed * Time.deltaTime;
                        gameObject.transform.position = pos;
                        // 指定したポジションに着いたら「バイバイ」を再生
                        if (thirdPoint.z <= gameObject.transform.position.z)
                        {
                            Debug.Log("FadeOut");
                        }
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
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
            RunGame.Stage.SceneController.Instance.StageClear();
        }
    }
}
