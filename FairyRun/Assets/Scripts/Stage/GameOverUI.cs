using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RunGame.Stage
{
    /// <summary>
    /// ゲームオーバーの表示と入力を処理します。
    /// </summary>
    public class GameOverUI : MonoBehaviour
    {
        /// <summary>
        /// 「GameOver」UIを指定します。
        /// </summary>
        [SerializeField]
        private GameObject gameOver = null;

        /// <summary>
        /// 「RETRY」ボタンと「TITLE」ボタンを指定します。
        /// </summary>
        /// <remarks>
        /// buttons[0]   retryButton
        /// buttons[1]   titleButton
        /// </remarks>
        [SerializeField]
        private Transform[] buttons = null;

        // 0の場合はretryButton、1の場合はtitleButton
        int selectedIndex = 0;

        /// <summary>
        /// 「GameOver」ダイアログを表示します。
        /// </summary>
        public void Show()
        {
            // ゲームオーバー演出のコルーチンを開始
            StartCoroutine(OnGameOver());
        }

        /// <summary>
        /// GameOverステートの際のフレーム更新処理です。
        /// </summary>
        IEnumerator OnGameOver()
        {
            gameOver.SetActive(true);
            yield return new WaitForSeconds(1);

            while (true)
            {
                // 左カーソルキーが押された場合
                if (Input.GetKeyUp(KeyCode.LeftArrow))
                {
                    selectedIndex--;
                }
                // 右カーソルキーが押された場合
                else if (Input.GetKeyUp(KeyCode.RightArrow))
                {
                    selectedIndex++;
                }
                // 「Enter」キーが押された場合
                else if (Input.GetKeyUp(KeyCode.Return))
                {
                    // 「RETRY」選択中
                    if (selectedIndex == 0)
                    {
                        // 現在起動しているシーンを再読み込み
                        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                        break;
                    }
                    // 「TITLE」選択中
                    else if (selectedIndex == 1)
                    {
                        SceneManager.LoadScene("Title");
                        break;
                    }
                }

                // 0～ボタン配列の最大数から飛び出ないように
                selectedIndex = Mathf.Clamp(selectedIndex, 0, buttons.Length - 1);
                // ボタン配列のすべての要素を繰り返し処理
                for (int index = 0; index < buttons.Length; index++)
                {
                    // 選択中のボタンのみ拡大
                    if (index == selectedIndex)
                    {
                        buttons[index].localScale = new Vector3(1.3f, 1.3f, 1);
                    }
                    else
                    {
                        buttons[index].localScale = Vector3.one;
                    }
                }

                yield return null;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
           // gameOver.SetActive(false);
        }
    }
}
