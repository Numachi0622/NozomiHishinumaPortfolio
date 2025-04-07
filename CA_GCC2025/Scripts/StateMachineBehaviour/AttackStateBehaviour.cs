using System.Collections;
using System.Collections.Generic;
using Interface;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Utility;

public class AttackStateBehaviour : StateMachineBehaviour
{
    [SerializeField] private float _waitFrame;
    private IAttackable _attack;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_attack == null)
        {
            _attack = animator.GetComponent<IAttackable>();
        }

        var waitFrame = _waitFrame / GameConst.ANIM_FPS;
        _attack.AttackImpact(waitFrame);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    // public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    // {
    // }
    
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(_attack == null || _attack is not IComboAttackable) return;
        
        ((IComboAttackable)_attack).AttackEndPerCombo();
    }

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
