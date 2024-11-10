using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Setting : MonoBehaviour
{
    private const string GAME_SCENE = "Game";
    private const string CALIBRATION_SCENE = "Calibration";
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            DOTween.KillAll();
            SceneManager.LoadScene(GAME_SCENE);
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            SceneManager.LoadScene(CALIBRATION_SCENE);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            SaveSystem.Instance.Delete();
        }
        else if (Input.GetKeyDown((KeyCode.Q)))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
