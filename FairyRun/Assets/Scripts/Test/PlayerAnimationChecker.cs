using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationChecker : MonoBehaviour
{
    private bool checkBaibaiFinish = false;
    public bool GetBaibaiFinish
    {
        get { return checkBaibaiFinish; }
        set { checkBaibaiFinish = value; }
    }

    public void SetBaibaiFinish()
    {
        checkBaibaiFinish = true;
    }
}
