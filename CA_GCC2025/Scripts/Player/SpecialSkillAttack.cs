using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using Interface;
using UnityEngine;
using Utility;
using Random = UnityEngine.Random;

public class SpecialSkillAttack : MonoBehaviour, ISkillAttackable
{
    /// <summary>
    /// スキル攻撃用のAttacker
    /// </summary>
    [SerializeField] private Attacker _attacker;

    /// <summary>
    /// プレイヤーのステータスパラメータ
    /// </summary>
    [SerializeField] private PlayerParams _playerParams;

    /// <summary>
    /// 攻撃力倍率を決めるレベル
    /// </summary>
    private int _level = 0;

    /// <summary>
    /// 高速斬撃のレベル
    /// </summary>
    private int _highSpeedSlashLevel = 2;

    /// <summary>
    /// 最後の斬撃が入るレベル
    /// </summary>
    private int _lastSlashLevel = 3;

    /// <summary>
    /// 最後の爆発攻撃が入るレベル
    /// </summary>
    private int _explosionLevel = 4;
    
    /// <summary>
    /// レベルによる攻撃力倍率
    /// </summary>
    private readonly float[] _powerMagnifications = new []
    {
        1.5f, 3f, 5f, 10f, 5f
    };

    private void OnDisable() => _level = 0;

    /// <summary>
    /// 攻撃
    /// </summary>
    public void Attack()
    {
        var power = _playerParams.BasePower.GetRandomPower;
        power = Mathf.RoundToInt(power * _powerMagnifications[_level]);
        var hitType = HitType.FromPlayerSkill;

        if (Random.value <= _playerParams.CriticalRate)
        {
            power *= _playerParams.CriticalMagnification.GetRandomMagnigication;
            hitType = HitType.FromPlayerSkillCritical;
        }
        _attacker.SetData(power, hitType);

        _attacker.EnableAttackCollider(onWaited: () =>
        {
            if (_level < _highSpeedSlashLevel)
            {
                SoundManager.Instance.PlaySe(SoundType.Slash);
            }
            if (_level < _lastSlashLevel)
            {
                var soundType = hitType == HitType.FromPlayerSkillCritical ? SoundType.CriticalHit : SoundType.SlashHit;
                SoundManager.Instance.PlaySe(soundType);
            }
            else if(_level == _lastSlashLevel)
            {
                SoundManager.Instance.PlaySe(SoundType.CriticalHit);
            }
            else if(_level == _explosionLevel && hitType == HitType.FromPlayerSkillCritical)
            {
                SoundManager.Instance.PlaySe(SoundType.ExplosionHit);
            }
        }).Forget();
    }

    public void AttackImpact(float waitTime)
    {
    }

    public void AttackEnd()
    {
    }

    public void LevelUp()
    {
        _level = Mathf.Min(_level + 1, _powerMagnifications.Length - 1);
    }

    public void LevelDown()
    {
        _level = Mathf.Max(_level - 1, 0);
    }
}
