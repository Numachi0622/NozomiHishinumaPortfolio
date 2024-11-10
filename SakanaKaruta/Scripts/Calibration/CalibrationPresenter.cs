using UnityEngine;
using UniRx;
using Urg;

public class CalibrationPresenter : MonoBehaviour
{
    private CalibrationView calibrationView;
    private CalibrationModel calibrationModel;
    [SerializeField] private DebugRenderer debugRenderer;

    private void Awake()
    {
        calibrationView = GetComponent<CalibrationView>();
        calibrationModel = new CalibrationModel();
        
        calibrationView.SetDebugRenderer(this.debugRenderer);

        this.calibrationView.CornerRight.Subscribe(right =>
        {
            debugRenderer.InitCorners(right, 0, 0, 0);
            calibrationView.SetCurrentRight(debugRenderer.CornerRight);
        })
        .AddTo(this);

        this.calibrationView.CornerLeft.Subscribe(left =>
        {
            debugRenderer.InitCorners(0, left, 0, 0);
            calibrationView.SetCurrentLeft(debugRenderer.CornerLeft);
        })
        .AddTo(this);

        this.calibrationView.CornerUp.Subscribe(up =>
        {
            debugRenderer.InitCorners(0,0,up,0);
            calibrationView.SetCurrentUp(debugRenderer.CornerUp);
        })
        .AddTo(this);

        this.calibrationView.CornerDown.Subscribe(down =>
        {
            debugRenderer.InitCorners(0,0,0, down);
            calibrationView.SetCurrentDown(debugRenderer.CornerDown);
        })
        .AddTo(this);

        this.calibrationView.OnClickSaveBtn = () =>
        {
            float right = debugRenderer.CornerRight;
            float left = debugRenderer.CornerLeft;
            float up = debugRenderer.CornerUp;
            float down = debugRenderer.CornerDown;
            
            this.calibrationModel.Save(right, left, up, down);
        };
    }
}
