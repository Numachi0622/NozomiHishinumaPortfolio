using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveSystem
{
    private static SaveSystem instance = new SaveSystem();
    public static SaveSystem Instance => instance;
    private ScoreData scoreData = new ScoreData();
    public ScoreData ScoreData => scoreData;
    private ScoreDataList scoreDataList;
    public ScoreDataList ScoreDataList => scoreDataList;

    private string path = Application.dataPath + "/ScoreData.json";
    public void Save()
    {
        string jsonData = JsonUtility.ToJson(scoreDataList, true);
        StreamWriter writer = new StreamWriter(path, false);
        writer.Write(jsonData);
        writer.Flush();
        writer.Close();
    }

    public void Load()
    {
        if (!File.Exists(path))
        {
            scoreDataList = new ScoreDataList();
            Save();
            return;
        }
        StreamReader reader = new StreamReader(path);
        string jsonData = reader.ReadToEnd();
        scoreDataList = JsonUtility.FromJson<ScoreDataList>(jsonData);
        reader.Close();
    }

    public void Delete()
    {
        string jsonData = JsonUtility.ToJson(new ScoreDataList(), true);
        StreamWriter writer = new StreamWriter(path, false);
        writer.Write(jsonData);
        writer.Flush();
        writer.Close();
    }
}
