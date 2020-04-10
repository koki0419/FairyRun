using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    // チュートリアルmessage
    [SerializeField]
    Item_String[] tutorialMessages = null;
    [SerializeField]
    float tutorialWaitTime; // 
    public float GetTutorialWaitTime
    {
        get { return tutorialWaitTime; }
    }
    [SerializeField]
    float tutorialShowTime; // チュートリアル画面表示時間
    public float GetTutorialShowTime
    {
        get { return tutorialShowTime; }
    }
    [SerializeField]
    float tutorialHydeTime; // チュートリアル画面非表示時間
    public float GetTutorialHydeTime
    {
        get { return tutorialHydeTime; }
    }
    /// <summary>
    /// チュートリアルのメッセージを返します
    /// </summary>
    /// <param name="messageNum"></param>
    /// <returns></returns>
    public string GetTutorialMessage(int messageNum)
    {
        var message = "";
        if(tutorialMessages.Length <= messageNum)
        {
            message = tutorialMessages[tutorialMessages.Length - 1].itemEffect;
            return message;
        }else if(messageNum < 0)
        {
            message = tutorialMessages[0].itemEffect;
            return message;
        }
        message = tutorialMessages[messageNum].itemEffect;
        return message;
    }
    // イベントを管理する変数
    // -1はイベントクリアしていない状態
    public int[] tutotialEventManagement;
    private void Start()
    {
        // 初期化
        tutotialEventManagement = new int[tutorialMessages.Length];
        for (int i = 0; i < tutorialMessages.Length; i++)
        {
            tutotialEventManagement[i] = -1;
        }
    }
}

public class Tutorial
{

}
