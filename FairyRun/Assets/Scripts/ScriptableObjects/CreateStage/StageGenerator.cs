using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RunGame.Stage {
    public class StageGenerator : MonoBehaviour
    {
        [SerializeField] private StageData stageData = null;
        // 現在のステージナンバーを取得
        private int nowStageNum;
        [SerializeField]
        private int debugStageNum = 0;
        // ロード先のパスを取得
        private const string stageDataLoadPath = "Datas/StageData/";
        // マップデータの名前を取得
        private const string stageMapDataName = "stageData_";
        // Start is called before the first frame update
        void Start()
        {
            nowStageNum = SceneController.StageNo + 1;
            if(debugStageNum != 0)
            {
                nowStageNum = debugStageNum+1;
            }
            // ステージデータをロード
        if (nowStageNum != 0)
                stageData = Resources.Load(stageDataLoadPath + stageMapDataName + nowStageNum.ToString()) as StageData;
            else
                stageData = Resources.Load(stageDataLoadPath + stageMapDataName) as StageData;
            // ステージ作成
            MapCreate(stageData);
        }
        /// <summary>
        /// マップの生成
        /// </summary>
        private void MapCreate(StageData stageData)
        {
            // タイルの生成
            int width = 0, hight = 0;
            int widthPos = 0, hightPos = 0;
            try
            {
                for (int zIndex = 0; zIndex < stageData.hight; zIndex++)
                {
                    hight++;
                    hightPos++;
                    width = 0;
                    widthPos = 0;
                    for (int xIndex = 0; xIndex < stageData.width; xIndex++)
                    {
                        width++;
                        var index = zIndex * stageData.width + xIndex;
                        // 今回は実験的に数値が１のタイルのみ生成しました。
                        // ここで生成する・しないを管理できます。
                        // 勿論、全てのタイルを生成も可能です。
                        if (stageData.internalData[index] != -1)
                        {
                            var newTile = Instantiate(stageData.TileTips[stageData.internalData[index]], transform);
                            newTile.transform.position = new Vector3(widthPos * 10 - 10, 0, hightPos * 10 - 10);
                            var num = stageData.internalData[index];
                            SetTile(newTile, num);
                        }
                        widthPos++;
                    }
                }
            }
            catch (System.Exception)
            {
                Debug.Log("hight : " + hight + "width : " + width);
                throw;
            }
        }
        // Update is called once per frame
        void Update()
        {

        }
        void SetTile(GameObject target, int num)
        {
            var setRotate = Vector3.zero;
            var scale = target.GetComponent<Transform>().localScale;
            scale = new Vector3(1.0f, 1.0f, 1.0f);
            switch (num)
            {
                case 0:// 草むら 済み
                    target.transform.Rotate(0.0f, 0.0f, 0.0f);
                    break;
                case 1:// 道（縦） 済み
                    target.transform.Rotate(0.0f, 0.0f, 0.0f);
                    break;
                case 2:// 道（横） 済み
                    target.transform.Rotate(0.0f, 90.0f, 0.0f);
                    break;
                case 3:// 行き止まり（上） 済み
                    target.transform.Rotate(0.0f, 180.0f, 0.0f);
                    break;
                case 4:// 曲がり角（下右） 済み
                    target.transform.Rotate(0.0f, 90.0f, 0.0f);
                    break;
                case 5:// 曲がり角（右下） 済み
                    target.transform.Rotate(0.0f, 0.0f, 0.0f);
                    scale = new Vector3(-1.0f, 1.0f, -1.0f);
                    break;
                case 6:// 行き止まり（左） 済み
                    target.transform.Rotate(0.0f, 90.0f, 0.0f);
                    break;
                case 7:// 行き止まり（右） 済み
                    target.transform.Rotate(0.0f, -90.0f, 0.0f);
                    break;
                case 8:// 曲がり角（上右） 済み
                    target.transform.Rotate(0.0f, 0.0f, 0.0f);
                    break;
                case 9:// 曲がり角（左下）済み
                    target.transform.Rotate(0.0f, -90.0f, 0.0f);
                    break;
                case 10:// 行き止まり（下） 済み
                    target.transform.Rotate(0.0f, 0.0f, 0.0f);
                    break;
                default:
                    break;
            }
            target.GetComponent<Transform>().localScale = scale;
        }
    }
}