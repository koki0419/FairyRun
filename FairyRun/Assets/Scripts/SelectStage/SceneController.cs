using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace RunGame.SelectStage
{
    /// <summary>
    /// 『ステージ選択画面』のシーン遷移を制御します。
    /// </summary>
    public class SceneController : MonoBehaviour
    {
        /// <summary>
        /// 「StageButton」の親オブジェクトを指定します。
        /// </summary>
        [SerializeField]
        private Transform buttons = null;
        /// <summary>
        /// ボタン選択時の表示スケールを指定します。
        /// </summary>
        [SerializeField]
        private Vector3 selectedScale = new Vector3(1.1f, 1.1f, 1);

        // 現在選択されているボタンを示すインデックス
        public int selectedIndex = 0;

        // totalのスコアポイント
        [SerializeField]
        int totalScorePoint = 0;
        public int debugTotalScorePoint = 0;
        // 現在の解放ステージを管理 ステージ１は常に解放状態
        public static int[] releaseStageManager =
        {
             1,
            -1,
            -1,
        };
        // 「releaseStageManager」と比較して「releaseStageManager[n]」= 1,「releaseStagechecker[n] = 0」の時
        // 解放アニメーション再生し解放
        // 解放後「releaseStagechecker」を更新
        public static int[] releaseStagechecker =
        {
             1,
            -1,
            -1,
        };

        // player１、２のアニメーション用
        [SerializeField]
        private CharacterIntroduction characterIntroduction = null;
        // 南京錠アニメーション
        [SerializeField]
        private Animator[] nankinjoAnimator = null;
        [SerializeField]
        private bool debug = false;
        enum SceneState
        {
            None,
            Release,
            Normal,
        }
        private SceneState sceneState = SceneState.None;

        //効果音再生用
        private SEManager seManager;
        //スティック入力処理の最適化用
        float previewStickInput = 0;
        [SerializeField]
        private Transform quitButtons = null;
        enum NomalSceneType
        {
            None,
            Nomal,
            Quit,
        }
        NomalSceneType nomalSceneType = NomalSceneType.Nomal;
        // Start is called before the first frame update
        void Start()
        {
            UiManager.Instance.HydeQuitDialogUI();
            // 現在までの獲得ポイントを取得します
            totalScorePoint = GameController.totalScorePoint;
            SetScorePoint(totalScorePoint);
            // SEマネージャー取得
            seManager = Camera.main.GetComponent<SEManager>();
            // GameControllerからステージ名一覧を取得
            var stageNames = GameController.Instance.StageNames;
            // buttons配列から各ボタンをループ処理
            for (int index = 0; index < buttons.childCount; index++)
            {
                var button = buttons.GetChild(index);
                // ボタンのテキストを修正
                button.GetComponentInChildren<Text>().text =
                    //string.Format("{0}.\n", index);
                string.Format("{0}", stageNames[index]);
            }
            // 各ステージ開放条件（Scoreポイント）をセット＆表示
            var releaseScorePoint = GameController.Instance.StageReleasePoint;
            UiManager.Instance.SetStageReleaseScoreImage(releaseScorePoint);

            // ----- 解放ステージを確認 -----
            var checkScorePoint = GameController.checkScorePoint;
            if (debug)
            {
                checkScorePoint = debugTotalScorePoint;
                totalScorePoint = debugTotalScorePoint;
                //releaseStagechecker = new int[]
                //{
                //    1,1,1
                //};
            }

            // 「checkReleasePoint」と「checkScorePoint」を比較
            // 「checkReleasePoint」<=「checkScorePoint」の時ステージ開放
            var checkScore = totalScorePoint;
            for (int i = 1; i < 3; i++)
            {
                var checkReleasePoint = GameController.Instance.StageReleasePoint[i];
                if (checkReleasePoint <= totalScorePoint)
                {
                    // 既に解放されていたらcontinue
                    if (releaseStageManager[i] == 1)
                    {
                        continue;
                    }
                    // 未開放なら「releaseStageManager[n] = 1」解放状態にする
                    // また、「sceneState = SceneState.Release」リリース状態に設定
                    releaseStageManager[i] = 1;
                    sceneState = SceneState.Release;
                }
                else
                {
                    releaseStageManager[i] = -1;
                }
            }
            // ステージ開放の確認→未開放なら南京錠画像を表示
            for (int i = 0; i < 3; i++)
            {
                // ステージ開放（南京錠取る）
                if (releaseStageManager[i] == 1 && releaseStagechecker[i] == 1)
                {
                    UiManager.Instance.HydeNankinjoImage(i);
                    UiManager.Instance.HydeReleaseScoreImage(i);
                }
                else
                {
                    UiManager.Instance.ShowNankinjoImage(i);
                    UiManager.Instance.ShowReleaseScoreImage(i);
                }
            }

            // 確認用のスコアポイントを更新
            GameController.checkScorePoint = totalScorePoint;
            if (sceneState != SceneState.Release)
                sceneState = SceneState.Normal;
        }

        // Update is called once per frame
        void Update()
        {
            switch (sceneState)
            {
                case SceneState.None:
                    break;
                case SceneState.Release:
                    for (int i = 0; i < 3; i++)
                    {
                        // 「releaseStageManager」と比較して「releaseStageManager[n]」= 1,「releaseStagechecker[n] = 0」の時
                        // 解放アニメーション再生し解放
                        // 解放後「releaseStagechecker」を更新
                        if (releaseStageManager[i] == 1 && releaseStagechecker[i] == 1)
                        {
                            continue;
                        }
                        else if (releaseStageManager[i] == 1 && releaseStagechecker[i] == -1)
                        {
                            // 解放 // 「releaseStagechecker」を更新
                            releaseStagechecker[i] = 1;
                            // アニメーション実行
                            nankinjoAnimator[i].SetTrigger("Release");
                            UiManager.Instance.HydeReleaseScoreImage(i);
                            //効果音ならす
                            seManager.PlaySound(SEManager.SEName.KEY);
                        }
                    }
                    // 解放後通常シーンに戻す
                    sceneState = SceneState.Normal;
                    break;
                case SceneState.Normal:
                    switch (nomalSceneType)
                    {
                        case NomalSceneType.Nomal:
                            var moveY = Input.GetAxis("Vertical");
                            // 「Enter」キーが押された場合
                            if (Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.Joystick1Button0))
                            {
                                // 『ステージ画面』へシーン遷移
                                Stage.SceneController.StageNo = selectedIndex;
                                if (selectedIndex == 0)
                                {
                                    SceneManager.LoadScene("Stage");
                                    return;
                                }
                                // ステージが解放されていれば遷移
                                if (CheckReleaseStage(selectedIndex))
                                {
                                    SceneManager.LoadScene(string.Format("Stage{0}", selectedIndex + 1));
                                    return;
                                }
                                Debug.LogWarning(string.Format("Stage{0}は解放されていません", selectedIndex));
                            }
                            // 上カーソルキーが押された場合
                            else if (IsInputUpKey(moveY))
                            {
                                selectedIndex--;
                                if (selectedIndex < 0)
                                {
                                    selectedIndex = 0;
                                }
                            }
                            // 下カーソルキーが押された場合
                            else if (IsInputDownKey(moveY))
                            {
                                selectedIndex++;
                                if (selectedIndex > buttons.childCount - 1)
                                {
                                    selectedIndex = buttons.childCount - 1;
                                }
                            }
                            // buttons配列から各ボタンをループ処理
                            for (int index = 0; index < buttons.childCount; index++)
                            {
                                var button = buttons.GetChild(index);
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
                            if (isSelectKey && ((moveY >= 0 && moveY < previewStickInput) || (moveY <= 0 && moveY > previewStickInput)))
                            {
                                isSelectKey = false;
                            }
                            //現在フレームのStickのY値を保存、次フレームで入力か非入力か判定用に使用
                            previewStickInput = moveY;
                            // Characterをアニメーションさせます
                            if (Input.GetKeyDown(KeyCode.J) || IsCheckPlayer_2_InputKey())
                            {
                                characterIntroduction.PlayAnimation_player_2();
                            }
                            if (Input.GetKeyDown(KeyCode.H) || IsCheckPlayer_1_InputKey())
                            {
                                characterIntroduction.PlayAnimation_player_1();
                            }
                            if (Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyUp(KeyCode.Joystick1Button7))
                            {
                                UiManager.Instance.ShowQuitDialogUI();
                                nomalSceneType = NomalSceneType.Quit;
                            }
                            break;
                        case NomalSceneType.Quit:
                            if (Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button7))
                            {
                                UiManager.Instance.HydeQuitDialogUI();
                                nomalSceneType = NomalSceneType.Nomal;
                            }
                            var selectKeyX = Input.GetAxis("Player_1_Move");
                            if (SelectButton(quitButtons, selectKeyX))
                            {
                                switch (selectedIndex)
                                {
                                    case 0: // やめない
                                        UiManager.Instance.HydeQuitDialogUI();
                                        nomalSceneType = NomalSceneType.Nomal;
                                        break;
                                    case 1: // 終了
                                        Application.Quit();
                                        break;
                                }
                            }
                            break;
                    }
                    
                    break;
                default:
                    break;
            }

        }
        bool SelectButton(Transform targetButtons, float select)
        {
            // 「Enter」キーが押された場合
            if (Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.Joystick1Button0))
            {
                UiManager.Instance.HydeQuitDialogUI();
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
        // 過去のｘ入力値（移動時の比較用）
        bool isSelectKey = false;
        // ステージ選択キー入力
        private bool IsInputDownKey(float moveY)
        {

            if (moveY < 0 && moveY < previewStickInput && !isSelectKey)
            {
                isSelectKey = true;
                return true;
            }
            return false;
        }
        private bool IsInputUpKey(float moveY)
        {

            if (moveY > 0 && moveY > previewStickInput && !isSelectKey)
            {
                isSelectKey = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// プレイヤー２のキャラクター確認用キー入力を確認します
        /// </summary>
        /// <returns></returns>
        private bool IsCheckPlayer_2_InputKey()
        {
            if (Input.GetKeyDown(KeyCode.Joystick2Button1) ||
                Input.GetKeyDown(KeyCode.Joystick2Button2) ||
            Input.GetKeyDown(KeyCode.Joystick2Button3))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// プレイヤー1キャラクター確認用のキー入力を確認します
        /// </summary>
        /// <returns></returns>
        private bool IsCheckPlayer_1_InputKey()
        {
            if (Input.GetKeyDown(KeyCode.Joystick1Button1) ||
                Input.GetKeyDown(KeyCode.Joystick1Button2) ||
            Input.GetKeyDown(KeyCode.Joystick1Button3))
            {
                return true;
            }
            return false;
        }
        // ステージが解放されているか確認
        private bool CheckReleaseStage(int stageNum)
        {
            if (releaseStageManager[stageNum] == 1)
                return true;
            else
                return false;
        }

        // Scoreポイントのセット
        private void SetScorePoint(int score)
        {
            UiManager.Instance.scoreUITextUpdate(score);
        }
    }
}