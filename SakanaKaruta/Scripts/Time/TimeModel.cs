using System;
using System.Collections;
using UniRx;
using UnityEngine;

public class TimeModel : MonoBehaviour 
{
    [SerializeField] private float limitTime = 180f;
    private WaitForSeconds sec = new WaitForSeconds(1f);
    private ReactiveProperty<float> gameTime = new ReactiveProperty<float>(1);
    public IObservable<float> GameTime => gameTime;
    public bool IsTimeUp => gameTime.Value <= 0;

    private void Awake()
    {
        gameTime.Value = limitTime;
    }

    private void Start()
    {
        StateManager.Instance.State.Subscribe(state =>
        {
            if (state != State.Game) return;
            StartCoroutine(TimeCount());
        })
        .AddTo(this);
    }

    private IEnumerator TimeCount()
    {
        while (!IsTimeUp)
        {
            gameTime.Value--;
            yield return sec;
        }
    }
}
