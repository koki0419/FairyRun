using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // 弾のスピード
    [SerializeField] private float bulletSpeed = 100;
    private new Rigidbody rigidbody;
    // 弾の飛んでいく力の向き
    private Vector3 force = Vector3.zero;
    //弾が消えるまでの時間(秒,Fairyによって設定)
    [SerializeField] private float bulletRemainSecond;

    private ParticleSystem particle;

    float destoryTimer = 9999;
    private void Update()
    {
        var velocity = rigidbody.velocity;
        velocity = force;
        rigidbody.velocity = velocity;

        bulletRemainSecond -= Time.deltaTime;
        if (bulletRemainSecond <= 0)
        {
            var em = particle.emission;
            em.rateOverTime = 0;
            destoryTimer = 3;
            GetComponent<MeshRenderer>().enabled = false;
        }

        destoryTimer -= Time.deltaTime;
        if(destoryTimer < 0)
        {
            Destroy(gameObject);
        }
    }
    public void ShootInit()
    {
        rigidbody = GetComponent<Rigidbody>();
        particle = GetComponentInChildren<ParticleSystem>();
        force = this.gameObject.transform.forward * bulletSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        var target_Tag = other.gameObject.tag;
        if(target_Tag == "Fiary" || target_Tag == "Enemy")
        {
            Destroy(gameObject);
        //}else if(target_Tag == "EnemyBullet" || target_Tag == "FairyBullet")
        //仮仕様:妖精の弾が一方的に敵の弾を消すように
        }else if(target_Tag == "FairyAttack")
        {
            var em = particle.emission;
            em.rateOverTime = 0;
            destoryTimer = 3;
            GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
