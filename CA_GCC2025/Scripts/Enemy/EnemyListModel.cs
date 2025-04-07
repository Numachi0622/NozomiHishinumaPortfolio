using System;
using System.Collections.Generic;
using R3;
using UnityEngine.Rendering;

public class EnemyListModel
{
    /// <summary>
    /// 敵数
    /// </summary>
    private ReactiveProperty<int> _enemyNum;
    public ReadOnlyReactiveProperty<int> EnemyNum => _enemyNum;

    /// <summary>
    /// 敵リスト
    /// </summary>
    private List<EnemyPresenter> _enemies;
    public List<EnemyPresenter> Enemies => _enemies;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="enemyNum"></param>
    public EnemyListModel(int enemyNum)
    {
        _enemyNum = new ReactiveProperty<int>(enemyNum);
        _enemies = new List<EnemyPresenter>();
    }

    /// <summary>
    /// 敵情報を登録
    /// </summary>
    /// <param name="enemy"></param>
    public void Register(EnemyPresenter enemy)
    {
        _enemies.Add(enemy);
        _enemyNum.Value = _enemies.Count;
    }

    /// <summary>
    /// 敵を減らす
    /// </summary>
    /// <param name="enemy"></param>
    public void Decrease(EnemyPresenter enemy)
    {
        _enemies.Remove(enemy);
        _enemyNum.Value = _enemies.Count;
    }
}