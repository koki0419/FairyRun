using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunGame.Stage
{
    public class GroundGenerator : MonoBehaviour
    {
        //足場のオブジェクト
        GameObject[] groundPrefab = new GameObject[3];

        [SerializeField, Tooltip("最初の足場")]
        GameObject firstGround = null;

        //次の足場を置くポイント
        GameObject[] nextPoint = new GameObject[3];

        //カーブ後は直線を生成
        int straightCount = 0;

        // Start is called before the first frame update
        void Start()
        {
            //生成する地面を読み込み
            groundPrefab[0] = Resources.Load<GameObject>("Ground_1");
            groundPrefab[1] = Resources.Load<GameObject>("Ground_2");
            groundPrefab[2] = Resources.Load<GameObject>("Ground_3");
            SetNextPoint(firstGround);
        }

        //次の足場を生成
        public void SetNextGround()
        {
            int nextPointNum = Random.Range(0, 3);
            //カーブ後は直線を生成する
            if(straightCount > 0)
            {
                nextPointNum = 0;
                straightCount--;
            }
            if(nextPointNum != 0)
            {
                straightCount = 1;
            }

            //足場生成
            GameObject nextGround = Instantiate(groundPrefab[nextPointNum]);
            //生成した足場の位置を合わせる
            nextGround.transform.parent = transform;
            nextGround.transform.position = nextPoint[nextPointNum].transform.position;
            nextGround.transform.rotation = nextPoint[nextPointNum].transform.rotation;
            SetNextPoint(nextGround);
        }

        //次の足場から生成するポイントを取得
        private void SetNextPoint(GameObject ground)
        {
            for(int i = 0; i < nextPoint.Length;i++)
            {
                nextPoint[i] = ground.transform.GetChild(i + 4).gameObject;
            }
        }
    }
}