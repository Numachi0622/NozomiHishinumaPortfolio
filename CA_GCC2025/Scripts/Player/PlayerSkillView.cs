using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using Utility;
using VolumeCoponent;
using VolumeCoponent.VolumeComponent;

public class PlayerSkillView : MonoBehaviour
{
    /// <summary>
    /// プレイヤーUIの親オブジェクト
    /// </summary>
    [SerializeField] private GameObject _pleyerView;

    /// <summary>
    /// フェード用のパネル
    /// </summary>
    [SerializeField] private Image _fadePanel;

    /// <summary>
    /// カットイン
    /// </summary>
    [SerializeField] private GameObject _cutIn;

    /// <summary>
    /// カットインで使用するキャラクター画像
    /// </summary>
    [SerializeField] private Image _character;

    /// <summary>
    /// 環境用PostProcessのコントローラ＝
    /// </summary>
    [SerializeField] private VolumeProfile _envVolume;

    /// <summary>
    /// エフェクト用PostProcessのコントローラー
    /// </summary>
    [SerializeField] private VolumeProfile _effVolume;

    /// <summary>
    /// キャラクターのアニメーション初期位置
    /// </summary>
    /// <returns></returns>
    private float _charaStartPosX = -1500f;

    /// <summary>
    /// キャラクターのアニメーション終了位置
    /// </summary>
    private float _charaEndPosX = 250f;

    /// <summary>
    /// VolumeのアニメーションSequence
    /// </summary>
    private Sequence _volumeSequence;

    /// <summary>
    /// フェードアニメーションのSequence
    /// </summary>
    private Sequence _fadeSequence;

    /// <summary>
    /// フェードさせる値
    /// </summary>
    private float _endFade = 0.5f;

    /// <summary>
    /// スキル発動演出
    /// </summary>
    public void StartSkill()
    {
        _volumeSequence?.Kill();
        
        _volumeSequence = DOTween.Sequence()
            .SetLink(gameObject)
            .Append(DOTween.To(
                () => new Color(1f, 1f, 1f, 1f),
                color =>
                {
                    if (_envVolume.TryGet(out ColorAdjustments colorAdjustments))
                    {
                        colorAdjustments ??= _envVolume.Add<ColorAdjustments>();
                        colorAdjustments.colorFilter.Override(color);
                    }
                },
                new Color(_endFade, _endFade, _endFade, 1f),
                0.5f))
            .Join(DOTween.To(
                () => 0f,
                intensity =>
                {
                    if (_effVolume.TryGet(out Bloom bloom))
                    {
                        bloom ??= _envVolume.Add<Bloom>();
                        bloom.intensity.Override(intensity);
                    }
                },
                1f, 0.5f))
            .AppendInterval(2f)
            .Append(DOTween.To(
                () => new Color(_endFade, _endFade, _endFade, 1f),
                color =>
                {
                    if (_envVolume.TryGet(out ColorAdjustments colorAdjustments))
                    {
                        colorAdjustments ??= _envVolume.Add<ColorAdjustments>();
                        colorAdjustments.colorFilter.Override(color);
                    }
                },
                new Color(1, 1, 1, 1f),
                0.5f))
            .Join(DOTween.To(
                () => 1f,
                intensity =>
                {
                    if (_effVolume.TryGet(out Bloom bloom))
                    {
                        bloom ??= _envVolume.Add<Bloom>();
                        bloom.intensity.Override(intensity);
                    }
                },
                0f, 0.5f));
    }
    
    /// <summary>
    /// スペシャルスキルの発動演出
    /// </summary>
    public void StartSpecialSkill(Action onComplete = null)
    {
        _fadeSequence?.Kill();

        _fadeSequence = DOTween.Sequence()
            .SetLink(gameObject)
            .AppendCallback(() =>
            {
                _character.transform.localPosition = new Vector3(_charaStartPosX, -667, 0);
                _cutIn.SetActive(true);
            })
            .Append(_character.transform.DOLocalMoveX(_charaEndPosX, 1f).SetEase(Ease.OutQuint))
            .AppendInterval(1f)
            .Append(_fadePanel.DOFade(1f, 1f))
            .AppendCallback(() =>
            {
                GameObserver.Instance.HideEnemyInSkillMode();
                _pleyerView.SetActive(false);
                onComplete?.Invoke();
            })
            .AppendInterval(0.5f)
            .Append(_fadePanel.DOFade(0f, 0.5f));
    }

    /// <summary>
    /// スペシャルスキルの発動後演出
    /// </summary>
    public void EndSpecialSkill(Action onComplete = null)
    {
        _fadeSequence?.Kill();

        _fadeSequence = DOTween.Sequence()
            .SetLink(gameObject)
            .Append(_fadePanel.DOFade(1f, 1f))
            .AppendCallback(() => onComplete?.Invoke())
            .AppendInterval(0.5f)
            .AppendCallback(() =>
            {
                _cutIn.SetActive(false);
                _pleyerView.SetActive(true);
            })
            .Append(_fadePanel.DOFade(0f, 0.5f));
    }
    
}
