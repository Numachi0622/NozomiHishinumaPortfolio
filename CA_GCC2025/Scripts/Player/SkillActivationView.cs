using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using R3;
using R3.Triggers;

public class SkillActivationView : MonoBehaviour
{
    /// <summary>
    /// スキル発動UI
    /// </summary>
    [SerializeField] private GameObject _skillActivator;

    /// <summary>
    /// スキル発動UIのアニメーションSequence;
    /// </summary>
    private Sequence _activatorSequence;

    /// <summary>
    /// スキル発動UIの初期座標
    /// </summary>
    private Vector2 _prevPos;

    /// <summary>
    /// 最後に選択したUIのTransform
    /// </summary>
    private Transform _lastSelectTransform;
    
    /// <summary>
    /// ドラッグ中フラグ
    /// </summary>
    private bool _isDragging = false;

    /// <summary>
    /// スキル発動UIを表示
    /// </summary>
    public void ShowSkillUI()
    {
        _isDragging = true;
        _skillActivator.SetActive(true);
    }

    /// <summary>
    /// ドラッグ状態をリセット
    /// </summary>
    public void ResetDrag()
    {
        _isDragging = false;
        _skillActivator.SetActive(false);
    }

    /// <summary>
    /// スキル発動UIを振動させる
    /// </summary>
    /// <param name="model"></param>
    public void Shake(SkillActivationModel model)
    {
        _lastSelectTransform = model.transform;
        _prevPos = model.transform.localPosition;
        _activatorSequence?.Kill();

        _activatorSequence = DOTween.Sequence()
            .Append(model.transform.DOShakePosition(10f, 7f, 10, 1f, false, true)
                .OnComplete(() => model.transform.localPosition = _prevPos))
            .Join(model.transform.DOScale(Vector3.one * 1.2f, 0.2f));
    }

    /// <summary>
    /// 振動を停止
    /// </summary>
    public void ResetShake()
    {
        _activatorSequence.Kill(true);
        
        if(_lastSelectTransform == null) return;
        _lastSelectTransform.localScale = Vector3.one;
    }
}
