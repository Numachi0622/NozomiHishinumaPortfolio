using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject[] windows;

    public void OpenMenuWindow()
    {
        Time.timeScale = 0;
        menuPanel.SetActive(true);
    }

    public void CloseMenuWindow()
    {
        Time.timeScale = 1;
        menuPanel.SetActive(false);
    }

    public void OpenWindow(int _winNum)
    {
        for(int i = 0;i < windows.Length; i++)
        {
            if (i == _winNum)
                windows[i].SetActive(true);
            else
                windows[i].SetActive(false);
        }
    }

    public void CloseWindow(GameObject _win)
    {
        _win.SetActive(false);
    }
}
