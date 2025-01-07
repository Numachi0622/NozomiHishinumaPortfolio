using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;

public class FPSView : MonoBehaviour
{
    [SerializeField] private Text fpsText;
    private void Start()
    {
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                float fps = 1.0f / Time.deltaTime;
                fpsText.text = fps.ToString("0.0");
            })
            .AddTo(this);
}
}
