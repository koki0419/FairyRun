using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFairyAttack : MonoBehaviour
{
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var pos = transform.position;
        pos.z += speed;
        transform.position = pos;
    }
}
