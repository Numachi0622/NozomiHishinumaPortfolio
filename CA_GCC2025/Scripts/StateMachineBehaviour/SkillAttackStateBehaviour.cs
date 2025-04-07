﻿using System.Collections;
using System.Collections.Generic;
using Interface;
using UnityEngine;
using Utility;

public class SkillAttackStateBehaviour : StateMachineBehaviour
{
    [SerializeField] private float _waitTime;
    private ISkillAttackable _attack;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_attack == null)
        {
            _attack = animator.GetComponent<ISkillAttackable>();
        }

        var waitFrame = _waitTime / GameConst.ANIM_FPS;
        _attack.AttackImpact(waitFrame);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
