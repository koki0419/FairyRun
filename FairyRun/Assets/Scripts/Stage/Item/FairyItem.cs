using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyItem : MonoBehaviour
{
    private ItemEntry item = null;
    private int point = 0;
    private const string player_LayerName = "Player";
    [SerializeField]
    private bool debug = false;

    private SEManager seManager;
    // Start is called before the first frame update
    void Start()
    {
        seManager = Camera.main.GetComponent<SEManager>();
        if (debug)
        {
            point = 1;
        }
    }
    public void Initialize(ItemEntry item)
    {
        this.item = item;
        point = item.point;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == player_LayerName)
        {
            seManager.PlaySound(SEManager.SEName.ITEM);
            RunGame.Stage.SceneController.fairyPoint += point;
            gameObject.SetActive(false);
        }
    }
}
