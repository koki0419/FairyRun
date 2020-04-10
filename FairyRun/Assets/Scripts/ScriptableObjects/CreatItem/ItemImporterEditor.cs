using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(ItemImporter))]
public class ItemImporterEditor : Editor
{
    ItemImporter itemImporter;
    private const string saveAssetPath = "/Resources";
    public override void OnInspectorGUI()
    {
        // 以下の命令をインスペクタに反映（表示）させます
        DrawDefaultInspector();
        itemImporter = target as ItemImporter;
        //var t = (ValueType)EditorGUILayout.EnumPopup(itemImporter.type, GUILayout.MaxWidth(100));
        // アイテム作成ボタン
        if (GUILayout.Button("アイテム作成"))
        {
            CreateItem(itemImporter);
            //Debug.Log("アイテム名 = : " + itemImporter.itemName);
            //Debug.Log("タイプ 　　= : " + itemImporter.type);
            //Debug.Log("コメント　 = : " + itemImporter.itemEffectComment);
        }
    }

    bool CreateItem(ItemImporter itemImporter)
    {
        // 読み込むCSVファイルがセットされているか確認
        if (itemImporter == null)
        {
            // ファイルがセットされていなかったとき
            Debug.Log("ファイルなし");
            return false;
        }
        // アイテム名・// アイテムのコメント
        switch (itemImporter.type)
        {
            case ValueType.BOOL:
                //item_Bool = CreateInstance<Item<bool>>();
                var item_Bool = CreateInstance<Item_Bool>();
                if (item_Bool == null)
                {
                    Debug.Log("ああああああああｚ");
                    return false;
                }
                //Debug.Log("アイテム名 = : " + itemImporter.itemName);
                item_Bool.itemName = itemImporter.itemName;
                item_Bool.message = itemImporter.message;
                // 保存先アセットパスを取得 ※プル（フル）パスはエラーになる
                var filePathBool = EditorUtility.SaveFilePanelInProject("TITLE", item_Bool.itemName, "asset", "Please enter a file name to save the texture to");
                // パスの長さが０の時読み込みをキャンセルした
                if (string.IsNullOrEmpty(filePathBool))
                {
                    return false;
                }
                var asset_Bool = (Item_Bool)AssetDatabase.LoadAssetAtPath(filePathBool, typeof(Item_Bool));
                if (asset_Bool == null)
                {
                    //指定のパスにファイルが存在しない場合は新規作成
                    AssetDatabase.CreateAsset(item_Bool, filePathBool);
                }
                else
                {
                    //指定のパスに既に同名のファイルが存在する場合は更新
                    EditorUtility.CopySerialized(item_Bool, asset_Bool);
                    AssetDatabase.SaveAssets();
                }
                AssetDatabase.Refresh();
                break;
            case ValueType.STRING:
                //var item_String = CreateInstance<Item<string>>();
                var item_String = CreateInstance<Item_String>();
                item_String.itemName = itemImporter.itemName;
                item_String.message = itemImporter.message;
                var filePathString = EditorUtility.SaveFilePanelInProject("TITLE", item_String.itemName, "asset", "Please enter a file name to save the texture to");
                // パスの長さが０の時読み込みをキャンセルした
                if (string.IsNullOrEmpty(filePathString))
                {
                    return false;
                }
                var asset_String = (Item_String)AssetDatabase.LoadAssetAtPath(filePathString, typeof(Item_String));
                if (asset_String == null)
                {
                    //指定のパスにファイルが存在しない場合は新規作成
                    AssetDatabase.CreateAsset(item_String, filePathString);
                }
                else
                {
                    //指定のパスに既に同名のファイルが存在する場合は更新
                    EditorUtility.CopySerialized(item_String, asset_String);
                    AssetDatabase.SaveAssets();
                }
                AssetDatabase.Refresh();
                break;
            case ValueType.INT:
                //var item_Int = CreateInstance<Item<int>>();
                var item_Int = CreateInstance<Item_Int>();
                item_Int.itemName = itemImporter.itemName;
                item_Int.message = itemImporter.message;
                var filePathInt = EditorUtility.SaveFilePanelInProject("TITLE", item_Int.itemName, "asset", "Please enter a file name to save the texture to");
                // パスの長さが０の時読み込みをキャンセルした
                if (string.IsNullOrEmpty(filePathInt))
                {
                    return false;
                }
                var asset_Int = (Item_Int)AssetDatabase.LoadAssetAtPath(filePathInt, typeof(Item_Int));
                if (asset_Int == null)
                {
                    //指定のパスにファイルが存在しない場合は新規作成
                    AssetDatabase.CreateAsset(item_Int, filePathInt);
                }
                else
                {
                    //指定のパスに既に同名のファイルが存在する場合は更新
                    EditorUtility.CopySerialized(item_Int, asset_Int);
                    AssetDatabase.SaveAssets();
                }
                AssetDatabase.Refresh();
                break;
            case ValueType.FLOAT:
                var item_Float = CreateInstance<Item_Float>();
                item_Float.itemName = itemImporter.itemName;
                item_Float.message = itemImporter.message;
                var filePathFloat = EditorUtility.SaveFilePanelInProject("TITLE", item_Float.itemName, "asset", "Please enter a file name to save the texture to");
                // パスの長さが０の時読み込みをキャンセルした
                if (string.IsNullOrEmpty(filePathFloat))
                {
                    return false;
                }
                var asset_Float = (Item_Float)AssetDatabase.LoadAssetAtPath(filePathFloat, typeof(Item_Float));
                if (asset_Float == null)
                {
                    //指定のパスにファイルが存在しない場合は新規作成
                    AssetDatabase.CreateAsset(item_Float, filePathFloat);
                }
                else
                {
                    //指定のパスに既に同名のファイルが存在する場合は更新
                    EditorUtility.CopySerialized(item_Float, asset_Float);
                    AssetDatabase.SaveAssets();
                }
                AssetDatabase.Refresh();
                break;
            case ValueType.DOUBLE:
                var item_Double = CreateInstance<Item_Double>();
                item_Double.itemName = itemImporter.itemName;
                item_Double.message = itemImporter.message;
                var filePathDouble = EditorUtility.SaveFilePanelInProject("TITLE", item_Double.itemName, "asset", "Please enter a file name to save the texture to");
                // パスの長さが０の時読み込みをキャンセルした
                if (string.IsNullOrEmpty(filePathDouble))
                {
                    return false;
                }
                var asset_Double = (Item_Double)AssetDatabase.LoadAssetAtPath(filePathDouble, typeof(Item_Double));
                if (asset_Double == null)
                {
                    //指定のパスにファイルが存在しない場合は新規作成
                    AssetDatabase.CreateAsset(item_Double, filePathDouble);
                }
                else
                {
                    //指定のパスに既に同名のファイルが存在する場合は更新
                    EditorUtility.CopySerialized(item_Double, asset_Double);
                    AssetDatabase.SaveAssets();
                }
                AssetDatabase.Refresh();
                break;
            default:
                break;
        }
        Debug.Log(itemImporter.name + " : アイテム作成が完了しました。");
        return true;
    }
}
#endif