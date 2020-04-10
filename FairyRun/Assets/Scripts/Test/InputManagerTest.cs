using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManagerTest : MonoBehaviour
{
    // 各ボタンのコード名
    private const string Player_1_Move = "Player_1_Move";
    private const string Player_2_Move = "Player_2_Move";
    private const string Player_1_Jump = "Player_1_Jump";
    private const string Player_2_Attack = "Player_2_Attack";

    public GameObject itemObj;
    public Items itemData = null;
    #region
    // デバッグ用のGUIエディターと各処理のテキスト表示用変数
    private GUIStyle style;
    private string debugText;
    private string debugText2;
    private string debugText3;
    private string debugText4;

    // Start is called before the first frame update
    void Start()
    {
        // GUIのフォントや文字サイズなどを変更するために
        // 「GUIStyle」を取得
        style = new GUIStyle();
        style.fontSize = 100;

        itemData = Resources.Load("ItemData") as Items;
    }
    #endregion
    // Update is called once per frame
    void Update()
    {
        // ------***プレイヤーボタン***------
        // プレイヤーのジャンプボタン
        if (Input.GetButtonDown(Player_1_Jump))
        {
            debugText3 = "プレイヤー1がジャンプしやがった";
        }
        // プレイヤーの移動ボタン
        var x = Input.GetAxis(Player_1_Move);
        if (x < 0)
        {
            debugText = "プレイヤー1の左移動";
        }
        else if (x > 0)
        {
            debugText = "プレイヤー1の右移動";
        }
        else
        {
            debugText = "";
        }
        // ------***妖精ボタン***------
        // 妖精の攻撃ボタン
        if (Input.GetButtonDown(Player_2_Attack))
        {
            debugText4 = "妖精の攻撃！！！！";
        }
        // 妖精の移動ボタン
        var x2 = Input.GetAxis(Player_2_Move);
        if (x2 < 0)
        {
            debugText2 = "プレイヤー2の左移動";
        }
        else if (x2 > 0)
        {
            debugText2 = "プレイヤー2の右移動";
        }
        else
        {
            debugText2 = "";
        }
        #region
        if (Input.GetButtonUp(Player_1_Jump))
        {
            debugText3 = "";
        }
        if (Input.GetButtonUp(Player_2_Attack))
        {
            debugText4 = "";
        }
        #endregion


        // ------***移動ボタン***------

    }
    void OnGUI()
    {
        // 各ボタンを押したときの反応をデバック形式で表示
        Rect rect = new Rect(10, 10, 400, 300);
        GUI.Label(rect, debugText, style);// プレイヤー1移動
        rect.y = 125;
        GUI.Label(rect, debugText2, style);// プレイヤー2移動
        rect.y = 225;
        GUI.Label(rect, debugText3, style);// プレイヤー1ジャンプ
        rect.y = 325;
        GUI.Label(rect, debugText4, style);// プレイヤー2攻撃
    }
}
