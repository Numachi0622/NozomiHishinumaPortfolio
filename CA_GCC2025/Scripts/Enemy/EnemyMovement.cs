using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Interface;
using UnityEngine;
using Utility;

public class EnemyMovement : IMovable
{
    /// <summary>
    /// 移動対象のTransform
    /// ToDo : 後々自動で進むように変更する
    /// </summary>
    private Transform _target;

    /// <summary>
    /// 敵のパラメータデータ
    /// </summary>
    private EnemyParams _enemyParams;

    /// <summary>
    /// 敵のAnimator
    /// </summary>
    private Animator _animator;

    /// <summary>
    /// 敵のTransform
    /// </summary>
    private Transform _enemyTransform;

    /// <summary>
    /// ノックバックアニメーションのSequence
    /// </summary>
    private Sequence _knockBackSequence;
    
    /// <summary>
    /// 攻撃対象の近くで停止した際の処理を格納
    /// </summary>
    public Action OnStopNearTarget;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="enemyParams"></param>
    /// <param name="enemyTransform"></param>
    public EnemyMovement(EnemyParams enemyParams,Transform enemyTransform)
    {
        _enemyParams = enemyParams;
        _enemyTransform = enemyTransform;
        
        // 一旦タグで取得
        _target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    /// <summary>
    /// 移動
    /// </summary>
    /// <param name="targetPos">ターゲットの座標</param>
    public void Move(Vector3 targetPos)
    {
        if(GameObserver.Instance.IsGameEnd) return;
        if(GameObserver.Instance.IsSkillTime) return;
        
        targetPos = _target.position;
        _enemyTransform.LookAt(targetPos);
        
        var currentPos = _enemyTransform.position;
        var distance = Vector3.Distance(targetPos, currentPos);
        if (distance <= _enemyParams.AttackRange)
        {
            OnStopNearTarget?.Invoke();
            return;
        }
        var maxDistanceDelta = _enemyParams.MaxSpeed * Time.deltaTime;
        _enemyTransform.position = Vector3.MoveTowards(currentPos, targetPos, maxDistanceDelta);
    }

    /// <summary>
    /// 被ダメ時のノックバック
    /// </summary>
    public void KnockBack(bool isDead = false, Action onComplete = null)
    {
        var destMag = isDead ? 4f : 1.2f;
        
        _knockBackSequence?.Kill();

        var destination = _enemyTransform.position - _enemyTransform.forward * destMag;
        _knockBackSequence = DOTween.Sequence()
            .SetLink(_enemyTransform.gameObject)
            .Append(_enemyTransform.DOMove(destination, 0.5f).SetEase(Ease.OutCubic))
            .OnComplete(() => onComplete?.Invoke());
    }
}
