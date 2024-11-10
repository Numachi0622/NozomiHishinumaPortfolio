using UniRx;
using UnityEngine;

public class ScorePresenter : MonoBehaviour
{
    private ScoreView scoreView;
    private ScoreModel scoreModel;
    private ResultPresenter resultPresenter;
    [SerializeField] private FishVisibilityModel fishVisibilityModel;
    [SerializeField] private MouseClickView mouseClickView;
    [SerializeField] private TimeModel timeModel;
    [SerializeField] private ResultView resultView;
    [SerializeField] private FishColliderModel fishColliderModel;
    private int prevScore = 0;

    private void Awake()
    {
        scoreModel = GetComponent<ScoreModel>();
        scoreView = GetComponent<ScoreView>();
        fishVisibilityModel.OnCompleteSetList += Init;
        resultPresenter = new ResultPresenter(timeModel, resultView, scoreModel, mouseClickView, fishColliderModel);
    }

    private void Init()
    {
        // ƒXƒRƒA‚Ì•Ï“®‚ðŠÄŽ‹
        scoreModel.Score.Subscribe(score =>
        {
            int addValue = score - prevScore;
            this.scoreView.UpdateScore(score, addValue);
            prevScore = score;
        })
        .AddTo(this);
    }

    public void OnDestroy()
    {
        resultPresenter.Dispose();
    }
}
