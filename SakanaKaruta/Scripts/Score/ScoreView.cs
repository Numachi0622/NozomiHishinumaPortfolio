using TMPro;
using UnityEngine;
using UniRx;
using DG.Tweening;

public class ScoreView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI increaseScoreText;
    [SerializeField] private TextMeshProUGUI decreaseScoreText;
    private Transform scoreTextTf;

    private void Awake()
    {
        scoreTextTf = scoreText.transform;
    }

    private void Start()
    {
        var scoreUi = scoreText.transform.parent;
        float startPos = 1150;
        float endPos = scoreUi.localPosition.x;
        scoreUi.localPosition = new Vector2(startPos, scoreUi.localPosition.y);

        StateManager.Instance.State.Subscribe(state =>
        {
            if (state == State.Game)
            {
                scoreUi.DOLocalMoveX(endPos, 0.75f);
            }
            else if (state == State.Result)
            {
                scoreUi.DOLocalMoveX(startPos, 0.75f);
            }
        })
        .AddTo(this);
    }

    public void UpdateScore(int score, int addValue)
    {
        if (addValue == 0)
        {
            return;
        }
        scoreText.text = score <= 0 ? "0" : score.ToString();
        var seq = DOTween.Sequence()
            .Append(scoreTextTf.DOScale(1.3f, 0.3f))
            .Append(scoreTextTf.DOScale(Vector3.one, 0.3f));
        
        string ope;
        TextMeshProUGUI addScoreText;
        if(addValue >= 0)
        {
            ope = "+";
            addScoreText = increaseScoreText;
            SoundManager.Instance.PlaySE("AddScore");
        }
        else
        {
            ope = "-";
            addScoreText = decreaseScoreText;
        }
        addScoreText.text = ope + addValue.ToString();
        var sequexce = DOTween.Sequence()
            .Append(addScoreText.DOFade(1, 0.3f))
            .AppendInterval(0.7f)
            .Append(addScoreText.DOFade(0, 0.3f));
    }
}
