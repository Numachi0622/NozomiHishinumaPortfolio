using UnityEngine;
using UniRx;
using System;
using UnityEngine.SceneManagement;
using Urg;

public class CalibrationView : MonoBehaviour
{
    private ReactiveProperty<float> cornerRight = new ReactiveProperty<float>();
    private ReactiveProperty<float> cornerLeft = new ReactiveProperty<float>();
    private ReactiveProperty<float> cornerUp = new ReactiveProperty<float>();
    private ReactiveProperty<float> cornerDown = new ReactiveProperty<float>();
    
    public IObservable<float> CornerRight => cornerRight;
    public IObservable<float> CornerLeft => cornerLeft;
    public IObservable<float> CornerUp => cornerUp;
    public IObservable<float> CornerDown => cornerDown;
    
    private float currentCornerRight, currentCornerLeft, currentCornerUp, currentCornerDown;
    private Rect rect = new Rect(0f, 0f, 300f, 300f);
    private const string Title = "Calibration";
    private const float MinValue = -2f, MaxValue = 2f;
    private DebugRenderer debugRenderer;

    public Action OnClickSaveBtn;

    private void OnGUI()
    {
        rect = GUI.Window(0, rect, (id) =>
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Up Value");
            GUILayout.FlexibleSpace();
            cornerUp.Value = GUILayout.HorizontalSlider(cornerUp.Value, MinValue, MaxValue, GUILayout.Width(200f));
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Right Value");
            GUILayout.FlexibleSpace();
            cornerRight.Value = GUILayout.HorizontalSlider(cornerRight.Value, MinValue, MaxValue, GUILayout.Width(200f));
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Left Value");
            GUILayout.FlexibleSpace();
            cornerLeft.Value = GUILayout.HorizontalSlider(cornerLeft.Value, MinValue, MaxValue, GUILayout.Width(200f));
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Down Value");
            GUILayout.FlexibleSpace();
            cornerDown.Value = GUILayout.HorizontalSlider(cornerDown.Value, MinValue, MaxValue, GUILayout.Width(200f));
            GUILayout.EndHorizontal();

            if (debugRenderer != null)
            {
                GUILayout.Label("CornerUp : " + debugRenderer.CornerUp);
                GUILayout.Label("CornerRight : " + debugRenderer.CornerRight);
                GUILayout.Label("CornerLeft : " + debugRenderer.CornerLeft);
                GUILayout.Label("CornerDown : " + debugRenderer.CornerDown);
            }

            if (GUILayout.Button("Save"))
            {
                OnClickSaveBtn?.Invoke();
            }

            if (GUILayout.Button("Gomplete"))
            {
                SceneManager.LoadScene("Game");
            }
        }, Title);
    }

    public void SetCurrentRight(float right)
    {
        this.currentCornerRight = right;
    }

    public void SetCurrentLeft(float left)
    {
        this.currentCornerLeft = left;
    }

    public void SetCurrentUp(float up)
    {
        this.currentCornerUp = up;
    }

    public void SetCurrentDown(float down)
    {
        this.currentCornerDown = down;
    }

    public void SetDebugRenderer(DebugRenderer debugRenderer)
    {
        this.debugRenderer = debugRenderer;
    }
    
}
