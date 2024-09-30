using UnityEngine;
using UnityEngine.SceneManagement;

public class Setting : MonoBehaviour
{
    private Rect rect = new Rect(0f, 0f, 300f, 200f);
    private const string Title = "SerialSetting";
    private const string Port = "Port";
    private string input = "";
    private string currentPort = "";

    private void OnGUI()
    {
        rect = GUI.Window(0, rect, (id) =>
        {
            GUILayout.BeginHorizontal();
            input = GUILayout.TextField(input, GUILayout.Width(100));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Save"))
            {
                currentPort = input;
                PlayerPrefs.SetString(Port,currentPort);
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Label("CurrentPort : " + currentPort);
            
            if (GUILayout.Button("Complete"))
            {
                SceneManager.LoadScene("Phase0");
            }
        }, Title);
    }
}
