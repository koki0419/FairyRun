using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(CreateMapSubstance))]
public class CreateMapSubstanceEditor : Editor
{
    CreateMapSubstance mapSubstance;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        mapSubstance = target as CreateMapSubstance;
        if (GUILayout.Button("ステージを作成します"))
        {
            // マップチップ取得
            StageData stageData = mapSubstance.stageData;
            // 作成するステージナンバー
            int stageNum = mapSubstance.stageNum;
            MapCreate(stageData, stageNum-1);
        }
    }

    /// <summary>
    /// マップの生成
    /// </summary>
    private void MapCreate(StageData stageData, int stageNum)
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
                widthPos = 0;
                for (int xIndex = 0; xIndex < stageData.width; xIndex++)
                {
                    var index = zIndex * stageData.width + xIndex;
                    // 今回は実験的に数値が１のタイルのみ生成しました。
                    // ここで生成する・しないを管理できます。
                    // 勿論、全てのタイルを生成も可能です。
                    if (stageData.internalData[index] != -1)
                    {
                        var newTile = Instantiate(stageData.TileTips[stageData.internalData[index]]);
                        if (stageNum == 0)
                        {
                            newTile.transform.position = new Vector3(widthPos * 10 - 20, 0, hightPos * 10 - 110);
                        }
                        else
                        {
                            newTile.transform.position = new Vector3(widthPos * 10 - 20, 0, hightPos * 10 - 20);
                        }
                        var num = stageData.internalData[index];
                        SetTile(newTile, num);
                        Undo.RegisterCreatedObjectUndo(newTile, string.Format("Ground{0}", index));
                    }
                    widthPos++;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("エラーメッセージ : " + e.Message);
            Debug.Log("hight : " + hight + "width : " + width);
            throw;
        }
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

#endif