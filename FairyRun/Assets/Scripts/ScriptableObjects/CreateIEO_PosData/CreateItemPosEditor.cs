using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(CreateItemPos))]
public class CreateItemPosEditor : Editor
{
    CreateItemPos createItemPos;
    public override void OnInspectorGUI()
    {
        createItemPos = target as CreateItemPos;
        DrawDefaultInspector();
        if (GUILayout.Button("アイテムポジション生成"))
        {
            // 保存先アセットパスを取得
            var filePath = EditorUtility.SaveFilePanelInProject("TITLE", "ItemPos", "asset", "Please enter a file name to save the texture to");
            var targetParentObj = createItemPos.targetObj as GameObject;
            var items = CreateInstance<Items>();
            items.itemList = new List<ItemEntry>();
            // 親オブジェクトから子オブジェクトのポジション要素を取得
            // リスト化させている
            for(int index = 0;index < targetParentObj.transform.childCount; index++)
            {
                ItemEntry item = new ItemEntry();
                var ChildObject = targetParentObj.transform.GetChild(index).gameObject;
                item.point = createItemPos.targetEfect.itemEffect;
                item.position = ChildObject.transform.position;
                var rotate = ChildObject.transform.rotation;
                item.rotate = rotate.eulerAngles.y;
                items.itemList.Add(item);
            }
            // インスタンス化したものをassetとして保存
            var asset = (FairyItem)AssetDatabase.LoadAssetAtPath(filePath, typeof(FairyItem));
            if (asset == null)
            {
                //指定のパスにファイルが存在しない場合は新規作成
                AssetDatabase.CreateAsset(items, filePath);
            }
            else
            {
                //指定のパスに既に同名のファイルが存在する場合は更新
                EditorUtility.CopySerialized(items, asset);
                AssetDatabase.SaveAssets();
            }
            AssetDatabase.Refresh();
        }
    }
}
#endif