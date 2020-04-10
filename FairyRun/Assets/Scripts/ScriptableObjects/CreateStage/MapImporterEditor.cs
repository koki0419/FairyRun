using UnityEngine;
using System.IO;
using System;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(MapCSVImporter))]
public class MapImporterEditor : Editor
{
    MapCSVImporter mapCSVImporter;
    string mapData = "";
    // エクスプローラの編集
    public override void OnInspectorGUI()
    {
        // 以下の命令をインスペクタに反映（表示）させます
        DrawDefaultInspector();

        mapCSVImporter = target as MapCSVImporter;
        // ステージ情報を作成するボタンです
        if (GUILayout.Button("ステージデータを作成します"))
        {
            // マップ作成の処理
            var mapCreateCheck = CreateData(mapCSVImporter);
            if (mapCreateCheck != 0)
            {
                switch (mapCreateCheck)
                {
                    case -200:
                        Debug.LogError("<color = red>ファイルがセットされていません</color>");
                        break;
                    case 100:
                        Debug.LogWarning("キャンセルしました");
                        break;
                    default:
                        break;
                }
                return;
            }
            Debug.Log("マップを作成しました");
        }
    }
    bool ReadFile()
    {
        try
        {
            // ファイルを読み込む→読み込み先のパスを取得
            var filePath = EditorUtility.OpenFilePanel("読み込む CSV ファイルを選択してください。", "Assets", "csv");
            // パスの長さが０の時読み込みをキャンセルした
            if (filePath.Length <= 0)
            {
                Debug.Log("キャンセルしました");
                return false;
            }
            Debug.Log("filePath : " + filePath);
            // Todo Resourceから展開じゃなくてパスがあるので外部から取得できるようにする
            mapData = File.ReadAllText(filePath);
            return true;
        }
        catch (Exception e)
        {
            Debug.Log("エラー : " + e.Message);
            return false;
            throw;
        }
    }
    int[,] newinternalData;
    private int CreateData(MapCSVImporter mapCSVImporter)
    {
        // ファイル読み込みが終了するまで先には進まない
        while (true)
        {
            var checker = ReadFile();
            if (checker)
                break;
            else
                return -200;
        }
        // 読み込むCSVファイルがセットされているか確認
        if (mapCSVImporter == null)
        {
            // 使用するタイルチップがセットされていない時
            return -200;
        }
        var data = CreateInstance<StageData>();
        // 保存先アセットパスを取得 ※プル（フル）パスはエラーになる
        var filePath = EditorUtility.SaveFilePanelInProject("ファイルの保存先を選択してください。", "stageData", "asset", "Please enter a file name to save the texture to");
        // パスの長さが０の時読み込みをキャンセルした
        if (string.IsNullOrEmpty(filePath))
        {
            return 100;
        }
        Debug.Log("filePath : " + filePath);
        // ------------**データ作成**---------------
        // 行単位で分割
        var separator = new char[] { '\n' };// 改行位置で切る設定にする
        //var lines = csvText.text.Split(separator);
        var lines = mapData.Split(separator);
        // Z軸（hight）方向の情報を初期化
        data.hight = lines.Length - 1;
        separator = new char[] { ',' };// カンマごと切る設定に変更
        for (int index = 0; index < lines.Length; index++)
        {
            // 列単位で分割する
            var columns = lines[index].Split(separator);
            if (string.IsNullOrEmpty(columns[0]))
            {
                Debug.Log("空");
                continue;
            }
            // マップデータの初期化
            if (data.width < 0)
            {
                data.width = columns.Length;
                data.internalData = new int[data.width * data.hight];
                newinternalData = new int[data.hight, data.width];
                for (int init_Index = 0; init_Index < data.hight; init_Index++)
                {
                    for (int init_Index2 = 0; init_Index2 < data.width; init_Index2++)
                    {

                        newinternalData[init_Index, init_Index2] = -1;
                    }
                }
            }
            // csvファイルから読み込んだデータをマップデータに格納する
            for (int index2 = 0; index2 < columns.Length; index2++)
            {
                // 格納対象の要素数を計算します
                var index3 = index * columns.Length + index2;
                var column = columns[index2];
                // データ格納
                newinternalData[index, index2] = int.Parse(column);
            }
        }
        // データの並び替え
        // 上下のみ反転（左右はなし）
        int changeIndex = data.hight - 1;
        for (int i = 0; i < data.hight / 2; i++)
        {
            for (int j = 0; j < data.width; j++)
            {
                var temp = newinternalData[i, j];// 上
                newinternalData[i, j] = newinternalData[changeIndex, j];
                newinternalData[changeIndex, j] = temp;
            }
            changeIndex--;
        }
        // データの移し替え
        int tempIndex = 0;
        for (int i = 0; i < data.hight; i++)
        {
            for (int j = 0; j < data.width; j++)
            {
                var temp = newinternalData[i, j];
                data.internalData[tempIndex] = temp;
                tempIndex++;
            }
        }
        data.TileTips = mapCSVImporter.mapObjct;
        // インスタンス化したものをassetとして保存
        var asset = (StageData)AssetDatabase.LoadAssetAtPath(filePath, typeof(StageData));
        if (asset == null)
        {
            //指定のパスにファイルが存在しない場合は新規作成
            AssetDatabase.CreateAsset(data, filePath);
        }
        else
        {
            //指定のパスに既に同名のファイルが存在する場合は更新
            EditorUtility.CopySerialized(data, asset);
            AssetDatabase.SaveAssets();
        }
        AssetDatabase.Refresh();
        Debug.Log(mapCSVImporter.name + " : マップデータの作成が完了しました。");
        return 0;
    }
}
#endif

