using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private CanvasGroup testCG;

    public void FadeOut(float _fadeTime = 3f)
    {
        testCG.DOFade(1, _fadeTime).OnComplete(() =>
        {
            if (PlayerPrefs.GetInt("PlayCount", 0) > 0)
                SceneChange("Game");
            else
                SceneChange("Tutorial");
        });
    }

    public void FadeOut(string _name)
    {
        testCG.DOFade(1, 3f).OnComplete(() => SceneChange(_name));
    }

    public void FadeIn(float _fadeTime = 3f)
    {
        testCG.alpha = 1;
        testCG.DOFade(0,_fadeTime);
    }

    public void SceneChange(string _name)
    {
        if (Time.timeScale == 0) Time.timeScale = 1;
        SceneManager.LoadScene(_name);
    }

    public void SceneChangeAndDataReset()
    {
        PlayerPrefs.DeleteAll();
        // タスクをリストから消去
        SaveSystem.Instance.TaskList.taskList.Clear();
        SaveSystem.Instance.Save();
        SceneChange("Tutorial");
    }
}
