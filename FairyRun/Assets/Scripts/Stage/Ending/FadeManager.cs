using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class FadeManager : MonoBehaviour
{

    [SerializeField]
    private Image image = null;
    private float r = 0.0f, g = 0.0f, b = 0.0f;
    private float alpha = 0;

    /// <summary>
    /// フェードアウト、インの初期設定 SetImage(Color.clear);
    /// </summary>
    /// <param name="setColor">初期カラー</param>
    public void SetImage(Color setColor)
    {
        var color = image.color;
        color = setColor;
        r = color.r;
        g = color.g;
        b = color.b;
        alpha = color.a;
        image.color = color;
        // Imageを最前表示にする
        image.gameObject.transform.SetAsLastSibling();
        image.gameObject.SetActive(true);
    }
    /// <summary>
    /// フェードアウトの処理 FadeOut(Color.white, 0.5f);
    /// </summary>
    /// <param name="targetColor">最終的な色</param>
    /// <param name="fadeSpeed">フェードアウトするときの速さ</param>
    /// <returns>"フェード中 = FALSE" "終了 = TRUE" </returns>
    public bool FadeOut(Color targetColor, float fadeSpeed)
    {
        Debug.Log("これ");
        if (alpha <= 1.0f)
        {
            image.color = new Color(targetColor.r, targetColor.g, targetColor.b, alpha);
            alpha += (fadeSpeed * Time.deltaTime);
            return false;
        }
        return true; // 終了
    }

    /// <summary>
    /// フェードインの処理 FadeIn(Color.white, 0.5f);
    /// </summary>
    /// <param name="targetColor">最終的な色</param>
    /// <param name="fadeSpeed">フェードインするときの速さ</param>
    /// <returns>"フェード中 = FALSE" "終了 = TRUE" </returns>
    public bool FadeIn(Color targetColor, float fadeSpeed)
    {
        if (alpha >= 0.0f)
        {
            image.color = new Color(r, g, b, alpha);
            alpha -= (fadeSpeed * Time.deltaTime);
            return false;
        }
        //表示を最奥に
        image.gameObject.transform.SetAsFirstSibling();
        image.gameObject.SetActive(false);
        return true;// 終了
    }
}
