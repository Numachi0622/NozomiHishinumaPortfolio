public class ResultModel
{
    private ScoreDataList scoreDataList;
    public ScoreDataList ScoreDataList => scoreDataList;
    public ResultModel(ScoreModel scoreModel)
    {
        SaveSystem.Instance.Load();
        scoreDataList = SaveSystem.Instance.ScoreDataList;
        ScoreData scoreData = new ScoreData(scoreModel.Score.Value, scoreModel.NumOfCorrectAns);
        scoreDataList.ScoreList.Add(scoreData);
        scoreDataList.ScoreList.Sort((x, y) => y.Score.CompareTo(x.Score));
        SaveSystem.Instance.Save();
    }
}
