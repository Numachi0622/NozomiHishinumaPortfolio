using TMPro;
using UnityEngine;
using UniRx;
using DG.Tweening;

public class TimeView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText;

    private void Start()
    {
        var timeUi = timeText.transform.parent;
        float startPos = -1150;
        float endPos = timeUi.localPosition.x;
        timeUi.localPosition = new Vector2(startPos, timeUi.localPosition.y);

        StateManager.Instance.State.Subscribe(state =>
        {
            if (state == State.Game)
            {
                timeUi.DOLocalMoveX(endPos, 0.75f);
            }
            else if (state == State.Result)
            {
                timeUi.DOLocalMoveX(startPos, 0.75f);
            }
        })
        .AddTo(this);
    }
    public void UpdateTime(float currentTime)
    {
        float min = currentTime / 60f;
        float sec = currentTime % 60;
        timeText.text = sec < 10 ? (int)min + ":0" + (int)sec : (int)min + ":" + (int)sec;
    }
}
