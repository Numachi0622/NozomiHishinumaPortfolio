using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Utility;

public class PlayerEffectManager : Singleton<PlayerEffectManager>
{
    /// <summary>
    /// 通常攻撃用の斬撃エフェクト
    /// </summary>
    [SerializeField] private ParticleSystem[] _normalSlashEffects;

    /// <summary>
    /// スキル攻撃用の斬撃エフェクト
    /// </summary>
    [SerializeField] private ParticleSystem _skillSlashEffect;

    /// <summary>
    /// 走っているときのエフェクト
    /// </summary>
    [SerializeField] private ParticleSystem _moveEffect;
    
    /// <summary>
    /// 通常攻撃の斬撃エフェクトを再生
    /// </summary>
    /// <param name="comboCount">コンボ数カウント</param>
    public void PlayNormalSlashEffect(int comboCount)
    {
        _normalSlashEffects[comboCount].Play();
    }

    /// <summary>
    /// スキル攻撃の斬撃エフェクトを再生
    /// </summary>
    public void PlaySkillSlashEffect()
    {
        _skillSlashEffect.Play();
    }

    /// <summary>
    /// 移動エフェクトを再生
    /// </summary>
    public void PlayMoveEffect()
    {
        if(_moveEffect.isPlaying) return;
        _moveEffect.Play();
    }

    /// <summary>
    /// 移動エフェクトを停止
    /// </summary>
    public void StopMoveEffect()
    {
        if (!_moveEffect.isPlaying) return;
        _moveEffect.Stop();
    }
}
