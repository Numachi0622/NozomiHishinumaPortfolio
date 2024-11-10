using System;
using UniRx;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class ResultPresenter : IDisposable
{
    private TimeModel timeModel;
    private ResultView resultView;
    private ResultModel resultModel;
    public ResultPresenter(TimeModel timeModel, ResultView resultView, ScoreModel scoreModel, MouseClickView mouseClickView, FishColliderModel fishColliderModel)
    {
        this.resultView = resultView;
        this.timeModel = timeModel;

        this.timeModel.GameTime.Subscribe(time =>
        {
            if (!this.timeModel.IsTimeUp) return;
            Observable.EveryUpdate()
                .Where(_ => !mouseClickView.IsShowGetFishUI)
                .First()
                .Subscribe(_ =>
                {
                    StateManager.Instance.SetState(State.Result);
                    this.resultModel = new ResultModel(scoreModel);
                    this.resultView.ShowResultUI(scoreModel, this.resultModel, fishColliderModel);
                });
            DOVirtual.DelayedCall(30f, () =>
            {
                StateManager.Instance.SetState(State.Title);
                SceneManager.LoadScene("Game");
            });
        });
    }

    public void Dispose()
    {
        this.Dispose();
    }
}
