using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RunGame.Stage
{
    /// <summary>
    /// UIによる情報表示を管理します。
    /// </summary>
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
            tutorialManager = GetComponent<TutorialManager>();
        }
        #endregion

        #region 「Message」UI
        /// <summary>
        /// 「Message」UIを指定します。
        /// </summary>
        [SerializeField]
        private GameObject message = null;

        /// <summary>
        /// 「Message」UIにメッセージを表示します。
        /// </summary>
        /// <param name="text">表示させたいテキスト</param>
        public void ShowMessage(string text)
        {
            message.GetComponentInChildren<Text>().text = text;
            message.SetActive(true);
        }

        /// <summary>
        /// 「Message」UIを隠します。
        /// </summary>
        public void HideMessage()
        {
            message.SetActive(false);
        }
        #endregion
        #region 「Score」UIとスコアUIのUpdate
        [SerializeField]
        private GameObject scoreUI = null;
        private Text scoreUIText = null;
        // ScoreUIを表示します。
        public void ShowScoreUI()
        {
            scoreUI.SetActive(true);
        }
        // ScoreUIを非表示します。
        public void HydeScoreUI()
        {
            scoreUI.SetActive(false);
        }
        // ScoreUIのUpdate
        public void UpdateScoreUI(int score)
        {
            scoreUIText.text = score.ToString();
        }
        #endregion
        #region 「リザルト」UI
        // リザルトUI
        [SerializeField]
        private GameObject resultUI = null;
        public void ShowResultUI()
        {
            resultUI.SetActive(true);
        }
        public void HydeResultUI()
        {
            resultUI.SetActive(false);
        }
        #endregion
        // リザルトの各データのTextを取得
        [SerializeField]
        private Text enemyDefeatedText = null;
        [SerializeField]
        private Text getScorePointText = null;
        [SerializeField]
        private Text clearTimeText = null;
        [SerializeField]
        private Text allScoreTimeText = null;
        #region 「tutorial」UI関係
        // チュートリアルUI
        [SerializeField]
        private GameObject tutorialUI = null;
        public TutorialManager tutorialManager = null;
        public Text tutorialText = null;
        public void ShowTutorialUI()
        {
            tutorialUI.SetActive(true);
        }
        public void HydeTutorialtUI()
        {
            tutorialUI.SetActive(false);
        }
        public void SetTutorialText(int tutorialNum)
        {
            tutorialText.text = tutorialManager.GetTutorialMessage(tutorialNum);
        }
        #endregion
        #region 「TIME: 00.00」表示用のUI
        /// <summary>
        /// 「TIME: 00.00」表示用のUIを指定します。
        /// 
        /// </summary>
        [SerializeField]
        private Text timeUI = null;
        [SerializeField]
        private GameObject timeUIObj = null;
        public void ShowTimeUI()
        {
            timeUIObj.SetActive(true);
        }
        public void HydeTimeUI()
        {
            timeUIObj.SetActive(false);
        }
        /// <summary>
        /// 「TIME: 00.0」UIの表示を更新します。
        /// </summary>
        private void UpdateTimeUI()
        {
            timeUI.text = SceneController.Instance.PlayTime.ToString("00.0");
        }
        #endregion

        [SerializeField]
        private GameObject tutorialDialogUI = null;
        public void ShowTutorialDialogUI()
        {
            tutorialDialogUI.SetActive(true);
        }
        public void HydeTutorialDialogUI()
        {
            tutorialDialogUI.SetActive(false);
        }
        // Pause画面
        [SerializeField]
        private GameObject pauseDialog = null;
        public void ShowPauseDialogUI()
        {
            pauseDialog.SetActive(true);
        }
        public void HydePauseDialogUI()
        {
            pauseDialog.SetActive(false);
        }

        [SerializeField]
        private Text endingMessageText = null;
        public void SetEndingMessage(string text)
        {
            endingMessageText.text = text;
        }
        public void ShowEndingMessageText()
        {
            // Imageを最前表示にする
            endingMessageText.gameObject.transform.SetAsLastSibling();
            endingMessageText.gameObject.SetActive(true);
        }
        public void HydeEndingMessageText()
        {
            endingMessageText.gameObject.SetActive(false);
        }

        public void SetResultUIState(int stageNum, float clearTime, int enemyDefeatedCount, int getScorePoint, int allScoreTime)
        {
            enemyDefeatedText.text = enemyDefeatedCount.ToString() + " 匹";
            getScorePointText.text = getScorePoint.ToString() + " pt";
            clearTimeText.text = clearTime.ToString("0.00") + " 秒";
            allScoreTimeText.text = allScoreTime.ToString() + " pt";
        }
        #region「妖精の攻撃ポイント」UI
        [SerializeField]
        private Text fairyAttackPointText = null;
        /// <summary>
        /// 妖精の攻撃ポイントをアップデート（表示）させます
        /// </summary>
        public void UpdateFairyAttackPointUi(int fairyAttackPoint)
        {
            fairyAttackPointText.text = SceneController.fairyPoint.ToString("00");
        }
        #endregion
        // Start is called before the first frame update
        void Start()
        {
            tutorialText = tutorialUI.GetComponentInChildren<Text>();
            scoreUIText = scoreUI.GetComponentInChildren<Text>();
            HideMessage();
            UpdateTimeUI();
            UpdateFairyAttackPointUi(SceneController.fairyPoint);
        }

        // Update is called once per frame
        void Update()
        {
            UpdateTimeUI();
            UpdateFairyAttackPointUi(SceneController.fairyPoint);
            UpdateScoreUI(SceneController.scoerPoint);
        }
    }
}
