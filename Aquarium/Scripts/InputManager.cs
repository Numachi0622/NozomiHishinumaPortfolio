using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    [SerializeField] private StateManager stateManager;
    [SerializeField] private FishingRod fishingRod;
    [SerializeField] private UnityEvent gameStartEvent;
    [SerializeField] private UnityEvent rotateEvent;
    [SerializeField] private UnityEvent fishSelectEvent;
    private bool isRodEnter => Input.GetKeyDown(KeyCode.UpArrow);
    private bool isRotate => Input.GetKeyDown(KeyCode.W);
    private bool isRotatable => stateManager.State == State.Hit && fishingRod.isOnWater;
    private bool isSelectable => stateManager.State == State.Result;

    public bool isRodExit => Input.GetKeyUp(KeyCode.UpArrow);

    private void Update()
    {
        if (isRodEnter && stateManager.State == State.Title)
        {
            stateManager.GoToWaitState();
            gameStartEvent?.Invoke();
        }
        else if(isRotatable && isRotate)
        {
            rotateEvent?.Invoke();
        }
        else if(isSelectable && isRotate)
        {
            fishSelectEvent?.Invoke();
        }
    }
}
