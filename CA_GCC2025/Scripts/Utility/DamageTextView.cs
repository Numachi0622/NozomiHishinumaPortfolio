using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Utility;
using DG.Tweening;
using R3;
using R3.Triggers;

public class DamageTextView : Singleton<DamageTextView>
{
    /// <summary>
    /// プレイヤーが攻撃を受けたときのダメージテキスト
    /// </summary>
    [SerializeField] private GameObject _playerDamageText;

    /// <summary>
    /// 敵が攻撃を受けたときのダメージテキスト
    /// </summary>
    [SerializeField] private GameObject _enemyDamageText;

    /// <summary>
    /// クリティカル時のダメージテキスト
    /// </summary>
    [SerializeField] private GameObject _criticalDamageText;

    /// <summary>
    /// プレイヤーと敵のダメージテキストを同時に管理するDictionary
    /// </summary>
    private Dictionary<HitType, List<TextMeshProUGUI>> _damageTextPool;

    protected override void Awake()
    {
        _damageTextPool = new Dictionary<HitType, List<TextMeshProUGUI>>()
        {
            { HitType.FromEnemy, new List<TextMeshProUGUI>() },
            { HitType.FromPlayer, new List<TextMeshProUGUI>() },
            { HitType.FromPlayerCritical, new List<TextMeshProUGUI>() }
        };
        
        base.Awake();
    }

    /// <summary>
    /// ダメージテキストを生成する
    /// </summary>
    /// <param name="type">ヒットのタイプ</param>
    /// <param name="damage">ダメージ量</param>
    /// <param name="pos">生成する座標</param>
    public void Play(HitType type, int damage, Vector3 pos)
    {
        var damageText = GetTextFromPool(type);
        
        damageText.text = damage.ToString();

        MoveText(damageText);

        damageText.UpdateAsObservable()
            .Subscribe(_ =>
            {
                damageText.transform.parent.position = RectTransformUtility.WorldToScreenPoint(Camera.main, pos);
            })
            .AddTo(this);
    }

    /// <summary>
    /// オブジェクトプールからダメージテキストを取得
    /// </summary>
    /// <param name="hitType"></param>
    /// <returns>ダメージテキスト</returns>
    private TextMeshProUGUI GetTextFromPool(HitType hitType)
    {
        var type = hitType;
        if (type == HitType.FromPlayerSkillCritical) type = HitType.FromPlayerCritical;
        else if (type == HitType.FromPlayerSkill) type = HitType.FromPlayer;
        
        for(int i = 0; i < _damageTextPool[type].Count; i++)
        {
            if (!_damageTextPool[type][i].gameObject.activeSelf)
            {
                _damageTextPool[type][i].gameObject.SetActive(true);
                return _damageTextPool[type][i];
            }
        }

        var textPrefab = GetTextType(type);
        var text = Instantiate(textPrefab, transform).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        _damageTextPool[type].Add(text);
        return text;
    }

    /// <summary>
    /// テキストのポップアップアニメーション
    /// </summary>
    /// <param name="text">アニメーションさせるText</param>
    private void MoveText(TextMeshProUGUI text)
    {
        var moveDir = Quaternion.Euler(0f, 0f, Random.Range(-45f, 45f)) * Vector3.up;
        var moveRoot = text.gameObject;
        var parent = moveRoot.transform.parent.gameObject;
        DOTween.Sequence()
            .SetLink(parent)
            .Append(moveRoot.transform.DOLocalMove(moveDir * 200f, 0.3f)).SetEase(Ease.OutCubic)
            .AppendInterval(0.5f)
            .Append(text.DOFade(0f, 0.2f))
            .AppendCallback(() =>
            {
                moveRoot.transform.localPosition = Vector3.zero;
                moveRoot.SetActive(false);
                text.alpha = 1f;
            });
    }

    private GameObject GetTextType(HitType type)
    {
        switch (type)
        {
            case HitType.FromEnemy :
                return _playerDamageText;
            case HitType.FromPlayer :
                return _enemyDamageText;
            case HitType.FromPlayerCritical :
                return _criticalDamageText;
            case HitType.FromPlayerSkillCritical :
                return _criticalDamageText;
            default: 
                return _enemyDamageText;
        }
    }
}
