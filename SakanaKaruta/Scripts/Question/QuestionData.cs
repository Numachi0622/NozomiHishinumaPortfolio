using System.Collections.Generic;

public static class QuestionData
{
    public static List<FishData> QuestionFishData;

    public static void Init(List<FishData> data)
    {
        QuestionFishData = new List<FishData>(data);
    }

    public static FishData LastPlace()
    {
        int lastIdx = QuestionFishData.Count - 1;
        return QuestionFishData[lastIdx];
    }
}
