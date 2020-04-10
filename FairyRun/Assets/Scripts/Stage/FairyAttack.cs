using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyAttack : MonoBehaviour
{
    //妖精の攻撃の速度(Fairyによって設定)
    float bulletSpeed;
    //妖精の攻撃の飛ぶ向き(Fairyによって設定)
    Vector3 bulletDirection;
    //妖精の攻撃が消えるまでの時間(秒,Fairyによって設定)
    float bulletRemainSecond;

    // Update is called once per frame
    void Update()
    {
        transform.position += bulletDirection * bulletSpeed;
        bulletRemainSecond -= Time.deltaTime;
        if(bulletRemainSecond <= 0)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 妖精の攻撃の情報を設定
    /// </summary>
    /// <param name="bSpeed">弾の飛ぶ速度</param>
    /// <param name="bDirection">弾の飛ぶ方向</param>
    /// <param name="bRemain">弾が何秒後に消えるか</param>
    public void SetBulletStats(float bSpeed,Vector3 bDirection,float bRemain)
    {
        bulletSpeed = bSpeed;
        bulletDirection = bDirection;
        bulletRemainSecond = bRemain;
    }
}
