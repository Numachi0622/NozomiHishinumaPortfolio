using System.Collections;
using System.Collections.Generic;
using R3;
using UnityEngine;

public class PlayerStateModel : IState<PlayerState.State>
{
    /// <summary>
    /// プレイヤーのState
    /// </summary>
    private ReactiveProperty<PlayerState.State> _state;
    public ReadOnlyReactiveProperty<PlayerState.State> State => _state;
    
    /// <summary>
    /// 現在のStateの値（公開用）
    /// </summary>
    public PlayerState.State CurrentState => _state.CurrentValue;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="state">初期化時のState</param>
    public PlayerStateModel(PlayerState.State state)
    {
        _state = new ReactiveProperty<PlayerState.State>(state);
    }

    /// <summary>
    /// 新たなStateをセット
    /// </summary>
    /// <param name="state">新しいState</param>
    public void SetState(PlayerState.State state)
    {
        if(_state.Value == state || _state.Value == PlayerState.State.Dead) return;
        _state.Value = state;
    }
}
