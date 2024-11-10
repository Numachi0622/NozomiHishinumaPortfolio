using UnityEngine;
using UniRx;

public class QuestionPresenter : MonoBehaviour
{
    private QuestionView questionView;
    private QuestionModel questionModel;

    private void Start()
    {
        questionView = GetComponent<QuestionView>();
        questionModel = GetComponent<QuestionModel>();
        
        questionModel.RemainingTime.Subscribe(time =>
        {
            this.questionView.UpdateFillAmount(time * 1 / questionModel.LimitTime);
            if(time <= 0)
            {
                if (StateManager.Instance.State.Value != State.Game) return;
                this.questionView.ShowHint(questionModel.FishData, false);
                InitQuestion();
            }
            else if(0 < time && time <= questionModel.LimitTime / 2)
            {
                this.questionView.ShowHint(questionModel.FishData, true);
            }
        })
        .AddTo(this);

        StateManager.Instance.State.Subscribe(state =>
        {
            if (StateManager.Instance.State.Value == State.Game)
            {
                this.questionModel.Init();
                this.questionView.ShowQuestion(this.questionModel.FishData);
            }
            else if(StateManager.Instance.State.Value == State.Result)
            {
                this.questionView.HideQuestion();
                this.questionView.HideHint();
            }
        })
        .AddTo(this);
    }

    public void InitQuestion()
    {
        var previousFishData = this.questionModel.FishData;
        this.questionModel.Init();
        this.questionView.UpdateQuestion(this.questionModel.FishData, previousFishData);
    }
}
