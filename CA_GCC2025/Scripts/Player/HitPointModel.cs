using R3;
using UnityEngine;

/// <summary>
/// 汎用HPモデル
/// </summary>
public class HitPointModel
{
    /// <summary>
    /// HP
    /// </summary>
    private ReactiveProperty<int> _hp;
    public ReadOnlyReactiveProperty<int> Hp => _hp;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="hp">初期化時のHP</param>
    public HitPointModel(int hp)
    {
        _hp = new ReactiveProperty<int>(hp);
    }

    /// <summary>
    /// 新たな値をセット（上書き）
    /// </summary>
    /// <param name="hp"></param>
    public void Set(int hp)
    {
        _hp.Value = Mathf.Max(hp, 0);
    }
    
    /// <summary>
    /// HP減少
    /// </summary>
    /// <param name="damage">ダメージ量</param>
    public void Decrease(int damage)
    {
        _hp.Value = Mathf.Max(_hp.Value - damage, 0);
    }
}
