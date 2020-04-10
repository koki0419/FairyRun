using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RunGame.Stage
{
    /// <summary>
    /// ステージ全体の崩壊処理を表します。
    /// </summary>
    public class StageController : MonoBehaviour
    {
        /// <summary>
        /// 破壊させるTilemapを指定します。
        /// </summary>
        [SerializeField]
        private Tilemap brokenTilemap = null;
        /// <summary>
        /// 崩壊時に生成するタイルプレハブを指定します。
        /// </summary>
        [SerializeField]
        private GameObject brokenTilePrefab = null;
        /// <summary>
        /// 崩壊が始まるまでのディレイ(秒)を指定します。
        /// </summary>
        [SerializeField]
        private float startDelay = 3;
        /// <summary>
        /// 崩壊間隔(秒)を指定します。
        /// </summary>
        [SerializeField]
        private float interval = 0.2f;

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(BreakTilemap(brokenTilemap));
        }

        /// <summary>
        /// 指定したTilemapを崩壊させます。
        /// </summary>
        /// <param name="tilemap">崩壊させたいTilemap</param>
        private IEnumerator BreakTilemap(Tilemap tilemap)
        {
            // 崩壊開始までのディレイ
            yield return new WaitForSeconds(startDelay);

            var position = Vector3Int.zero;
            var cellBounds = tilemap.cellBounds;

            for (int x = cellBounds.xMin; x <= cellBounds.xMax; x++)
            {
                for (int y = cellBounds.yMin; y <= cellBounds.yMax; y++)
                {
                    // セルの位置
                    position.x = x;
                    position.y = y;
                    // セル位置のタイルを取得
                    var tile = tilemap.GetTile(position);
                    // タイルの存在確認
                    if (tile != null)
                    {
                        // セルのワールド座標を取得
                        var worldPosition = tilemap.GetCellCenterWorld(position);
                        // Tilemapからタイルを削除
                        tilemap.SetTile(position, null);
                        // 崩壊タイルを生成
                        var brokenTile = Instantiate(
                            brokenTilePrefab, worldPosition, brokenTilePrefab.transform.rotation);
                    }
                }
                yield return new WaitForSeconds(interval);
            }
        }
    }
}