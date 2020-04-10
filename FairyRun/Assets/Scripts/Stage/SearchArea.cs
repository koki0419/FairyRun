using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        transform.parent.SendMessage("OnChildrenTriggerEnter", other);
    }

    private void OnTriggerExit(Collider other)
    {
        transform.parent.SendMessage("OnChildrenTriggerExit", other);
    }
}
