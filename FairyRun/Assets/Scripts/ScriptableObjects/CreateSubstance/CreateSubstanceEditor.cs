using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(CreateSubstance))]
public class CreateSubstanceEditor : Editor
{
    CreateSubstance createItemSubstance;
    int selectSubstanceIndex = 0;
    public override void OnInspectorGUI()
    {
        createItemSubstance = target as CreateSubstance;
        DrawDefaultInspector();
        if (GUILayout.Button("作成します"))
        {
            var createType = createItemSubstance.type;
            switch (createType)
            {
                case CreateType.ITEM:
                    CreateItemSubstance(createItemSubstance);
                    break;
                case CreateType.ENEMY:
                    CreateEnemySubstance(createItemSubstance);
                    break;
                case CreateType.OBSTACLE:
                    CreateObstacleSubstance(createItemSubstance);
                    break;
            }
        }
    }
    /// <summary>
    /// 実体の生成
    /// </summary>
    private void CreateItemSubstance(CreateSubstance substanceData)
    {
        try
        {
            // アイテムのポジションデータを取得
            var datas = substanceData.itemData.itemList;
            // 生成するオブジェクトを取得
            var tips = substanceData.substanceItemTip;
            // アイテムデータ数を取得
            var dataLength = datas.Count;
            for (int index = 0; index < dataLength; index++)
            {
                var newObj = Instantiate(tips[0]);
                var pos = newObj.transform.position;
                pos.x = datas[index].position.x + substanceData.errorRange_Item.x;
                pos.y = datas[index].position.y;
                pos.z = datas[index].position.z + substanceData.errorRange_Item.z;
                newObj.transform.position = pos;
                newObj.transform.Rotate(0, 90, 0);
                Undo.RegisterCreatedObjectUndo(newObj, "item" + index);
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("エラーメッセージ : " + e.Message);
            throw;
        }
    }

    private void CreateEnemySubstance(CreateSubstance substanceData)
    {
        try
        {
            // アイテムのポジションデータを取得
            var datas = substanceData.enemyData.enemyList;
            // 生成するオブジェクトを取得
            var tips = substanceData.substanceEnemyTip;
            // アイテムデータ数を取得
            var dataLength = datas.Count;
            for (int index = 0; index < dataLength; index++)
            {
                var newObj = Instantiate(tips[0]);
                var pos = newObj.transform.position;
                pos.x = datas[index].position.x + substanceData.errorRange_Enemy.x;
                pos.y = datas[index].position.y;
                pos.z = datas[index].position.z + substanceData.errorRange_Enemy.z;
                newObj.transform.position = pos;
                newObj.transform.Rotate(0, datas[index].rotate, 0);
                Undo.RegisterCreatedObjectUndo(newObj, "enemy" + index);
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("エラーメッセージ : " + e.Message);
            throw;
        }
    }
    private void CreateObstacleSubstance(CreateSubstance substanceData)
    {
        try
        {
            // アイテムのポジションデータを取得
            var datas = substanceData.obstacleData.ObstacleList;
            // 生成するオブジェクトを取得
            var tips = substanceData.substanceObstacleTip;
            // アイテムデータ数を取得
            var dataLength = datas.Count;
            for (int index = 0; index < dataLength; index++)
            {
                var newObj = Instantiate(tips[0]);
                var pos = newObj.transform.position;
                pos.x = datas[index].position.x + substanceData.errorRange_Obstacle.x;
                pos.y = datas[index].position.y;
                pos.z = datas[index].position.z + substanceData.errorRange_Obstacle.z;
                newObj.transform.position = pos;
                newObj.transform.Rotate(0, datas[index].rotate, 0);
                Undo.RegisterCreatedObjectUndo(newObj, "obstacle" + index);
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("エラーメッセージ : " + e.Message);
            throw;
        }
    }
}
#endif