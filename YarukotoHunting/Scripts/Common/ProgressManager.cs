using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    public int progress { get; private set; }

    private void Awake()
    {
        progress = PlayerPrefs.GetInt("Progress",0);
    }

    public void UpdateProgress()
    {
        progress++;
        PlayerPrefs.SetInt("Progress",progress);
    }
}
