using System.Collections;
using System.Collections.Generic;
using Interface;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public struct BloomComponent : IVolumeComponent
{
    [SerializeField] private bool _isActive;
    [SerializeField, Min(0)] private float _threshold;
    [SerializeField, Min(0)] private float _intensity;
    [SerializeField, Range(0f, 1f)] private float _scatter;
    [SerializeField] private Color _tint;
    [SerializeField] private float _clamp;

    public void Update(VolumeProfile profile)
    {
        if (!profile.TryGet(out Bloom bloom)) return;

        bloom ??= profile.Add<Bloom>();
        bloom.active = _isActive;
        bloom.threshold.Override(_threshold);
        bloom.intensity.Override(_intensity);
        bloom.scatter.Override(_scatter);
        bloom.tint.Override(_tint);
        //bloom.clamp.Override(_clamp);
    }
}
