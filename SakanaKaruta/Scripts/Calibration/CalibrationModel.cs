using UnityEngine;

public class CalibrationModel
{
    public void Save(float right, float left, float up, float down)
    {
        PlayerPrefs.SetFloat("CornerRight", right);
        PlayerPrefs.SetFloat("CornerLeft", left);
        PlayerPrefs.SetFloat("CornerUp", up);
        PlayerPrefs.SetFloat("CornerDown", down);
    }
}
