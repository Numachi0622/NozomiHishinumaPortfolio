using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Interface;
using UnityEngine;
using Utility;
using Random = UnityEngine.Random;
using State = PlayerState.State;

public class PlayerSkillAttack : MonoBehaviour, ISkillAttackable
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
    [SerializeField] private Attacker[] _attacker;

    /// <summary>
    /// スキルによる攻撃力倍率
    /// </summary>
    private float[] _powerMagnifications;

    /// <summary>
    /// 攻撃力倍率を決定するレベル
    /// </summary>
    private int _level = 0;

    /// <summary>
    /// 最後に攻撃したタイプ
    /// </summary>
    private HitType _lastHitType;

    /// <summary>
    /// スキル終了時の処理を格納
    /// </summary>
    public Action OnCompleteSkill;

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize(PlayerParams playerParams, Animator animator, IState<State> stateModel)
    {
        _playerParams = playerParams;
        _animator = animator;
        _stateModel = stateModel;
        _powerMagnifications = new []
        {
            3f, 5f
        };
        
        // Colliderを放射状に配置
        for (int i = 1; i < _attacker.Length; i++)
        {
            _attacker[i].transform.position = Quaternion.Euler(0f, -120f + 30f * (i - 1), 0f) * Vector3.forward * 2.5f;
        }
        
        // 攻撃ヒット時のイベントを登録
        _attacker[0].OnTriggerEnterEvent = () =>
        {
            if(_lastHitType == HitType.FromPlayer)
                SoundManager.Instance.PlaySe(SoundType.LastComboSlashHit);
            else if(_lastHitType == HitType.FromPlayerCritical)
                SoundManager.Instance.PlaySe(SoundType.CriticalHit);
        };

        for (int i = 1; i < _attacker.Length; i++)
        {
            _attacker[i].OnTriggerEnterEvent = () =>
            {
                if(_lastHitType == HitType.FromPlayerCritical)
                    SoundManager.Instance.PlaySe(SoundType.ExplosionHit);
            };
        }
    }
    
    /// <summary>
    /// 攻撃
    /// </summary>
    public void Attack()
    {
        _stateModel.SetState(PlayerState.State.SKillAttack);
        _animator.SetTrigger("SkillAttack");
    }

    /// <summary>
    /// 攻撃Colliderの有効タイミングを設定
    /// </summary>
    /// <param name="waitTime"></param>
    public void AttackImpact(float waitTime)
    {
        // 最初の斬撃
        var power = _playerParams.BasePower.GetRandomPower;
        power = Mathf.RoundToInt(power * _powerMagnifications[_level]);
        _lastHitType = HitType.FromPlayer;
        
        // クリティカル計算
        if (Random.value <= _playerParams.CriticalRate)
        {
            _lastHitType = HitType.FromPlayerCritical;
            power *= _playerParams.CriticalMagnification.GetRandomMagnigication;
        }
        
        _attacker[0].SetData(power, _lastHitType);
        
        _attacker[0].EnableAttackCollider(
            waitTime, 
            () =>
            {
                SoundManager.Instance.PlaySe(SoundType.Slash);
                PlayerEffectManager.Instance.PlaySkillSlashEffect();
            })
            .Forget();
        
        // 追撃
        AttackImpactFollowUp(waitTime + 0.5f).Forget();
    }

    /// <summary>
    /// 追撃のCollider有効タイミングを設定
    /// </summary>
    /// <returns></returns>
    private async UniTaskVoid AttackImpactFollowUp(float waitTime)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(waitTime));
        
        // 追撃
        _level++;
        var interval = (1.0f - waitTime) / (_attacker.Length - 1);
        for (int i = 1; i < _attacker.Length; i++)
        {
            var power = _playerParams.BasePower.GetRandomPower;
            power = Mathf.RoundToInt(power * _powerMagnifications[_level]);
            var hitType = HitType.FromPlayer;
            if (Random.value <= _playerParams.CriticalRate)
            {
                hitType = HitType.FromPlayerCritical;
                power *= _playerParams.CriticalMagnification.GetRandomMagnigication;
            }
            _attacker[i].SetData(power, hitType);
            _attacker[i].EnableAttackCollider(interval * (i - 1), () =>
            {
                SoundManager.Instance.PlaySe(SoundType.SkillAttack);
            }).Forget();
        }

        _level = 0;
        OnCompleteSkill?.Invoke();
    }

    public void AttackEnd()
    {
    }

    public void LevelUp()
    {
        throw new System.NotImplementedException();
    }

    public void LevelDown()
    {
        throw new System.NotImplementedException();
    }
}
