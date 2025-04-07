using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using R3;
using R3.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillActivationPresenter : MonoBehaviour
{
    /// <summary>
    /// 通常スキル発動を検知するクラス
    /// </summary>
    [SerializeField] private SkillActivationModel _skillActModel;
    
    /// <summary>
    /// スペシャルスキル発動を検知するクラス
    /// </summary>
    [SerializeField] private SkillActivationModel _specialSkillActModel;
    
    /// <summary>
    /// スキル発動ビュー
    /// </summary>
    private SkillActivationView _view;

    /// <summary>
    /// CanvasにあるGraphicRaycaster
    /// </summary>
    private GraphicRaycaster _raycaster;

    /// <summary>
    /// スキル発動モデル
    /// </summary>
    /// <returns></returns>
    private SkillActivationModel _model;

    /// <summary>
    /// 通常スキルを開始する処理を格納
    /// </summary>
    public Action OnSkillActivate;

    /// <summary>
    /// スペシャルスキルを開始する処理を格納
    /// </summary>
    public Action OnSpecialSkillActivate;
    
    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize()
    {
        var trigger = GetComponent<ObservableEventTrigger>();
        _view = GetComponent<SkillActivationView>();
        _raycaster = transform.root.GetComponent<GraphicRaycaster>();

        _skillActModel.OnSkillActivate = OnSkillActivate;
        _specialSkillActModel.OnSkillActivate = OnSpecialSkillActivate;

        trigger.OnBeginDragAsObservable()
            .Subscribe(_ => _view.ShowSkillUI())
            .AddTo(this);

        trigger.OnDragAsObservable()
            .Select(eventData =>
            {
                var raycasts = new List<RaycastResult>();
                _raycaster.Raycast(eventData, raycasts);

                var results = raycasts
                    .Select(activator => activator.gameObject.GetComponent<SkillActivationModel>())
                    .Where(activator => activator != null)
                    .ToList();

                if (results.Count > 0)
                {
                    return results[0];
                }
                return null;
            })
            .Subscribe(activator =>
            {
                SoundManager.Instance.PlaySeContinue(SoundType.SkillReady);
                if (activator == _model) return;

                _view.ResetShake();
                _model = activator;
                if (_model == null) return;
                
                
                _view.Shake(activator);
                SoundManager.Instance.PlaySe(SoundType.SkillReadySelect);
            })
            .AddTo(this);

        trigger.OnPointerUpAsObservable()
            .Select(_ =>
            {
                _view.ResetDrag();
                SoundManager.Instance.StopSeContinue();
                return _model;
            })
            .Where(activator => activator != null)
            .Subscribe(activator =>
            {
                activator.SkillActivate();
                _view.ResetDrag();
                SoundManager.Instance.PlaySe(SoundType.SKillActivate);
            })
            .AddTo(this);
    }
    
}
