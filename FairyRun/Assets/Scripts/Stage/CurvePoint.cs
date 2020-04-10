using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurvePoint : MonoBehaviour
{
    //次のカーブをどっちに曲がるか(0=左/1=曲がらない/2=右)
    [SerializeField]
    int nextCurve;
    public int debug = 0;
    public void SetNextCurve(int nextCurveIndex,int set)
    {
        nextCurve = nextCurveIndex;
        debug = set;
    }
    /// <summary>
    /// 次のカーブをどちらに曲がるか取得
    /// </summary>
    /// <returns>0=左/1=曲がらない/2=右</returns>
    public int GetNextCurve()
    {
        return nextCurve;
    }

}
