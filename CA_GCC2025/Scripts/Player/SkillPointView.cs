using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;

public class SkillPointView : MonoBehaviour
{
    /// <summary>
    /// ゲージのバー表示のImage
    /// </summary>
    [SerializeField] private Image[] _gauge;

    /// <summary>
    /// スキルレベルに応じて切り替わるフロントアイコン
    /// </summary>
    [SerializeField] private Image[] _front;

    /// <summary>
    /// SP残量のテキスト
    /// </summary>
    [SerializeField] private TextMeshProUGUI _spText;

    /// <summary>
    /// 通常スキルの発動検知するUI
    /// </summary>
    [SerializeField] private Image _skillActivator;

    /// <summary>
    /// スペシャルスキルの発動検知するUI
    /// </summary>
    [SerializeField] private Image _specialSkillActivator;

    /// <summary>
    /// SPゲージアニメーションSequence
    /// </summary>
    private Sequence _gaugeSequence;

    /// <summary>
    /// スキルてきるのアニメーションSequence
    /// </summary>
    private Sequence _skillTextSequence;

    /// <summary>
    /// 更新前のSP
    /// </summary>
    [SerializeField]private float _prevSp = 0;

    /// <summary>
    /// スキル発動UIが非活性のときのカラー
    /// </summary>
    /// <returns></returns>
    private Color _enableColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    

    /// <summary>
    /// SPゲージ更新
    /// </summary>
    public void UpdateSpGauge(float sp)
    {
        if(sp > GameConst.MAX_SKILL_POINT || sp < 0) return;

        var isIncrease = _prevSp <= sp;

        if (isIncrease)
        {
            IncreaseSpAnim(sp);
        }
        else
        {
            DecreaseSpAnim(sp);   
        }
    }

    /// <summary>
    /// SPゲージの増加アニメーション
    /// </summary>
    /// <param name="sp"></param>
    private void IncreaseSpAnim(float sp)
    {
        _gaugeSequence?.Kill();
        _gaugeSequence = DOTween.Sequence()
            .SetLink(gameObject);
        
        var currentSpLevel = Mathf.FloorToInt(_prevSp);
        var totalDiff = sp - (currentSpLevel + _gauge[currentSpLevel].fillAmount);
        if (_gauge[currentSpLevel].fillAmount + totalDiff < 1.0f)
        {
            _gaugeSequence.Append(_gauge[currentSpLevel]
                .DOFillAmount(_gauge[currentSpLevel].fillAmount + totalDiff, 0.2f));
        }
        else
        {
            var excess = _gauge[currentSpLevel].fillAmount + totalDiff - 1.0f;
            _gaugeSequence.Append(_gauge[Mathf.Min(currentSpLevel, GameConst.MAX_SKILL_POINT - 1)].DOFillAmount(1.0f, 0.2f));

            if (sp >= GameConst.MAX_SKILL_POINT)
            {
                _gaugeSequence.AppendCallback(() =>
                {
                    currentSpLevel = GameConst.MAX_SKILL_POINT;
                    _prevSp = currentSpLevel;
                });
                return;
            }
            _gaugeSequence.Append(_gauge[Mathf.Min(currentSpLevel + 1, GameConst.MAX_SKILL_POINT - 1)].DOFillAmount(excess, 0.2f))
                .AppendCallback(() => currentSpLevel = Mathf.Min(currentSpLevel + 1, GameConst.MAX_SKILL_POINT));
        }
        _gaugeSequence.AppendCallback(() => _prevSp = currentSpLevel + _gauge[currentSpLevel].fillAmount);
    }

    /// <summary>
    /// SPゲージの減少アニメーション
    /// </summary>
    private void DecreaseSpAnim(float sp)
    {
        _gaugeSequence?.Kill();
        _gaugeSequence = DOTween.Sequence()
            .SetLink(gameObject);

        var currentSpLevel = Mathf.FloorToInt(_prevSp);
        if (_prevSp - Mathf.Floor(_prevSp) <= 0)
        {
            currentSpLevel = Mathf.Max(currentSpLevel - 1, 0);
        }
        var totalDiff = sp - (currentSpLevel + _gauge[currentSpLevel].fillAmount);

        if (_gauge[currentSpLevel].fillAmount + totalDiff > 0f)
        {
            _gaugeSequence.Append(_gauge[currentSpLevel]
                .DOFillAmount(_gauge[currentSpLevel].fillAmount + totalDiff, 0.1f));
        }
        else
        {
            var changeValue = _gauge[currentSpLevel].fillAmount;
            _gaugeSequence.Append(_gauge[currentSpLevel].DOFillAmount(0f, 0.1f));
                        
            currentSpLevel = Mathf.Max(currentSpLevel - 1, 0);
            totalDiff += changeValue;
            if (totalDiff >= 0f)
            {
                _gaugeSequence.AppendCallback(() => _prevSp = currentSpLevel + _gauge[currentSpLevel].fillAmount);
                return;
            }
            
            changeValue = Mathf.Max(_gauge[currentSpLevel].fillAmount + totalDiff, 0f);
            _gaugeSequence.Append(_gauge[currentSpLevel].DOFillAmount(Mathf.Round(changeValue * 10) / 10f, 0.1f));
            
            currentSpLevel = Mathf.Max(currentSpLevel - 1, 0);
            totalDiff += (1.0f - changeValue);
            if (totalDiff >= 0f)
            {
                _gaugeSequence.AppendCallback(() => _prevSp = currentSpLevel + _gauge[currentSpLevel].fillAmount);
                return;
            }
            
            changeValue = Mathf.Max(_gauge[currentSpLevel].fillAmount + totalDiff, 0f);
            _gaugeSequence.Append(_gauge[currentSpLevel].DOFillAmount(Mathf.Round(changeValue * 10) / 10f, 0.1f));
            
            currentSpLevel = Mathf.Max(currentSpLevel - 1, 0);
            _gaugeSequence.AppendCallback(() => _prevSp = currentSpLevel + _gauge[currentSpLevel].fillAmount);
            return;
        }
        _gaugeSequence.AppendCallback(() =>
        {
            _prevSp = currentSpLevel + _gauge[currentSpLevel].fillAmount;
        });
    }

    /// <summary>
    /// レベルに応じてフロントアイコンとテキストを切り替える
    /// </summary>
    /// <param name="spLevel"></param>
    public void UpdateSpIcon(int spLevel)
    {
        // UI切り替え
        for (int i = 0; i < _front.Length; i++)
        {
            var isActive = i == spLevel - 1;
            _front[i].gameObject.SetActive(isActive);
        }
        
        _spText.text = spLevel.ToString();
        
        UpdateSkillActivator(spLevel);
        
        // アニメーション
        _skillTextSequence?.Kill();

        var targetScale = Vector3.one * 1.3f;
        _skillTextSequence = DOTween.Sequence()
            .Append(_spText.transform.DOScale(targetScale, 0.3f).SetLoops(2, LoopType.Yoyo));
    }

    /// <summary>
    /// スキル発動条件に応じてUIを非活性にする
    /// </summary>
    private void UpdateSkillActivator(int spLevel)
    {
        var isActive = spLevel >= 1;
        _skillActivator.raycastTarget = isActive;
        _skillActivator.transform.GetChild(0).GetComponent<Image>().color =
            isActive ? Color.white: _enableColor;

        isActive = spLevel == 3;
        _specialSkillActivator.raycastTarget = isActive;
        _specialSkillActivator.transform.GetChild(0).GetComponent<Image>().color =
            isActive ? Color.white : _enableColor;

    }
}
