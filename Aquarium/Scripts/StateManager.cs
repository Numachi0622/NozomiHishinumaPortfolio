using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.SceneManagement;

public enum State
{
    Title,
    Wait,
    Hit,
    Result
}

public class StateManager : MonoBehaviour
{
    [SerializeField] private State state;
    public State State { get => state; }
    [SerializeField] private FishingRod rod;
    [SerializeField] private float timeUntilRestart = 60f;

    private void Start()
    {
        state = State.Title;
    }

    public void GoToTitleState()
    {
        state = State.Title;
    }

    public void GoToWaitState()
    {
        state = State.Wait;
        rod.PlayEnterAnim();
        rod.HitTimeCameraWorkOut();
    }

    public void GoToHitState()
    {
        state = State.Hit;
    }

    public void GoToResultState()
    {
        state = State.Result;
        DOVirtual.DelayedCall(timeUntilRestart, () => SceneManager.LoadScene("Game"));
    }
}
