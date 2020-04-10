using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingManager : MonoBehaviour
{
    // フェードマネージャー
    [SerializeField]
    FadeManager fadeManager = null;
    // 異空間ゲート（チューブ）の位置座標
    [SerializeField]
    private Vector3 tubePos = Vector3.zero;
    // 異空間ゲート（チューブ）
    [SerializeField]
    private GameObject gateObj = null;

    private void Start()
    {
        var tubePrefab = Resources.Load("Gate Variant");
        gateObj = Instantiate(tubePrefab) as GameObject;
        gateObj.SetActive(false);
    }

    public void SetWarpGate()
    {
        gateObj.transform.position = tubePos;
        gateObj.SetActive(true);
    }
    public void SetEnding(Color targetColor)
    {
        fadeManager.SetImage(targetColor);
    }
    public bool FadeOut(Color targetColor, float fadeSpeed)
    {
        var fadeCheck = true;
        fadeCheck = fadeManager.FadeOut(targetColor, 0.5f);
        return fadeCheck;
    }

}
