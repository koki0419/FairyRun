using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  // ←追加

namespace RunGame.Title
{
    /// <summary>
    /// 『タイトル画面』のシーン遷移を制御します。
    /// </summary>
    public class SceneController : MonoBehaviour
    {
        [SerializeField]
        private GameObject quitDialog = null;
        private void ShowQuitDialogUI()
        {
            quitDialog.SetActive(true);
        }
        private void HydeQuitDialogUI()
        {
            quitDialog.SetActive(false);
        }
        [SerializeField]
        private Transform quitButtons = null;

        // private int selectButtinIndex = 0;

        enum SceneType
        {
            None,
            Nomal,
            Quit,
        }
        private SceneType sceneType = SceneType.Nomal;
        private void Start()
        {
            HydeQuitDialogUI();
            sceneType = SceneType.Nomal;
        }
        // Update is called once per frame
        void Update()
        {
            switch (sceneType)
            {
                case SceneType.Nomal:
                    // 「Enter」キーが押された場合
                    if (Input.GetKeyUp(KeyCode.Return) || Input.GetButtonDown("Player_1_Jump"))
                    {
                        // 『ステージ選択画面』へシーン遷移
                        SceneManager.LoadScene("SelectStage");
                    }
                    if (Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyUp(KeyCode.Joystick1Button7))
                    {
                        ShowQuitDialogUI();
                        sceneType = SceneType.Quit;
                    }
                    break;
                case SceneType.Quit:
                    if (Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button7))
                    {
                        HydeQuitDialogUI();
                        sceneType = SceneType.Nomal;
                    }
                    var selectKeyX = Input.GetAxis("Player_1_Move");
                    if (SelectButton(quitButtons, selectKeyX))
                    {
                        switch (selectedIndex)
                        {
                            case 0: // 終了
                                Application.Quit();
                                break;
                            case 1: // やめない
                                HydeQuitDialogUI();
                                sceneType = SceneType.Nomal;
                                break;
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        // 現在選択されているボタンを示すインデックス
        private int selectedIndex = 0;
        //前フレームのスティック入力値
        private float previewStickInput = 0;
        /// <summary>
        /// ボタン選択時の表示スケールを指定します。
        /// </summary>
        [SerializeField]
        private Vector3 selectedScale = new Vector3(1.1f, 1.1f, 1);
        bool SelectButton(Transform targetButtons, float select)
        {
            // 「Enter」キーが押された場合
            if (Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.Joystick1Button0))
            {
                HydeQuitDialogUI();
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

        /// <summary>
        /// 「StartButton」をクリックした際に
        /// 呼び出されます。
        /// </summary>
        public void OnClickStartButton()
        {
            // 『ステージ選択画面』へシーン遷移
            SceneManager.LoadScene("SelectStage");
        }
    }
}