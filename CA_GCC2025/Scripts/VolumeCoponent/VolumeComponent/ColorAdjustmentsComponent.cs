using Interface;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace VolumeCoponent.VolumeComponent
{
    [System.Serializable]
    public struct ColorAdjustmentsComponent : IVolumeComponent
    {
        [SerializeField] private bool _isActive;
        [SerializeField] private float _postExposure;
        [SerializeField, Range(-100f, 100f)] private float _contrast;
        [SerializeField, ColorUsage(true,true)] private Color _colorFilter;
        [SerializeField, Range(-180f, 180)] private float _hueShift;
        [SerializeField, Range(-100f, 100)] private float _saturation;

        public void Update(VolumeProfile profile)
        {
            if (!profile.TryGet(out ColorAdjustments colorAdjustments)) return;

            colorAdjustments ??= profile.Add<ColorAdjustments>();
            colorAdjustments.active = _isActive;
            colorAdjustments.postExposure.Override(_postExposure);
            colorAdjustments.contrast.Override(_contrast);
            colorAdjustments.colorFilter.Override(_colorFilter);
            colorAdjustments.hueShift.Override(_hueShift);
            colorAdjustments.saturation.Override(_saturation);
        }
    }
}
