using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterIntroduction : MonoBehaviour
{
    [SerializeField]
    private Animator player_1_Animator = null;
    [SerializeField]
    private Animator player_2_Animator = null;

    [SerializeField]
    private CharaAnimationChecker player_1_Checker = null;
    [SerializeField]
    private CharaAnimationChecker player_2_Checker = null;

    [SerializeField]
    private bool player_1_IsAnimation = false;
    [SerializeField]
    private bool player_2_IsAnimation = false;

    public bool Player_1_IsAnimation
    {
        get { return player_1_IsAnimation; }
    }
    public bool Player_2_IsAnimation
    {
        get { return player_2_IsAnimation; }
    }

    private void Update()
    {
        player_1_IsAnimation = player_1_Checker.AnimationChecker;
        player_2_IsAnimation = player_2_Checker.AnimationChecker;
    }
    // アニメーションを管理
    public void PlayAnimation_player_1()
    {
        if (!Player_1_IsAnimation)
        {
            player_1_Animator.SetTrigger("InputKey");
            return;
        }
        return;
    }
    public void PlayAnimation_player_2()
    {
        if (!Player_2_IsAnimation)
        {
            player_2_Animator.SetTrigger("InputKey");
            return;
        }
        return;
    }
    public void CheckAnimationExit()
    {
        AnimatorStateInfo animInfo = player_2_Animator.GetCurrentAnimatorStateInfo(0);
        if (animInfo.normalizedTime < 1.0f)
        {
            //player_2_Animator.CrossFeede("player_2_Animator");
        }
    }
}
