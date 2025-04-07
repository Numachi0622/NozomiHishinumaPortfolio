using System.Collections;
using System.Collections.Generic;
using Interface;
using UnityEngine;

public class EnemyDamage : IDamagable
{
    /// <summary>
    /// 敵のHPモデル
    /// </summary>
    private HitPointModel _hPModel;
    
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="hitPointModel"></param>
    public EnemyDamage(HitPointModel hitPointModel)
    {
        _hPModel = hitPointModel;
    }
    
    /// <summary>
    /// ダメージ処理
    /// </summary>
    /// <param name="damage">ダメージ量</param>
    /// <exception cref="NotImplementedException"></exception>
    public void Damage(int damage)
    {
        _hPModel.Decrease(damage);
    }
}
