using System.IO;
using UnityEngine;

public class SaveSystem
{
    private static SaveSystem instance = new SaveSystem();
    public static SaveSystem Instance => instance;
    private SaveSystem() { Load(); }
    public string Path => Application.persistentDataPath + "/TaskData.json";
    //public string Path => Application.dataPath + "/TaskData.json";

    private TaskData taskData = new TaskData();
    public TaskData TaskData => taskData;

    public TaskList TaskList { get; private set; }

    // JsonUtilityを用いたセーブ関数
    public void Save()
    {
        string jsonData = JsonUtility.ToJson(TaskList,true);
        StreamWriter writer = new StreamWriter(Path, false);
        writer.WriteLine(jsonData);
        writer.Flush();
        writer.Close();
    }

    // JsonUtilityを用いたロード関数
    public void Load()
    {
        if (!File.Exists(Path))
        {
            TaskList = new TaskList();
            Save();
            return;
        }
        StreamReader reader = new StreamReader(Path);
        string jsonData = reader.ReadToEnd();
        TaskList = JsonUtility.FromJson<TaskList>(jsonData);
        reader.Close();
    }
}
