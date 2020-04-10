using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaAnimationChecker : MonoBehaviour
{
    bool isAnimation = false;

    public bool AnimationChecker
    {
        get { return isAnimation; }
    }

    public void ReleaseAnimationKey()
    {
        isAnimation = false;
    }
    public void StartAnimationKey()
    {
        isAnimation = true;
    }
}
