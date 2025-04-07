using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sound;
using UnityEngine;
using Utility;

public class SoundManager : Singleton<SoundManager>
{
    /// <summary>
    /// BGM用AudioSource
    /// </summary>
    [SerializeField] private AudioSource _bgmSource;

    /// <summary>
    /// SE用AudioSource
    /// </summary>
    [SerializeField] private AudioSource _seSource;

    /// <summary>
    /// 続けて鳴らすSE用AurioSource
    /// </summary>
    [SerializeField] private AudioSource _continueSeSource;

    /// <summary>
    /// 音源データ
    /// </summary>
    [SerializeField] private SoundData _soundData;

    /// <summary>
    /// 音源を止めるときのフェードSequence
    /// </summary>
    private Sequence _stopSequence;

    /// <summary>
    /// SE再生
    /// </summary>
    public void PlaySe(SoundType type)
    {
        _seSource.PlayOneShot(_soundData.Sounds(type));
    }

    /// <summary>
    /// SEを鳴らし続ける
    /// </summary>
    /// <param name="type"></param>
    public void PlaySeContinue(SoundType type)
    {
        _continueSeSource.clip = _soundData.Sounds(type);
        if(_continueSeSource.isPlaying) return;
        _continueSeSource.Play();
    }

    /// <summary>
    /// SEを止める
    /// </summary>
    public void StopSeContinue()
    {
        if(!_continueSeSource.isPlaying) return;

        _stopSequence?.Kill();

        var volume = _continueSeSource.volume;
        _stopSequence = DOTween.Sequence()
            .SetLink(gameObject)
            .Append(_continueSeSource.DOFade(0f, 0.5f))
            .AppendCallback(() =>
            {
                _continueSeSource.Stop();
                _continueSeSource.volume = volume;
            });
    }
}
