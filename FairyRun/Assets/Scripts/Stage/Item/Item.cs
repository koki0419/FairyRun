using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public Item_Int item = null;
    private int point = 0;
    private const string player_LayerName = "Player";
    // Start is called before the first frame update
    void Start()
    {
        point = 10;
    }
    public void Initialize()
    {
        point = item.itemEffect;
    }
    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == player_LayerName)
        {
            ScoreManager.scoerPoint += point;
            Debug.Log("当たった");
            gameObject.SetActive(false);
        }
    }
}
