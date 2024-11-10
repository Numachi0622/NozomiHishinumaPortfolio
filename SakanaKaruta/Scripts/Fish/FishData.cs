using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class FishData
{
    private string kanjiName;
    private string kanaName;
    private int id;
    private string hint;
    public FishData(string kanjiName, string kanaName, int id, string hint)
    {
        this.kanjiName = kanjiName;
        this.kanaName = kanaName;
        this.id = id;
        this.hint = hint;
    }
    public FishData(string[] datas)
    {
        this.kanjiName = datas[0];
        this.kanaName= datas[1];
        this.id = int.Parse(datas[2]);
        this.hint = datas[3];
    }
    public string GetKanjiName => kanjiName;
    public string GetKanaName => kanaName;
    public int GetId => id;
    public string GetHint => hint;
}

public class FishDataList
{
    private List<FishData> fishDataList = new List<FishData>();
    public FishDataList()
    {
        string path = Path.Combine(Application.dataPath, "Fish_Data.csv");
        if (!File.Exists(path))
        {
            Debug.Log("ファイルなし");
            return;
        }

        string[] lines = File.ReadAllLines(path);
        foreach(string line in lines)
        {
            string[] fishDatas = line.Split(",");
            fishDataList.Add(new FishData(fishDatas));
        }
    }
    public List<FishData> GetFishDataList => fishDataList;
}

public static class ListExtensions
{
    private static System.Random rand = new System.Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rand.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}