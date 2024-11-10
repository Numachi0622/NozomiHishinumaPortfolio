using System;
using System.Collections.Generic;

[Serializable]
public class ScoreData
{
    public int Score;
    public int NumOfCorrectAns;
    public ScoreData() { }
    public ScoreData(int score, int numOfCorrectAns) 
    {
        this.Score = score;
        this.NumOfCorrectAns = numOfCorrectAns;
    }
}

[Serializable]
public class ScoreDataList
{
    public List<ScoreData> ScoreList = new List<ScoreData>();
    public ScoreDataList() { }
    public ScoreDataList(ScoreData scoreData)
    {
        ScoreList.Add(scoreData);
    }
}
