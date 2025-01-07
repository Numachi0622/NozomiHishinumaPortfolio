using System;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class HandBrush : MonoBehaviour
{
    [SerializeField] private OVRHand _hand;
    [SerializeField] private Transform _indexTf;
    [SerializeField] private GameObject _linePrefab;
    [SerializeField] private BoidsGPU _boidsGPU;
    [SerializeField] private TrailGPU _trailGPU;
    [SerializeField] private AudioSource _audioSource;
    private LineRenderer _lineRenderer;

    private void Start()
    {
        this.UpdateAsObservable()
            .Where(_ => IsPinching)
            .Subscribe(_ =>
            {
                var writePos = _indexTf.position;
                if (_lineRenderer == null)
                {
                    _lineRenderer = Instantiate(_linePrefab, writePos, Quaternion.identity).GetComponent<LineRenderer>();
                }

                var nextPosIdx = _lineRenderer.positionCount;
                _lineRenderer.positionCount = nextPosIdx + 1;

                _lineRenderer.SetPosition(nextPosIdx, writePos);
            })
            .AddTo(this);

        this.UpdateAsObservable()
            .Select(_ => IsPinching)
            .DistinctUntilChanged()
            .Where(isPinching => !IsPinching)
            .Subscribe(_ =>
            {
                var center = Vector3.zero;
                var positions = new Vector3[_lineRenderer.positionCount];
                _lineRenderer.GetPositions(positions);
                foreach (var pos in positions)
                {
                    center += pos;
                }
                center /= positions.Length;
                
                var parent = new GameObject();
                parent.transform.position = center;
                _lineRenderer.transform.SetParent(parent.transform);

                var brushAnim = BrushAnim(parent.transform, _lineRenderer.material,2f)
                    .OnStart(() => _lineRenderer = null)
                    .OnComplete(() =>
                    {
                        AddBoid(Mathf.CeilToInt(positions.Length * 0.1f), center);
                        _audioSource.PlayOneShot(_audioSource.clip);
                    });
            })
            .AddTo(this);
    }

    public bool IsPinching
        => _hand.GetFingerIsPinching(OVRHand.HandFinger.Index) 
           && _hand.GetFingerIsPinching(OVRHand.HandFinger.Thumb);

    public void AddBoid(int addValue, Vector3 pos)
    {
        _boidsGPU.IsAddBoid = true;
        _boidsGPU.AddPosition = pos;
        _boidsGPU.AddValue = addValue;
        _trailGPU.IsAddTrail = true;
        _trailGPU.AddValue = addValue;
    }

    public Tween BrushAnim(Transform tf, Material material, float duration)
    {
        return DOTween.Sequence()
            .Append(tf.DOScale(Vector3.zero, duration))
            .Join(tf.DORotate(Vector3.forward * 360f, duration, RotateMode.FastBeyond360));
    }
}
