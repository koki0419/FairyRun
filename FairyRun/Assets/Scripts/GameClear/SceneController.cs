using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RunGame.GameClear
{
    /// <summary>
    /// 『ゲームクリアー画面』のシーン遷移を制御します。
    /// </summary>
    public class SceneController : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            // 「Enter」キーが押された場合
            if (Input.GetKeyUp(KeyCode.Return))
            {
                // 『タイトル画面』へシーン遷移
                SceneManager.LoadScene("Title");
            }
        }
    }
}