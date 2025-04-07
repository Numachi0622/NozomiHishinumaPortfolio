using System.Collections;
using System.Collections.Generic;
using R3;
using UnityEngine;

public class EnemyStateModel : IState<EnemyState.State>
{
    /// <summary>
    /// 敵のState
    /// </summary>
    private ReactiveProperty<EnemyState.State> _state;

    public ReadOnlyReactiveProperty<EnemyState.State> State => _state;

    /// <summary>
    /// 現在のStateの値（公開用）
    /// </summary>
    public EnemyState.State CurrentState => _state.CurrentValue;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="state"></param>
    public EnemyStateModel(EnemyState.State state)
    {
        _state = new ReactiveProperty<EnemyState.State>(state);
    }
    
    /// <summary>
    /// 新たなStateをセット
    /// </summary>
    /// <param name="state">新しいState</param>
    public void SetState(EnemyState.State state)
    {
        if(_state.Value == state || _state.Value == EnemyState.State.Dead) return;
        _state.Value = state;
    }
}
