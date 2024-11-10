using UniRx;
using UnityEngine;

public class ScoreModel : MonoBehaviour
{
    private int baseScore = 100;
    private int decreaseValue = -50;
    private ReactiveProperty<int> score = new ReactiveProperty<int>(0);
    public ReactiveProperty<int> Score => score;
    private int numOfCorrectAns = 0;
    public int NumOfCorrectAns => numOfCorrectAns;

    public void AddScore(QuestionModel questionModel)
    {
        if (StateManager.Instance.State.Value != State.Game) return;
        float newScore = AddValue(questionModel);
        score.Value += (int)newScore;
        numOfCorrectAns++;
    }

    private float AddValue(QuestionModel questionModel)
    {
        float timeElapsedRate = questionModel.TimeElapsedRate;
        float value;
        if (timeElapsedRate >= 0.85f)
        {
            value = 5f;
        }
        else if (timeElapsedRate >= 0.75f)
        {
            value = 3f;
        }
        else if (timeElapsedRate >= 0.5f)
        {
            value = 2f;
        }
        else
        {
            value = (1 + (int)(questionModel.TimeElapsedRate * 10f) * 0.1f);
        }
        return baseScore * value;
    }
}
