using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HitPointView : MonoBehaviour
{
    /// <summary>
    /// ゲージのバー表示のImage
    /// </summary>
    [SerializeField] private Image _gauge;

    /// <summary>
    /// 差分ゲージのバー表示のImage
    /// </summary>
    [SerializeField] private Image _diffGauge;

    /// <summary>
    /// HP残量のテキスト
    /// </summary>
    [SerializeField] private TextMeshProUGUI _hpText;

    /// <summary>
    /// 最大HP
    /// </summary>
    private int _maxHp;

    /// <summary>
    /// HPゲージアニメーションのSequence
    /// </summary>
    private Sequence _gaugeSequence;

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize(int maxHp)
    {
        _maxHp = maxHp;
        _hpText.text = $"{_maxHp}/{_maxHp}";
    }

    /// <summary>
    /// HPゲージ更新
    /// </summary>
    public void UpdateHp(int hp)
    {
        var rate = (float)hp / _maxHp;
        
        // アニメーション再生中であればキャンセル
        _gaugeSequence?.Kill();

        // アニメーション再生
        _gaugeSequence = DOTween.Sequence()
            .SetLink(gameObject)
            .Append(_gauge.DOFillAmount(rate, 0.1f).SetEase(Ease.Linear))
            .Append(_diffGauge.DOFillAmount(rate, 0.5f).SetDelay(0.2f));

        _hpText.text = $"{hp}/{_maxHp}";
    }
}
