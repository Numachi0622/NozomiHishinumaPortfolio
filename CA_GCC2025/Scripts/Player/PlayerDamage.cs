using System.Collections;
using System.Collections.Generic;
using Interface;
using UnityEngine;

public class PlayerDamage : IDamagable
{
    /// <summary>
    /// プレイヤーのHPモデル
    /// </summary>
    private HitPointModel _hPModel;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="hitPointModel"></param>
    public PlayerDamage(HitPointModel hitPointModel)
    {
        _hPModel = hitPointModel;
    }
    
    /// <summary>
    /// ダメージ処理
    /// </summary>
    /// <param name="damage">ダメージ量</param>
    public void Damage(int damage)
    {
        _hPModel.Decrease(damage);
    }
}
