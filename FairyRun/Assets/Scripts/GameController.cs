using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunGame
{
    /// <summary>
    /// ゲーム全体で使用されるリソースを管理します。
    /// </summary>
    public class GameController : MonoBehaviour
    {
        #region シングルトンインスタンス

        [RuntimeInitializeOnLoadMethod]
        private static void CreateInstance()
        {
            // Resourcesからプレハブを読み込む
            var prefab = Resources.Load<GameObject>("GameController");
            Instantiate(prefab);
        }

        /// <summary>
        /// このクラスのインスタンスを取得します。
        /// </summary>
        public static GameController Instance
        {
            get
            {
                return instance;
            }
        }
        private static GameController instance = null;

        /// <summary>
        /// Start()の実行より先行して処理したい内容を記述します。
        /// </summary>
        void Awake()
        {
            // 初回作成時
            if (instance == null)
            {
                instance = this;
                // シーンをまたいで削除されないように設定
                DontDestroyOnLoad(gameObject);
                // セーブデータを読み込む
                Load();
            }
            // 2個目以降の作成時
            else
            {
                Destroy(gameObject);
            }
        }

        #endregion

        /// <summary>
        /// ベストタイムを取得または設定します。
        /// </summary>
        public float BestTime
        {
            get { return bestTime; }
            set
            {
                bestTime = value;
                Save();
            }
        }
        /// <summary>
        /// ベストタイムの基準値を指定します。
        /// </summary>
        [SerializeField]
        private float bestTime = 60;

        /// <summary>
        /// ステージ名を取得します。
        /// </summary>
        public string[] StageNames
        {
            get { return stageNames; }
        }
        /// <summary>
        /// ステージ名を配列で指定します。
        /// </summary>
        private string[] stageNames = {
            "はじまりのみち",
            "ちゅうかんちてん",
            "さいごのみち",
        };
        /// <summary>
        /// 起動時からの総合獲得ポイント
        /// </summary>
        [SerializeField]
        public static int totalScorePoint = 0;
        public int[] StageReleasePoint
        {
            get { return stageReleasePoint; }
        }
        public static int checkScorePoint = 0;
        /// <summary>
        /// ステージ２，３の解放条件（ポイント）
        /// </summary>
        [SerializeField]
        private int[] stageReleasePoint =
        {
            0,
            2000,// ステージ2解放条件
            5000,// ステージ3解放条件
        };
        // 各ステージのゴール位置
        private Vector3[] goalPos =
        {
            new Vector3( 422.0f,2.25f,270.0f), // ステージ1
            new Vector3( 500.0f,2.25f,940.0f), // ステージ2
            new Vector3( 460.0f,2.0f,985.0f), // ステージ3
        };
        public Vector3[] GoalPos
        {
            get { return goalPos; }
        }

        // 各ステージのゴールの向き
        private float[] goalRotate_Y =
        {
            -90, // ステージ1
            180, // ステージ2
            180, // ステージ3
        };
        public float[] GoalRotate_Y
        {
            get { return goalRotate_Y; }
        }
        /// <summary>
        /// GameControllerが破棄される場合に呼び出されます。
        /// </summary>
        private void OnDestroy()
        {
            Save();
        }

        // セーブデータ用の識別子
        static readonly string bestTimeKey = "BestTimeKey";

        /// <summary>
        /// ゲームデータを読込みます。
        /// </summary>
        private void Load()
        {
            BestTime = PlayerPrefs.GetFloat(bestTimeKey, BestTime);
        }

        /// <summary>
        /// ゲームデータを保存します。
        /// </summary>
        public void Save()
        {
            PlayerPrefs.SetFloat(bestTimeKey, BestTime);
            PlayerPrefs.Save();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
