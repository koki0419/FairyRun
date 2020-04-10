using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RunGame.SelectStage
{
    public class UiManager : MonoBehaviour
    {
        #region インスタンスへのstaticなアクセスポイント
        /// <summary>
        /// このクラスのインスタンスを取得します。
        /// </summary>
        public static UiManager Instance
        {
            get { return instance; }
        }
        static UiManager instance = null;

        /// <summary>
        /// Start()より先に実行されます。
        /// </summary>
        private void Awake()
        {
            instance = this;
        }
        #endregion
        // 南京錠の画像
        [SerializeField]
        private GameObject[] nankinjoImage = null;
        // 南京錠の画像の表示
        public void ShowNankinjoImage(int showNum)
        {
            nankinjoImage[showNum].SetActive(true);
        }
        // 南京錠画像の非表示
        public void HydeNankinjoImage(int showNum)
        {
            nankinjoImage[showNum].SetActive(false);
        }

        // Score表示テキスト
        [SerializeField]
        private Text scoreUIText = null;
        // ScoreテキストのUpdate
        public void scoreUITextUpdate(int score)
        {
            scoreUIText.text = string.Format("{0}", score);
        }
        // 各ステージ開放条件（Scoreポイント）
        [SerializeField]
        private GameObject[] stageReleaseScoreImage = null;

        // 解放スコアの画像の表示
        public void ShowReleaseScoreImage(int showNum)
        {
            stageReleaseScoreImage[showNum].SetActive(true);
        }
        // 解放スコア画像の非表示
        public void HydeReleaseScoreImage(int showNum)
        {
            stageReleaseScoreImage[showNum].SetActive(false);
        }

        public void SetStageReleaseScoreImage(int[] releaseScorePoint)
        {
            for (int i = 0; i < stageReleaseScoreImage.Length; i++)
            {
                var text = stageReleaseScoreImage[i].GetComponentInChildren<Text>().text;
                text = string.Format("{0}", releaseScorePoint[i]);
                stageReleaseScoreImage[i].GetComponentInChildren<Text>().text = text;
            }
        }

        [SerializeField]
        private GameObject quitDialog = null;
        public void ShowQuitDialogUI()
        {
            quitDialog.SetActive(true);
        }
        public void HydeQuitDialogUI()
        {
            quitDialog.SetActive(false);
        }
    }
}