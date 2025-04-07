using System;
using System.Collections.Generic;
using Interface;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using VolumeCoponent.VolumeComponent;

namespace VolumeCoponent
{
    public class VolumeController : MonoBehaviour
    {
        private Volume _volume;
        private VolumeProfile _volumeProfile;
        [SerializeField] private BloomComponent _bloom;
        [SerializeField] private ColorAdjustmentsComponent _colorAdjustments;

        private void Awake()
        {
            _volume = GetComponent<Volume>();
            _volumeProfile = _volume.profile;
        }

        private void Update() => SetValue();

        public void SetValue()
        {
            _bloom.Update(_volumeProfile);
            _colorAdjustments.Update(_volumeProfile);
        }
    }
}
