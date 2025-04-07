using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using System;

public class SpecialSkillEffect : MonoBehaviour
{
    /// <summary>
    /// 振動させるカメラ
    /// </summary>
    [SerializeField] private Camera _shakeCamera;

    /// <summary>
    /// カメラの振動アニメーションのSequence
    /// </summary>
    private Sequence _shakeSequence;

    /// <summary>
    /// スキル演出が全て終了したときの処理を格納
    /// </summary>
    public Action OnCompleteEffect;

    /// <summary>
    /// スキル用のカメラ振動
    /// </summary>
    /// <param name="strength">振動の強さ</param>
    public void ExplosionShake(float strength)
    {
        _shakeSequence?.Kill();

        _shakeSequence = DOTween.Sequence()
            .SetLink(gameObject)
            .Append(_shakeCamera.transform.DOShakePosition(1f, strength))
            .SetUpdate(UpdateType.Late);
    }

    /// <summary>
    /// スキル演出を終了させる
    /// </summary>
    public void EndEffect()
    {
        OnCompleteEffect?.Invoke();
    }

}