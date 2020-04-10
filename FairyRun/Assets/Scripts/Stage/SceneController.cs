using System.Collections;   // コルーチンのため
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RunGame.Stage
{
    /// <summary>
    /// 『ステージ画面』のシーン遷移を制御します。
    /// </summary>
    public class SceneController : MonoBehaviour
    {
        #region インスタンスへのstaticなアクセスポイント
        /// <summary>
        /// このクラスのインスタンスを取得します。
        /// </summary>
        public static SceneController Instance
        {
            get { return instance; }
        }
        static SceneController instance = null;

        /// <summary>
        /// Start()より先に実行されます。
        /// </summary>
        private void Awake()
        {
            instance = this;
            if (isDebug)
            {
                stageNo = debugStageNum - 1;
            }
        }
        #endregion

        /// <summary>
        /// 起動するシーン番号を取得または設定します。
        /// </summary>
        public static int StageNo
        {
            get { return stageNo; }
            set { stageNo = value; }
        }
        private static int stageNo = 0;

        /// <summary>
        /// プレハブからステージを生成する場合はtrueを指定します。
        /// </summary>
        /// <remarks>ステージ開発用のシーンではfalseに設定します。</remarks>
        [SerializeField]
        private bool instantiateStage = true;

        /// <summary>
        /// ステージ開始からの経過時間(秒)を取得します。
        /// </summary>
        public float PlayTime { get; private set; }
        //public float PlayTime {
        //    get { return playTime; }
        //    private set { playTime = value; }
        //}
        //float playTime = 0;

        //  敵倒した数
        private int enemyDefeatedCount = 0;
        public int EnemyDefeatedCount
        {
            get { return enemyDefeatedCount; }
            set { enemyDefeatedCount = value; }
        }
        // スコアポイント
        public static int scoerPoint = 0;
        // 妖精攻撃ポイント
        public static int fairyPoint = 0;
        // 起動しているOnPlay()コルーチン
        Coroutine playState = null;
        // 外部のゲームオブジェクトの参照変数
        Player player;
        ResultManager resultManager;
        TutorialManager tutorialManager = null;
        // チュートリアルを開始するかどうか
        public bool isTutorial;
        // プレイ中かどうか
        public static bool isPlaying;
        // クリア後の移動先（遷移先）
        private const string NEXTSCENEAFTERCLEARING = "SelectStage";
        // ロード先のパスを取得
        private const string stageDataLoadPath = "Datas/StageData/";
        // マップデータの名前を取得
        private const string stageMapDataName = "_New_stageData_";
        [SerializeField] private StageData stageData = null;
        [SerializeField]
        private bool isDebug = false;

        [SerializeField]
        private int debugStageNum = 0;

        private EndingManager endingManager = null;
        public static bool isEnding = false;
        // goalの位置指定
        [SerializeField]
        private Vector3 goalPos = Vector3.zero;
        // goalの向き（ｙ）
        [SerializeField]
        private float goalRotateY = 0;
        // goalオブジェクト取得
        [SerializeField]
        private GameObject goalObj = null;

        //効果音再生用
        private SEManager seManager;
        private void SetGoalObj()
        {
            if(StageNo != 2)
            {
                goalObj = Resources.Load("Goal_close Variant") as GameObject;
            }
            else
            {
                goalObj = Resources.Load("Goal Variant") as GameObject;
            }
            var newObj = Instantiate(goalObj);
            newObj.transform.position = goalPos;
            newObj.transform.Rotate(0, goalRotateY, 0);
        }
        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        void Start()
        {
            // 他のゲームオブジェクトを参照
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            resultManager = GameObject.Find("ResultManager").GetComponent<ResultManager>();
            tutorialManager = UiManager.Instance.tutorialManager;
            endingManager = GetComponent<EndingManager>();
            scoerPoint = 0;
            fairyPoint = 0;
            isPlaying = false;
            isEnding = false;
            seManager = Camera.main.GetComponent<SEManager>();

            UiManager.Instance.HydeEndingMessageText();
            // 初期化
            endingMessage = new Item_String[endingMessageCount];
            endingMessage[0] = Resources.Load("EndingMessage/Ending_1") as Item_String;
            if (!Ending)
            {
                if (stageNo + 1 == 3)
                {
                    Ending = true;
                }
            }
            ending = Ending;
            // ステージプレハブを読み込む
            if (instantiateStage)
            {
                var stageName = string.Format(stageMapDataName + (stageNo + 1).ToString());
                stageData = Resources.Load(stageDataLoadPath + stageName) as StageData;
                // ステージ作成
                MapCreate(stageData);
            }
            // goal位置と回転（向き）を取得
            goalPos = GameController.Instance.GoalPos[stageNo];
            goalRotateY = GameController.Instance.GoalRotate_Y[stageNo];
            SetGoalObj();
            // UIを非表示処理
            UiManager.Instance.HydeResultUI();
            UiManager.Instance.HydeTutorialtUI();
            UiManager.Instance.HydeTimeUI();
            // それぞれのステージ用のBGMを再生
            AudioClip clip = null;
            // bgmを読み込む
            if (stageNo == GameController.Instance.StageNames.Length - 1)
            {
                // 最終ステージの場合
                clip = Resources.Load<AudioClip>("bgm_02");
            }
            else
            {
                // 通常ステージの場合
                clip = Resources.Load<AudioClip>("bgm_01");
            }
            var bgmAudio = Camera.main.GetComponent<AudioSource>();
            // bgmAudio.clip = clip;
            // bgmAudio.Play();
            StartCoroutine(OnStart());
        }
        /// <summary>
        /// 「TutorialButton」の親オブジェクトを指定します。
        /// </summary>
        [SerializeField]
        private Transform tutorialButtons = null;
        /// <summary>
        /// ボタン選択時の表示スケールを指定します。
        /// </summary>
        [SerializeField]
        private Vector3 selectedScale = new Vector3(1.1f, 1.1f, 1);
        // 現在選択されているボタンを示すインデックス
        public int selectedIndex = 0;

        // 「pauseボタン」
        [SerializeField]
        private Transform pauseButtons = null;

        //前フレームのスティック入力値
        private float previewStickInput = 0;
        // エンディングメッセージ
        [SerializeField]
        private int endingMessageCount = 0;
        private Item_String[] endingMessage = null;

        bool SelectButton(Transform targetButtons, float select)
        {
            // 「Enter」キーが押された場合
            if (Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.Joystick1Button0))
            {
                UiManager.Instance.HydeTutorialDialogUI();
                return true;
            }
            // 上カーソルキーが押された場合
            else if (IsInputUpKey(select))
            {
                selectedIndex--;
                if (selectedIndex < 0)
                {
                    selectedIndex = 0;
                }
            }
            // 下カーソルキーが押された場合
            else if (IsInputDownKey(select))
            {
                selectedIndex++;
                if (selectedIndex > targetButtons.childCount - 1)
                {
                    selectedIndex = targetButtons.childCount - 1;
                }
            }
            // buttons配列から各ボタンをループ処理
            for (int index = 0; index < targetButtons.childCount; index++)
            {
                var button = targetButtons.GetChild(index);
                // 選択中のボタンは拡大表示
                if (index == selectedIndex)
                {
                    button.localScale = selectedScale;
                }
                // 非選択中のボタンは通常表示
                else
                {
                    button.localScale = Vector3.one;
                }
            }
            if (isSelectKey && ((select >= 0 && select < previewStickInput) || (select <= 0 && select > previewStickInput)))
            {
                isSelectKey = false;
            }
            previewStickInput = select;
            return false;
        }
        /// <summary>
        /// コルーチンを使ったカウントダウン演出
        /// </summary>
        IEnumerator OnStart()
        {
            yield return new WaitForSeconds(1); // 1秒待機
            if (StageNo == 0)
            {
                UiManager.Instance.ShowTutorialDialogUI();
                while (true)
                {
                    var moveX = Input.GetAxis("Player_1_Move");
                    // ボタンセレクト終了後の処理
                    if (SelectButton(tutorialButtons, moveX))
                    {
                        switch (selectedIndex)
                        {
                            case 0:
                                isTutorial = true;
                                break;
                            case 1:
                                isTutorial = false;
                                break;
                        }
                        break;
                    }
                    yield return null;
                }
            }

            if (isTutorial)
            {
                UiManager.Instance.ShowTutorialUI();
                GetTutorialText(tutorialTextNum);
                isShowMessage = true;
                tutorialTimer = tutorialManager.GetTutorialShowTime;
                player.IsMoving = false;
                playState = StartCoroutine(OnTutorial());
            }
            else
            {
                UiManager.Instance.ShowMessage("ＲＥＡＤＹ？");
                yield return new WaitForSeconds(1); // 1秒待機
                UiManager.Instance.ShowMessage("ＧＯ");
                yield return new WaitForSeconds(1); // 1秒待機
                UiManager.Instance.HideMessage();
                UiManager.Instance.ShowTimeUI();
                // ステージをプレイ開始
                playState = StartCoroutine(OnPlay());
            }
            isPlaying = true;
        }
        bool isPause = false;
        // ポーズボタンを押したPlayerを取得
        // -1 = Player_1 // 1 = Player_2 // 0 :押されていない
        int pauseButtonPersonWhoPushed = 0;
        // キー入力確認
        bool isInKey = false;
        /// <summary>
        /// Playステートの際のフレーム更新処理です。
        /// </summary>
        IEnumerator OnPlay()
        {
            // player.IsActive = true;
            while (true)
            {
                isInKey = false;
                if (isPause)
                {
                    // プレイヤー_1
                    if (pauseButtonPersonWhoPushed == -1)
                    {
                        var moveX = Input.GetAxis("Player_1_Move");
                        // ボタンセレクト終了後の処理
                        if (SelectButton(pauseButtons, moveX))
                        {
                            switch (selectedIndex)
                            {
                                case 0:// タイトル
                                    SceneManager.LoadScene("SelectStage");
                                    break;
                                case 1:// リトライ
                                    var sceneName = SceneManager.GetActiveScene().name;
                                    SceneManager.LoadScene(sceneName);
                                    break;
                            }
                        }
                        // Pause終了
                        if (Input.GetKeyDown(KeyCode.Joystick1Button7))
                        {
                            isInKey = true;
                            isPause = false;
                            isPlaying = true;
                            selectedIndex = 0;
                            pauseButtonPersonWhoPushed = 0;
                            UiManager.Instance.HydePauseDialogUI();
                        }
                    }// プレイヤー_2
                    else if (pauseButtonPersonWhoPushed == 1)
                    {
                        var moveX = Input.GetAxis("Player_2_Move");
                        // ボタンセレクト終了後の処理
                        if (SelectButton(pauseButtons, moveX))
                        {
                            switch (selectedIndex)
                            {
                                case 0:// タイトル
                                    SceneManager.LoadScene("SelectStage");
                                    break;
                                case 1:// リトライ
                                    var sceneName = SceneManager.GetActiveScene().name;
                                    SceneManager.LoadScene(sceneName);
                                    break;
                            }
                        }
                        // Pause終了
                        if (Input.GetKeyDown(KeyCode.Joystick2Button7))
                        {
                            isInKey = true;
                            isPause = false;
                            isPlaying = true;
                            selectedIndex = 0;
                            pauseButtonPersonWhoPushed = 0;
                            UiManager.Instance.HydePauseDialogUI();
                        }
                    }
                }

                if (isPlaying)
                {
                    PlayTime += Time.deltaTime;
                    // Pause
                    if (Input.GetKeyDown(KeyCode.Joystick1Button7) && !isInKey)
                    {
                        pauseButtonPersonWhoPushed = -1;
                        Debug.Log("Player1");
                    }
                    else if (Input.GetKeyDown(KeyCode.Joystick2Button7) && !isInKey)
                    {
                        pauseButtonPersonWhoPushed = 1;
                        Debug.Log("Player2");
                    }

                    if ((pauseButtonPersonWhoPushed == -1 || pauseButtonPersonWhoPushed == 1) && !isInKey)
                    {
                        Debug.Log("Pause");
                        isPlaying = false;
                        isPause = true;
                        selectedIndex = 0;
                        UiManager.Instance.ShowPauseDialogUI();
                    }
                }

                yield return null;
            }
        }
        IEnumerator OnTutorial()
        {
            while (true)
            {
                Tutorial();
                if (endTutorial)
                {
                    break;
                }
                yield return null;
            }
            // ここまで来たらOnPlay()を呼び出す
            playState = StartCoroutine(OnPlay());
        }
        bool endTutorial = false;
        // 過去のｘ入力値（移動時の比較用）
        bool isSelectKey = false;
        // ステージ選択キー入力
        private bool IsInputDownKey(float moveX)
        {
            if (moveX > 0 && moveX > previewStickInput && !isSelectKey)
            {
                isSelectKey = true;
                return true;
            }

            return false;
        }
        private bool IsInputUpKey(float moveX)
        {
            if (moveX < 0 && moveX < previewStickInput && !isSelectKey)
            {
                isSelectKey = true;
                return true;
            }

            return false;
        }

        // チュートリアルをいくつ終了したかでチュートリアルの進行状況を確認します。
        int tutorialCheckNum = 0;
        // 表示するメッセージ番号
        int tutorialTextNum = 0;
        float tutorialTimer = 0.0f;// チュートリアル画面表示非表示カウンター
        // メッセージが表示されているか→表示状態ならカウンターを起動させる
        bool isShowMessage = false;
        bool isHydeTime = false;
        bool waitTime = false;
        public int tutoralCount = 0;
        private void Tutorial()
        {
            if (isShowMessage)
            {
                tutorialTimer -= Time.deltaTime;
                // チュートリアルメッセージ表示状態で待機中
                if (tutorialTimer <= 0)
                {
                    // メッセージを非表示
                    UiManager.Instance.HydeTutorialtUI();
                    // 次のチュートリアルメッセージをセット
                    tutorialTextNum++;
                    GetTutorialText(tutorialTextNum);
                    isShowMessage = false;
                    player.IsMoving = true;
                    // チュートリアル1の時は3秒後テキスト切替
                    if (tutorialCheckNum == 0)
                    {
                        // Eventクリアチェック
                        tutorialManager.tutotialEventManagement[tutorialCheckNum] = 1;
                        // 次のチュートリアル
                        tutorialCheckNum++;
                        // メッセージを表示
                        UiManager.Instance.ShowTutorialUI();
                        // タイマーセット
                        tutorialTimer = tutorialManager.GetTutorialShowTime;
                        // 表示フラグをオン
                        isShowMessage = true;
                        player.IsMoving = false;
                    }
                    else if (tutorialCheckNum == 1)
                    {
                        isHydeTime = true;
                        player.IsMoving = true;
                        tutorialTimer = tutorialManager.GetTutorialHydeTime;
                    }
                    else if (tutorialCheckNum == 4)
                    {
                        // Eventクリアチェック
                        tutorialManager.tutotialEventManagement[tutorialCheckNum] = 1;
                    }
                }
            }

            // チュートリアル進行状況確認
            tutoralCount = tutorialManager.tutotialEventManagement.Length;
            var check = 0;
            for (int i = 0; i < tutorialManager.tutotialEventManagement.Length; i++)
            {
                if (tutorialManager.tutotialEventManagement[i] == 1) check++;
            }
            if (check == tutoralCount)
            {
                // ステージをプレイ開始
                endTutorial = true;
                isShowMessage = false;
                player.IsMoving = true;
                UiManager.Instance.ShowTimeUI();
                StopCoroutine(playState);
                return;
            }

            // 6秒間自由タイム
            if (isHydeTime)
            {
                tutorialTimer -= Time.deltaTime;
                if (tutorialTimer <= 0)
                {
                    tutorialManager.tutotialEventManagement[tutorialCheckNum] = 1;
                    // 次のチュートリアル
                    tutorialCheckNum++;
                    // メッセージを表示
                    UiManager.Instance.ShowTutorialUI();
                    // タイマーセット
                    tutorialTimer = tutorialManager.GetTutorialShowTime;
                    // 表示フラグをオン
                    isShowMessage = true;
                    isHydeTime = false;
                    player.IsMoving = false;
                }
            }
            // イベント3,4,5（アイテム獲得待ち）
            if (!waitTime)
            {
                if (tutorialCheckNum == 2 && fairyPoint > 0 || tutorialCheckNum == 3 && enemyDefeatedCount > 0 || tutorialCheckNum == 4 && scoerPoint > 0)
                {
                    tutorialTimer = tutorialManager.GetTutorialWaitTime;
                    waitTime = true;
                }
            }
            else
            {
                // アイテム獲得後ｎ秒後次のチュートリアルを表示 
                // [tutorialCheckNum == 2,3,4]の時
                tutorialTimer -= Time.deltaTime;
                if (tutorialTimer <= 0)
                {
                    // Eventクリアチェック
                    tutorialManager.tutotialEventManagement[tutorialCheckNum] = 1;
                    if (tutorialCheckNum != 4)
                    {
                        // 次のチュートリアル
                        tutorialCheckNum++;
                        // メッセージを表示
                        UiManager.Instance.ShowTutorialUI();
                        // タイマーセット
                        tutorialTimer = tutorialManager.GetTutorialShowTime;
                        // 表示フラグをオン
                        isShowMessage = true;
                        waitTime = false;
                        player.IsMoving = false;
                    }
                }
            }
        }
        private void GetTutorialText(int tutorialNum)
        {
            UiManager.Instance.SetTutorialText(tutorialNum);
        }
        /// <summary>
        /// ステージをクリアーさせます。
        /// </summary>
        public void StageClear()
        {
            // 現在のコルーチンを停止
            if (playState != null)
            {
                StopCoroutine(playState);
            }
            //UiManager.Instance.HydeTimeUI();
            // リザルトを初期化
            resultManager.SetResultData(stageNo, PlayTime, enemyDefeatedCount, scoerPoint);
            //resultManager.SetResultData(stageNo, PlayTime, 150, 500); // デバック用
            UiManager.Instance.SetResultUIState(resultManager.StageNum, resultManager.ClearTime, resultManager.EnemyDefeatedCount, resultManager.GetScorePoint, resultManager.AllScorePoint);
            GameController.totalScorePoint += resultManager.AllScorePoint;
            //player.IsActive = false;
            // ステージクリアー演出のコルーチンを開始
            StartCoroutine(OnStageClear());
        }
        public static bool ending = false;
        [SerializeField]
        private bool Ending = false;
        public static bool playEnding = false;
        /// <summary>
        /// StageClearステートの際のフレーム更新処理です。
        /// </summary>
        IEnumerator OnStageClear()
        {
            //ステージクリア効果音再生
            seManager.PlaySound(SEManager.SEName.CLEAR);
            // リザルト表示
            UiManager.Instance.ShowResultUI();
            playEnding = false;
            // リザルト評価用
            while (true)
            {
                if (Input.GetKeyUp(KeyCode.Joystick1Button0) || Input.GetKeyUp(KeyCode.Return))
                {
                    playEnding = true;
                    break;
                }
                yield return null;  // 次のフレームまで待機
            }
            if (ending)
            {
                UiManager.Instance.HydeResultUI();
                SetEnding();
                // リザルトアニメーション（キャラ）終了待ち無限ループ
                while (true)
                {
                    if (isEnding)
                    {
                        var check = endingManager.FadeOut(Color.white, 0.5f);
                        if (check)
                        {
                            // ステージ番号を伝えてから「Result」を読み込む
                            Result.SceneController.StageNo = StageNo;
                            Result.SceneController.ClearTime = PlayTime;
                            UiManager.Instance.SetEndingMessage(endingMessage[0].itemEffect);
                            UiManager.Instance.ShowEndingMessageText();
                            break;
                        }
                    }
                    yield return null;  // 次のフレームまで待機
                }
            }
            // リザルトアニメーション（評価）終了待ち無限ループ
            while (true)
            {
                // アニメーション終了を感知後キー入力で遷移
                // 「Enter」キーが押された場合
                if (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Return))
                {
                    SceneManager.LoadScene(NEXTSCENEAFTERCLEARING);
                    break;
                }
                yield return null;  // 次のフレームまで待機
            }
            yield return new WaitForSeconds(1);
        }
        // 最終ステージクリア時
        private void SetEnding()
        {
            // Warpgateセット
            endingManager.SetWarpGate();
            // FadeOut前の処理
            endingManager.SetEnding(Color.clear);
        }
        /// <summary>
        /// マップの生成
        /// </summary>
        private void MapCreate(StageData stageData)
        {
            // タイルの生成
            int width = 0, hight = 0;
            int widthPos = 0, hightPos = 0;
            try
            {
                for (int zIndex = 0; zIndex < stageData.hight; zIndex++)
                {
                    hight++;
                    hightPos++;
                    width = 0;
                    widthPos = 0;
                    for (int xIndex = 0; xIndex < stageData.width; xIndex++)
                    {
                        width++;
                        var index = zIndex * stageData.width + xIndex;
                        if (stageData.internalData[index] != -1)
                        {
                            var newTile = Instantiate(stageData.TileTips[stageData.internalData[index]], transform);
                            if (stageNo == 0)
                            {
                                newTile.transform.position = new Vector3(widthPos * 10 - 20, 0, hightPos * 10 - 110);
                            }
                            else
                            {
                                newTile.transform.position = new Vector3(widthPos * 10 - 20, 0, hightPos * 10 - 20);
                            }
                            var num = stageData.internalData[index];
                            SetTile(newTile, num);
                        }
                        widthPos++;
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.Log("エラーメッセージ : " + e.Message);
                Debug.Log("hight : " + hight + "width : " + width);
                throw;
            }
        }
        void SetTile(GameObject target, int num)
        {
            var setRotate = Vector3.zero;
            var scale = target.GetComponent<Transform>().localScale;
            scale = new Vector3(1.0f, 1.0f, 1.0f);
            switch (num)
            {
                case 0:// 草むら 済み
                    var rand = Random.Range(0, 3);
                    var rotateY = 0.0f;
                    switch (rand)
                    {
                        case 0:
                            rotateY = 0.0f;
                            break;
                        case 1:
                            rotateY = 90.0f;
                            break;
                        case 2:
                            rotateY = -90.0f;
                            break;
                        case 3:
                            rotateY = 180.0f;
                            break;
                        default:
                            break;
                    }
                    target.transform.Rotate(0.0f, rotateY, 0.0f);
                    break;
                case 1:// 道（縦） 済み
                    target.transform.Rotate(0.0f, 0.0f, 0.0f);
                    break;
                case 2:// 道（横） 済み
                    target.transform.Rotate(0.0f, 90.0f, 0.0f);
                    break;
                case 3:// 行き止まり（上） 済み
                    target.transform.Rotate(0.0f, 180.0f, 0.0f);
                    break;
                case 4:// 曲がり角（下右） 済み
                    target.transform.Rotate(0.0f, 90.0f, 0.0f);
                    target.GetComponentInChildren<CurvePoint>().SetNextCurve(2, 0);
                    break;
                case 5:// 曲がり角（右下） 済み
                    target.transform.Rotate(0.0f, 0.0f, 0.0f);
                    scale = new Vector3(-1.0f, 1.0f, -1.0f);
                    if (StageNo == 0)
                    {
                        target.GetComponentInChildren<CurvePoint>().SetNextCurve(2, 1);
                    }
                    else
                    {
                        target.GetComponentInChildren<CurvePoint>().SetNextCurve(0, 1);
                    }
                    break;
                case 6:// 行き止まり（左） 済み
                    target.transform.Rotate(0.0f, 90.0f, 0.0f);
                    break;
                case 7:// 行き止まり（右） 済み
                    target.transform.Rotate(0.0f, -90.0f, 0.0f);
                    break;
                case 8:// 曲がり角（上右） 済み
                    target.transform.Rotate(0.0f, 0.0f, 0.0f);
                    if (StageNo == 0)
                    {
                        target.GetComponentInChildren<CurvePoint>().SetNextCurve(0, 2);
                    }
                    else
                    {
                        target.GetComponentInChildren<CurvePoint>().SetNextCurve(2, 2);
                    }
                    break;
                case 9:// 曲がり角（左下）済み
                    target.transform.Rotate(0.0f, -90.0f, 0.0f);
                    target.GetComponentInChildren<CurvePoint>().SetNextCurve(0, 3);
                    break;
                case 10:// 行き止まり（下） 済み
                    target.transform.Rotate(0.0f, 0.0f, 0.0f);
                    break;
                default:
                    break;
            }
            target.GetComponent<Transform>().localScale = scale;
        }
    }
}