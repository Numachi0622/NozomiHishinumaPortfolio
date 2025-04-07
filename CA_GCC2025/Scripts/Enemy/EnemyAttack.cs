using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Interface;
using UnityEngine;
using Utility;
using State = EnemyState.State;

public class EnemyAttack : MonoBehaviour, IAttackable
{
    /// <summary>
    /// 敵のパラメータデータ
    /// </summary>
    private EnemyParams _enemyParams;

    /// <summary>
    /// 敵のAnimator
    /// </summary>
    private Animator _animator;

    /// <summary>
    /// 敵のStateModel
    /// </summary>
    private IState<EnemyState.State> _stateModel;

    /// <summary>
    /// 敵のAttacker
    /// </summary>
    private Attacker _attacker;

    /// <summary>
    /// 攻撃準備状態に入ったときの処理を格納
    /// </summary>
    public Action OnAttackReady;

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="enemyParams"></param>
    /// <param name="animator"></param>
    /// <param name="attacker"></param>
    /// <param name="stateModel"></param>
    public void Initialize(EnemyParams enemyParams, Animator animator, Attacker attacker, IState<State> stateModel)
    {
        _enemyParams = enemyParams;
        _animator = animator;
        _attacker = attacker;
        _stateModel = stateModel;

        _attacker.OnTriggerEnterEvent = () =>
        {
            SoundManager.Instance.PlaySe(SoundType.EnemyAttackHit);
        };
    }

    /// <summary>
    /// 攻撃準備に入る
    /// </summary>
    public async UniTaskVoid AttackReady()
    {
        if(!IsAttackable) return;
        
        _stateModel.SetState(State.AttackReady);
        
        _animator.SetBool("IsMove", false);
        _animator.SetTrigger("AttackReady");
        
        OnAttackReady?.Invoke();
        
        SoundManager.Instance.PlaySe(SoundType.EnemyAttackReady);
        
        await UniTask.Delay(TimeSpan.FromSeconds(_enemyParams.AttackReadyTime));
        Attack();
    }

    /// <summary>
    /// 攻撃
    /// </summary>
    public void Attack()
    {
        _stateModel.SetState(State.Attack);
        _animator.SetTrigger("Attack");
    }

    /// <summary>
    /// 攻撃Colliderの有効タイミングを設定
    /// StateMachineBehaviourを継承したクラスから呼び出される
    /// </summary>
    /// <param name="waitTime"></param>
    public void AttackImpact(float waitTime)
    {
        var power = _enemyParams.BasePower.GetRandomPower;
        _attacker.SetData(power, HitType.FromEnemy);
        
        _attacker.EnableAttackCollider(waitTime, () =>
        {
            SoundManager.Instance.PlaySe(SoundType.EnemyAttack);
        }).Forget();
    }

    /// <summary>
    /// 攻撃を終了する
    /// </summary>
    public void AttackEnd()
    {
        if(_stateModel.CurrentState == State.Idle || 
           _stateModel.CurrentState == State.AttackReady) return;
        
        AttackCoolTime().Forget();
    }
    
    /// <summary>
    /// 攻撃後のクールダウンを適用
    /// 攻撃終了時に呼ばれる
    /// </summary>
    private async UniTaskVoid AttackCoolTime()
    {
        _stateModel.SetState(State.AttackCoolTime);
        await UniTask.Delay(TimeSpan.FromSeconds(_enemyParams.AttackCoolTime));
        _stateModel.SetState(State.Idle);
    }

    /// <summary>
    /// 攻撃可能かを判定する
    /// </summary>
    private bool IsAttackable => _stateModel.CurrentState == State.Idle;
}
