using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(CreateEnemy_Obstacle_PosData))]
public class CreateEnemy_Obstacle_PosDataEditor : Editor
{
    CreateEnemy_Obstacle_PosData createPos;
    int selectFileIndex = 0;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        // どのCSVファイルを使用するか選択（ラジオボタン）
        selectFileIndex = GUILayout.SelectionGrid(selectFileIndex, new string[] {"EnemyPos", "ObstaclePosv" }, 1, EditorStyles.radioButton);
        createPos = target as CreateEnemy_Obstacle_PosData;
        if (GUILayout.Button("ポジション生成"))
        {
            // 保存先アセットパスを取得
            var filePath = EditorUtility.SaveFilePanelInProject("TITLE", "ItemPos", "asset", "Please enter a file name to save the texture to");
            var targetParentObj = createPos.targetObj as GameObject;
            switch (selectFileIndex)
            {
                case 0:
                    var enemys = CreateInstance<Enemys>();
                    enemys.enemyList = new List<EnemyEntry>();
                    // 親オブジェクトから子オブジェクトのポジション要素を取得
                    // リスト化させている
                    for (int index = 0; index < targetParentObj.transform.childCount; index++)
                    {
                        EnemyEntry enemy = new EnemyEntry();
                        var ChildObject = targetParentObj.transform.GetChild(index).gameObject;
                        enemy.position = ChildObject.transform.position;
                        var rotate = ChildObject.transform.rotation;
                        enemy.rotate = rotate.eulerAngles.y;
                        enemys.enemyList.Add(enemy);
                        CreatAssets(enemys, filePath);
                    }
                    break;
                case 1:
                    var obstacles = CreateInstance<Obstacles>();
                    obstacles.ObstacleList = new List<ObstacleEntry>();
                    // 親オブジェクトから子オブジェクトのポジション要素を取得
                    // リスト化させている
                    for (int index = 0; index < targetParentObj.transform.childCount; index++)
                    {
                        ObstacleEntry obstacle = new ObstacleEntry();
                        var ChildObject = targetParentObj.transform.GetChild(index).gameObject;
                        obstacle.position = ChildObject.transform.position;
                        var rotate = ChildObject.transform.rotation;
                        obstacle.rotate = rotate.eulerAngles.y;
                        obstacles.ObstacleList.Add(obstacle);
                    }
                    CreatAssets(obstacles, filePath);
                    break;
                default:
                    break;
            }
            
           
        }
    }

    private void CreatAssets(Obstacles obstacles, string filePath)
    {
        // インスタンス化したものをassetとして保存
        var asset = (Obstacles)AssetDatabase.LoadAssetAtPath(filePath, typeof(Obstacles));
        if (asset == null)
        {
            //指定のパスにファイルが存在しない場合は新規作成
            AssetDatabase.CreateAsset(obstacles, filePath);
        }
        else
        {
            //指定のパスに既に同名のファイルが存在する場合は更新
            EditorUtility.CopySerialized(obstacles, asset);
            AssetDatabase.SaveAssets();
        }
        AssetDatabase.Refresh();
    }
    private void CreatAssets(Enemys enemys, string filePath)
    {
        // インスタンス化したものをassetとして保存
        var asset = (Enemys)AssetDatabase.LoadAssetAtPath(filePath, typeof(Enemys));
        if (asset == null)
        {
            //指定のパスにファイルが存在しない場合は新規作成
            AssetDatabase.CreateAsset(enemys, filePath);
        }
        else
        {
            //指定のパスに既に同名のファイルが存在する場合は更新
            EditorUtility.CopySerialized(enemys, asset);
            AssetDatabase.SaveAssets();
        }
        AssetDatabase.Refresh();
    }
}
#endif