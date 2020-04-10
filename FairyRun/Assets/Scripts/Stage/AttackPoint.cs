using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPoint : MonoBehaviour
{
    //UIを表示する対象のオブジェクト
    private Transform target;
    [SerializeField, Tooltip("ターゲットからの表示距離")]
    private Vector3 offset;

    private RectTransform rect;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("Fairy").transform;
        rect = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        rect.position = RectTransformUtility.WorldToScreenPoint(Camera.main, target.position + offset);
    }
}
