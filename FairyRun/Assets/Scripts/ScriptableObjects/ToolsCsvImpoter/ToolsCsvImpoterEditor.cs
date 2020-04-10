using UnityEngine;
using System.IO;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(ToolsCsvImpoter))]

public class ToolsCsvImpoterEditor : Editor
{
    ToolsCsvImpoter csvImporter;
    string mapData = "";

    private const string saveAssetPath = "";
    private const string csvFileName_Item = "ITEM";
    private const string csvFileName_Enemy = "ENEMY";
    private const string csvFileName_Obstacle = "OBSTACLE";
    int selectFileIndex = 0;
    int errorCheckIndex = -1;
    // エラーチェック用のクラスを作る
    enum ErrorCode
    {
        none,
        FileImportSuccess = 1,// ファイルインポート成功
        CreatDataSuccess = 2,// データ作成成功
        FileImportError = -100,
        CreatDataError = -200,
        DifferentFileError = -300,
        PathError = -400,
    }

    public override void OnInspectorGUI()
    {
        csvImporter = target as ToolsCsvImpoter;
        DrawDefaultInspector();
        // どのCSVファイルを使用するか選択（ラジオボタン）
        selectFileIndex = GUILayout.SelectionGrid(selectFileIndex, new string[] { "ItemCsv", "EnemyCsv", "ObstacleCsv" }, 1, EditorStyles.radioButton);
        // データ作成
        if (GUILayout.Button("データを作成します"))
        {
            var fileName = csvImporter.createName;
            // 保存先アセットパスを取得
            var filePath = EditorUtility.SaveFilePanelInProject("TITLE", fileName, "asset", "Please enter a file name to save the texture to");
            // パスの長さが０の時読み込みをキャンセルした
            if (string.IsNullOrEmpty(filePath))
            {
                errorCheckIndex = (int)ErrorCode.CreatDataError;
                ErrorChecker(errorCheckIndex);
                return;
            }
            switch (selectFileIndex)
            {
                case 0:
                    errorCheckIndex = CreatItem(filePath);
                    break;
                case 1:
                    errorCheckIndex = CreatEnemy(filePath);
                    break;
                case 2:
                    errorCheckIndex = CreatObstacle(filePath);
                    break;
                default:
                    break;
            }
            ErrorChecker(errorCheckIndex);
        }
    }
    private bool ReadFile()
    {
        var filePath = EditorUtility.OpenFilePanel("Title", "Assets", "csv");
        if (filePath.Length <= 0)
            return false;
        // Todo Resourceから展開じゃなくてパスがあるので外部から取得できるようにする
        mapData = File.ReadAllText(filePath);
        return true;
    }
    private int CreatItem(string filePath)
    {
        while (true)
        {
            var checker = ReadFile();
            if (checker)
                break;
            else
                return -200;
        }
        var itemData = CreateInstance<Items>();
        itemData.itemList = new List<ItemEntry>();
        // -------** データ作成 **------
        // 行単位で分割
        var separator = new char[] { '\n' };
        var lines = mapData.Split(separator);
        // 列単位で分割
        separator = new char[] { ',' };
        if (lines[0].Split(separator)[1] != csvFileName_Item)
        {
            return (int)ErrorCode.DifferentFileError;
        }
        try
        {
            // index = 1 は、先頭のヘッダー行を取り除くため
            for (int lineIndex = 2; lineIndex < lines.Length; lineIndex++)
            {
                ItemEntry item = new ItemEntry();
                var column = lines[lineIndex].Split(separator);
                if (string.IsNullOrEmpty(column[0]))
                {
                    continue;
                }
                int columnIndex = 0;
                item.id = int.Parse(column[columnIndex++]);
                item.itemName = column[columnIndex++];
                item.point = int.Parse(column[columnIndex++]);
                item.position.x = float.Parse(column[columnIndex++]);
                item.position.y = float.Parse(column[columnIndex++]);
                item.position.z = float.Parse(column[columnIndex]);
                item.rotate = float.Parse(column[columnIndex]);
                itemData.itemList.Add(item);
            }
            CreatDataAsset(filePath, itemData);
            return (int)ErrorCode.CreatDataSuccess;
        }
        catch (System.Exception e)
        {
            Debug.LogError("エラー : " + e.Message);
            return (int)ErrorCode.DifferentFileError;
            throw;
        }
    }
    private int CreatEnemy(string filePath)
    {
        while (true)
        {
            var checker = ReadFile();
            if (checker)
                break;
            else
                return -200;
        }
        var enemyData = CreateInstance<Enemys>();
        enemyData.enemyList = new List<EnemyEntry>();

        // -------** データ作成 **------
        // 行単位で分割
        var separator = new char[] { '\n' };
        var lines = mapData.Split(separator);
        // 列単位で分割
        separator = new char[] { ',' };
        if (lines[0].Split(separator)[1] != csvFileName_Enemy)
        {
            Debug.Log(lines[0].Split(separator)[1] + "  :  " + csvFileName_Enemy);
            return (int)ErrorCode.DifferentFileError;
        }
        // checker を使用して正常終了したかどうか判定する
        try
        {
            // index = 1 は、先頭のヘッダー行を取り除くため
            for (int lineIndex = 2; lineIndex < lines.Length; lineIndex++)
            {
                EnemyEntry enemy = new EnemyEntry();
                var column = lines[lineIndex].Split(separator);
                if (string.IsNullOrEmpty(column[0]))
                {
                    continue;
                }
                int columnIndex = 0;
                enemy.id = int.Parse(column[columnIndex++]);
                enemy.enemyName = column[columnIndex++];
                enemy.position.x = float.Parse(column[columnIndex++]);
                enemy.position.y = float.Parse(column[columnIndex++]);
                enemy.position.z = float.Parse(column[columnIndex++]);
                enemy.rotate = float.Parse(column[columnIndex]);
                enemyData.enemyList.Add(enemy);
            }
            CreatDataAsset(filePath, enemyData);
            return (int)ErrorCode.CreatDataSuccess;
        }
        catch (System.Exception e)
        {
            Debug.LogError("エラー : " + e.Message);
            return (int)ErrorCode.DifferentFileError;
            throw;
        }
    }
    private int CreatObstacle(string filePath)
    {
        while (true)
        {
            var checker = ReadFile();
            if (checker)
                break;
            else
                return -200;
        }
        var obstacleData = CreateInstance<Obstacles>();
        obstacleData.ObstacleList = new List<ObstacleEntry>();
        // -------** データ作成 **------
        // 行単位で分割
        var separator = new char[] { '\n' };
        var lines = mapData.Split(separator);
        // 列単位で分割
        separator = new char[] { ',' };
        if (lines[0].Split(separator)[1] != csvFileName_Obstacle)
        {
            return (int)ErrorCode.DifferentFileError;
        }
        try
        {
            // index = 1 は、先頭のヘッダー行を取り除くため
            for (int lineIndex = 2; lineIndex < lines.Length; lineIndex++)
            {
                ObstacleEntry obstacle = new ObstacleEntry();
                var column = lines[lineIndex].Split(separator);
                if (string.IsNullOrEmpty(column[0]))
                {
                    continue;
                }
                int columnIndex = 0;
                obstacle.id = int.Parse(column[columnIndex++]);
                obstacle.obstacleName = column[columnIndex++];
                obstacle.position.x = float.Parse(column[columnIndex++]);
                obstacle.position.y = float.Parse(column[columnIndex++]);
                obstacle.position.z = float.Parse(column[columnIndex++]);
                obstacle.rotate = float.Parse(column[columnIndex]);
                obstacleData.ObstacleList.Add(obstacle);
            }
            CreatDataAsset(filePath, obstacleData);
            return (int)ErrorCode.CreatDataSuccess;
        }
        catch (System.Exception e)
        {
            Debug.LogError("エラー : " + e.Message);
            return (int)ErrorCode.DifferentFileError;
            throw;
        }
    }
    private void CreatDataAsset(string filePath, Items itemData)
    {
        // インスタンス化したものをassetとして保存
        var asset = (Items)AssetDatabase.LoadAssetAtPath(filePath, typeof(Items));
        if (asset == null)
        {
            //指定のパスにファイルが存在しない場合は新規作成
            AssetDatabase.CreateAsset(itemData, filePath);
        }
        else
        {
            //指定のパスに既に同名のファイルが存在する場合は更新
            EditorUtility.CopySerialized(itemData, asset);
            AssetDatabase.SaveAssets();
        }
        AssetDatabase.Refresh();
    }
    private void CreatDataAsset(string filePath, Enemys enemyData)
    {
        // インスタンス化したものをassetとして保存
        var asset = (Enemys)AssetDatabase.LoadAssetAtPath(filePath, typeof(Enemys));
        if (asset == null)
        {
            //指定のパスにファイルが存在しない場合は新規作成
            AssetDatabase.CreateAsset(enemyData, filePath);
        }
        else
        {
            //指定のパスに既に同名のファイルが存在する場合は更新
            EditorUtility.CopySerialized(enemyData, asset);
            AssetDatabase.SaveAssets();
        }
        AssetDatabase.Refresh();
    }
    private void CreatDataAsset(string filePath, Obstacles obstacleData)
    {
        // インスタンス化したものをassetとして保存
        var asset = (Obstacles)AssetDatabase.LoadAssetAtPath(filePath, typeof(Obstacles));
        if (asset == null)
        {
            //指定のパスにファイルが存在しない場合は新規作成
            AssetDatabase.CreateAsset(obstacleData, filePath);
        }
        else
        {
            //指定のパスに既に同名のファイルが存在する場合は更新
            EditorUtility.CopySerialized(obstacleData, asset);
            AssetDatabase.SaveAssets();
        }
        AssetDatabase.Refresh();
    }
    private void ErrorChecker(int errorIndex)
    {
        switch (errorIndex)
        {
            case (int)ErrorCode.none:
                break;
            case (int)ErrorCode.FileImportSuccess:
                Debug.Log("ファイルのインポートに成功しました");
                break;
            case (int)ErrorCode.CreatDataSuccess:
                Debug.Log("データ作成に成功しました");
                break;
            case (int)ErrorCode.FileImportError:
                Debug.LogWarning("ファイルのインポートをキャンセルしました");
                break;
            case (int)ErrorCode.CreatDataError:
                Debug.LogWarning("データ作成をキャンセルしました");
                break;
            case (int)ErrorCode.DifferentFileError:
                Debug.LogError("異なるファイルがインポートされている可能性があります");
                Debug.LogError("もしくは作成データの種類が異なっている可能性があります");
                break;
            case (int)ErrorCode.PathError:
                Debug.LogWarning("パスがありません");
                break;
            default:
                break;
        }
    }
}
#endif