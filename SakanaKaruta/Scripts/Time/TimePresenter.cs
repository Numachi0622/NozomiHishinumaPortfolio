using UnityEngine;
using UniRx;

public class TimePresenter : MonoBehaviour
{
    private TimeModel timeModel;
    private TimeView timeView;

    private void Awake()
    {
        timeModel = GetComponent<TimeModel>();
        timeView = GetComponent<TimeView>();
        
        timeModel.GameTime.Subscribe(time =>
        {
            timeView.UpdateTime(time);
            if (timeModel.IsTimeUp)
            {
                // ƒQ[ƒ€I—¹Œã‚Ìˆ—
            }
        });
    }
}
