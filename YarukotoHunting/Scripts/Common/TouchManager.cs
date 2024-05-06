using UnityEngine;

public class TouchManager : MonoBehaviour
{
    public int touchCount { get; private set; } // 触れている指の本数

    public bool isTouchJoystic { get; private set; } // ジョイスティックに触れている間はtrueを返す
    public bool isTouchButton { get; private set; } // ボタンに触れたらtrueを返す
    public Touch touchInput { get; private set; }

    private void Start()
    {
        isTouchJoystic = false;
        isTouchButton = false;
    }

    private void Update()
    {
        if (Input.touchCount == 0) return;
        else if(Input.touchCount == 1)
        {
            if(Input.GetTouch(0).position.x > Screen.width / 2) touchInput = Input.GetTouch(0);
        }
        else if(Input.touchCount == 2)
        {
            if (Input.GetTouch(0).position.x > Screen.width / 2) touchInput = Input.GetTouch(0);
            else if(Input.GetTouch(1).position.x > Screen.width / 2) touchInput = Input.GetTouch(1);
        }
    }

    // ジョイスティックのEventTriggerから呼び出される
    public void TouchJoystic()
    {
        isTouchJoystic = true;
    }
    public void NotTouchJoystic()
    {
        isTouchJoystic = false;
    }
    public void TouchButton(bool _isTouch)
    {
        isTouchButton = _isTouch;
    }
}
