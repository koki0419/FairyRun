using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RunGame.Stage
{
    [Serializable]
    [CreateAssetMenu(fileName = "New Animated Tile", menuName = "Tiles/Animated Tile")]
    public class AnimatedTile : TileBase
    {
        /// <summary>
        /// タイルとして描画されるスプライトを指定します。
        /// </summary>
        [SerializeField]
        internal Sprite sprite;

        /// <summary>
        /// タイルのを指定します。
        /// </summary>
        [SerializeField]
        private Sprite[] animatedSprites;
        /// <summary>
        /// タイルのを指定します。
        /// </summary>
        [SerializeField]
        private float animationSpeed = 1.0f;
        /// <summary>
        /// このアニメーションタイルの開始時間(秒)を指定します。
        /// </summary>
        [SerializeField]
        private float animationStartTime = 0;

        /// <summary>
        /// タイルの色を指定します。
        /// </summary>
        [SerializeField]
        private Color color = Color.white;
        /// <summary>
        /// タイルの変換行列を指定します。
        /// </summary>
        [SerializeField]
        private Matrix4x4 transform = Matrix4x4.identity;

        /// <summary>
        /// タイルのGameObjectを指定します。
        /// </summary>
        [SerializeField]
        private GameObject instantiatedGameObject = null;
        /// <summary>
        /// タイルのTileFlagsを指定します。
        /// </summary>
        [SerializeField]
        private TileFlags flags = TileFlags.LockColor;
        /// <summary>
        /// タイルのColliderTypeを指定します。
        /// </summary>
        [SerializeField]
        private Tile.ColliderType colliderType = Tile.ColliderType.Sprite;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position">タイルマップ内のこのタイルの位置</param>
        /// <param name="tilemap">所属するタイルマップ</param>
        /// <param name="go">プレハブから生成されたGameObject</param>
        /// <returns></returns>
        public override bool StartUp(
            Vector3Int position, ITilemap tilemap,
            GameObject go)
        {
            Debug.LogFormat("StartUp({0}, {1})", position, go);
            return base.StartUp(position, tilemap, go);
        }

        /// <summary>
        /// スクリプタブルタイルからアニメーションデータを取得します。
        /// </summary>
        /// <param name="position">タイルマップ内のこのタイルの位置</param>
        /// <param name="tilemap">所属するタイルマップ</param>
        /// <param name="tileAnimationData">タイルのアニメーションを実行するためのデータ</param>
        /// <returns>このメソッドの呼び出しが成功したかどうか</returns>
        public override bool GetTileAnimationData(
            Vector3Int position, ITilemap tilemap,
            ref TileAnimationData tileAnimationData)
        {
            Debug.LogFormat("GetTileAnimationData({0})", position);
            // アニメーションしない場合
            if (animatedSprites == null || animatedSprites.Length == 0)
            {
                return false;
            }
            tileAnimationData.animatedSprites = animatedSprites;
            tileAnimationData.animationSpeed = animationSpeed;
            tileAnimationData.animationStartTime = animationStartTime;
            return true;
        }

        /// <summary>
        /// タイルマップ上でタイルがどのように表示されるかを特定します。
        /// </summary>
        /// <param name="position">タイルマップ内のこのタイルの位置</param>
        /// <param name="tilemap">所属するタイルマップ</param>
        /// <param name="tileData">タイルマップ上の表示に使用されるデータ</param>
        public override void GetTileData(
            Vector3Int position, ITilemap tilemap,
            ref TileData tileData)
        {
            Debug.LogFormat("GetTileData({0})", position);

            tileData.sprite = this.sprite;

            tileData.color = this.color;
            tileData.transform = this.transform;
            tileData.gameObject = this.instantiatedGameObject;
            tileData.flags = this.flags;
            tileData.colliderType = this.colliderType;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(AnimatedTile))]
    public class AnimatedTileEditor : Editor
    {
        public override Texture2D RenderStaticPreview(string assetPath, UnityEngine.Object[] subAssets, int width, int height)
        {
            var animatedTile = target as AnimatedTile;
            var icon = animatedTile.sprite;
            if (icon == null)
            {
                return base.RenderStaticPreview(assetPath, subAssets, width, height);
            }

            var preview = AssetPreview.GetAssetPreview(icon);
            var final = new Texture2D(width, height);
            EditorUtility.CopySerialized(preview, final);
            return final;
        }
    }
#endif
}
