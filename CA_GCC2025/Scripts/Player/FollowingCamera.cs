using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

public class FollowingCamera : MonoBehaviour
{
    /// <summary>
    /// 最大追従速度
    /// </summary>
    [SerializeField] private float _maxSpeed = 10f;

    /// <summary>
    /// 追従速度が最大になる距離
    /// </summary>
    [SerializeField] private float _maxSpeedDistance = 2f;

    /// <summary>
    /// 追従対象のTransform
    /// </summary>
    private Transform _target;
    
    /// <summary>
    /// カメラとターゲットのオフセット
    /// </summary>
    private Vector3 _offset;

    /// <summary>
    /// 振動の際のオフセット
    /// </summary>
    private Vector3 _shakeOffset;

    /// <summary>
    /// 振動アニメーションのSequence
    /// </summary>
    private Sequence _shakeSequence;

    /// <summary>
    /// ズームアニメーションのSequence
    /// </summary>
    private Sequence _zoomSequence;

    /// <summary>
    /// ズーム中のフラグ
    /// </summary>
    private bool _isZooming;

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="target"></param>
    public void Initialize(Transform target)
    {
        _target = target;
        _offset = transform.position - _target.position;
    }

    private void Update() => Follow();

    /// <summary>
    /// 追従処理
    /// </summary>
    private void Follow()
    {
        if(_target == null) return;
        if (_isZooming) return;
        
        var currentPos = transform.position;
        var targetPos = _target.position + _offset;
        var distance = Vector3.Distance(currentPos, targetPos);
        var followSpeed = _maxSpeed * Mathf.InverseLerp(0f, _maxSpeedDistance, distance);
        transform.position = Vector3.MoveTowards(currentPos, targetPos, followSpeed * Time.deltaTime);
        
        // 振動のパラメータも加算
        transform.position += _shakeOffset;
    }

    /// <summary>
    /// 振動
    /// </summary>
    public void Shake()
    {
        if(_target == null) return;
        
        // 振動中の場合キャンセルする
        _shakeSequence?.Kill();

        _shakeSequence = DOTween.Sequence()
            .SetLink(gameObject)
            .Append(DOTween.Shake(() => Vector3.zero, offset => _shakeOffset = offset, 0.3f, 0.02f, 30));
    }

    /// <summary>
    /// ズームさせる
    /// </summary>
    public void Zoom()
    {
        if(_target == null) return;

        _isZooming = true;
        var dir = (_target.position - transform.position).normalized;
        
        _zoomSequence?.Kill();

        _zoomSequence = DOTween.Sequence()
            .SetLink(gameObject)
            .Append(transform.DOMove(dir, 0.5f).SetRelative(true))
            .AppendInterval(2f)
            .AppendCallback(() => _isZooming = false);
    }
    
}
