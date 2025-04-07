using System;
using DG.Tweening;
using Interface;
using UnityEngine;
using Utility;

public class PlayerMovement : IMovable
{
    /// <summary>
    /// プレイヤーのパラメータデータ
    /// </summary>
    /// <returns></returns>
    private PlayerParams _playerParams;
    
    /// <summary>
    /// プレイヤーのTransform
    /// </summary>
    private Transform _playerTransform;
    
    /// <summary>
    /// プレイヤーのAniamtor
    /// </summary>
    private Animator _animator;

    /// <summary>
    /// 攻撃時の回転アニメーションのSequence
    /// </summary>
    private Sequence _attackRotateSequence;
    
    /// <summary>
    /// ダッシュアタックアニメーションのSequence
    /// </summary>
    private Sequence _dashAttackSequence;

    /// <summary>
    /// 攻撃時の回転中フラグ
    /// </summary>
    private bool _isAttackRotating = false;

    /// <summary>
    /// ダッシュアタック中フラグ
    /// </summary>
    private bool _isDashAttacking = false;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="playerParams"></param>
    /// <param name="animator"></param>
    /// <param name="transform"></param>
    public PlayerMovement(PlayerParams playerParams, Animator animator, Transform transform)
    {
        _playerParams = playerParams;
        _animator = animator;
        _playerTransform = transform;
    }

    /// <summary>
    /// 移動
    /// </summary>
    /// <param name="direction">移動方向</param>
    public void Move(Vector3 direction)
    {
        var speedRate = direction.magnitude;
        var speed = Mathf.Lerp(0f, _playerParams.MaxSpeed, speedRate);
        var moveDir = direction * (speed * Time.deltaTime);
        var movedPos = _playerTransform.position + moveDir;
        movedPos.x = Mathf.Clamp(movedPos.x, GameConst.MIN_MOVABLE_AREA, GameConst.MAX_MOVABLE_AREA);
        movedPos.z = Mathf.Clamp(movedPos.z, GameConst.MIN_MOVABLE_AREA, GameConst.MAX_MOVABLE_AREA);
        
        _playerTransform.LookAt(movedPos);
        _playerTransform.position = movedPos;
        
        _animator.SetFloat("Speed", speedRate);

        if (direction.magnitude < 0.5f)
        {
            SoundManager.Instance.PlaySeContinue(SoundType.Walk);
            return;
        }
        
        SoundManager.Instance.PlaySeContinue(SoundType.Run);
        PlayerEffectManager.Instance.PlayMoveEffect();
    }

    /// <summary>
    /// 攻撃時にターゲットの方向に回転する
    /// </summary>
    /// <param name="target">ターゲットのTransform</param>
    public void AttackRotate(Transform target)
    {
        if (_isAttackRotating) return;

        _isAttackRotating = true;
        
        // ターゲットへの方向を計算
        var targetDir = (target.position - _playerTransform.position).normalized;
        var targetAngleY = Quaternion.LookRotation(targetDir).eulerAngles.y;
        
        // 再生中のアニメーションをキャンセル
        _attackRotateSequence?.Kill();

        // 回転アニメーション再生
        _attackRotateSequence = DOTween.Sequence()
            .SetLink(_playerTransform.gameObject)
            .Append(_playerTransform.DORotateQuaternion(Quaternion.Euler(0, targetAngleY, 0), _playerParams.AttackRotateTime))
            .AppendCallback(() => _isAttackRotating = false);
    }

    /// <summary>
    /// 攻撃時にターゲットの方向にダッシュアタックする
    /// </summary>
    /// <param name="target">ターゲットのTransform</param>
    public void DashAttack(Transform target)
    {
        if(_isDashAttacking) return;
        
        // ターゲットへの方向を計算
        var targetPos = target.position;
        var targetDir = (targetPos - _playerTransform.position).normalized;
        
        // ターゲットに近すぎる場合はダッシュ不可
        var dist = Vector3.Distance(targetPos, _playerTransform.position);
        if (dist < _playerParams.DashAttackOffset) return;
        
        // 進行距離を計算
        var destination = targetPos - targetDir * _playerParams.DashAttackOffset;
        var dashDist = Mathf.Min(Vector3.Distance(destination, _playerTransform.position), _playerParams.MaxDashMoveDist);
        var moveValue = targetDir * dashDist;
        moveValue.y = 0f;

        _isDashAttacking = true;
        
        // 再生中のアニメーションをキャンセル
        _dashAttackSequence?.Kill();

        // ダッシュアタックアニメーション再生
        _dashAttackSequence = DOTween.Sequence()
            .SetLink(_playerTransform.gameObject)
            .Append(_playerTransform.DOMove(moveValue, _playerParams.DashAttackTime).SetRelative(true))
            .AppendCallback(() => _isDashAttacking = false);
    }
}
