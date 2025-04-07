using System;
using Cysharp.Threading.Tasks;
using Interface;
using UnityEngine;
using Utility;
using Random = UnityEngine.Random;
using State = PlayerState.State;

public class PlayerAttack : MonoBehaviour, IComboAttackable
{
    /// <summary>
    /// プレイヤーのパラメータデータ
    /// </summary>
    private PlayerParams _playerParams;
    
    /// <summary>
    /// プレイヤーのAnimator
    /// </summary>
    private Animator _animator;
    
    /// <summary>
    /// プレイヤーのStateModel
    /// </summary>
    private IState<PlayerState.State> _stateModel;
    
    /// <summary>
    /// プレイヤーのAttacker
    /// </summary>
    private Attacker _attacker;

    /// <summary>
    /// 現在のコンボ数
    /// </summary>
    private int _comboCount = 0;

    /// <summary>
    /// コンボによる攻撃力倍率
    /// </summary>
    private float[] _powerMagnifications;

    /// <summary>
    /// 最後に攻撃したタイプ
    /// </summary>
    private HitType _lastAttackType;

    /// <summary>
    /// 1コンボごとの攻撃終了時の処理を格納
    /// </summary>
    public Action OnAttackEndPerCombo;

    /// <summary>
    /// コンボ中フラグ
    /// </summary>
    public bool IsComboAttacking => _comboCount > 0;

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="playerParams"></param>
    /// <param name="animator"></param>
    /// <param name="attacker"></param>
    /// <param name="stateModel"></param>
    public void Initialize(PlayerParams playerParams, Animator animator, Attacker attacker, IState<State> stateModel, SkillPointModel spModel)
    {
        _playerParams = playerParams;
        _animator = animator;
        _attacker = attacker;
        _stateModel = stateModel;
        _powerMagnifications = new []
        {
            1.1f, 1.2f, 1.3f, 1.4f, 1.5f, 1.6f, 2f
        };

        // 攻撃ヒット時のイベントを登録
        attacker.OnTriggerEnterEvent = () =>
        {
            // SP獲得
            spModel.Increase(_playerParams.IncreaseSpValue);

            // 音源再生
            if (_lastAttackType == HitType.FromPlayer)
            {
                if (_comboCount == _playerParams.MaxComboCount)
                {
                    SoundManager.Instance.PlaySe(SoundType.LastComboSlashHit);
                    return;
                }
                
                SoundManager.Instance.PlaySe(SoundType.SlashHit);
                return;
            }
            
            SoundManager.Instance.PlaySe(SoundType.CriticalHit);
        };
    }
    
    /// <summary>
    /// 攻撃
    /// </summary>
    public void Attack()
    {
        if (_comboCount >= _playerParams.MaxComboCount) return;
        
        _stateModel.SetState(_comboCount > 0 ? State.ComboAttack : State.Attack);
        _animator.SetTrigger("Attack");
    }

    /// <summary>
    /// 攻撃Colliderの有効タイミングを設定
    /// StateMachineBehaviourを継承したクラスから呼び出される
    /// </summary>
    /// <param name="waitTime">Colliderの実行タイミング（待機時間）</param>
    public void AttackImpact(float waitTime)
    {
        // 攻撃力を適用
        var power = _playerParams.BasePower.GetRandomPower;
        power = Mathf.RoundToInt(power * _powerMagnifications[_comboCount]);
        _lastAttackType = HitType.FromPlayer;
        
        // クリティカル計算
        if (Random.value <= _playerParams.CriticalRate)
        {
            _lastAttackType = HitType.FromPlayerCritical;
            power *= _playerParams.CriticalMagnification.GetRandomMagnigication;
        }
        
        _attacker.SetData(power, _lastAttackType);
        
        // コンボ加算
        ComboCount();
        
        _attacker.EnableAttackCollider(
            waitTime,
            () =>
            {
                PlayerEffectManager.Instance.PlayNormalSlashEffect(_comboCount - 1);
                SoundManager.Instance.PlaySe(SoundType.AttackVoice);
                SoundManager.Instance.PlaySe(SoundType.Slash);
            })
            .Forget();
    }

    /// <summary>
    /// 攻撃を終了する
    /// </summary>
    public void AttackEnd()
    {
        if(_stateModel.CurrentState == State.Idle) return;
        _animator.ResetTrigger("Attack");
        _comboCount = 0;
        AttackCoolTime().Forget();
    }

    /// <summary>
    /// コンボ数をカウント
    /// 攻撃Stateが切り替わったタイミングで呼ばれる
    /// </summary>
    public void ComboCount()
    {
        _comboCount = Mathf.Min(_comboCount + 1, _playerParams.MaxComboCount + 1);
    }

    /// <summary>
    /// 1コンボごとの攻撃終了処理
    /// </summary>
    public void AttackEndPerCombo()
    {
        OnAttackEndPerCombo?.Invoke();
    }

    /// <summary>
    /// 攻撃後のクールダウンを適用
    /// 攻撃終了時に呼ばれる
    /// </summary>
    private async UniTaskVoid AttackCoolTime()
    {
        _stateModel.SetState(State.AttackCoolTime);
        await UniTask.Delay(TimeSpan.FromSeconds(_playerParams.AttackCoolTime));
        _stateModel.SetState(State.Idle);
    }
}
