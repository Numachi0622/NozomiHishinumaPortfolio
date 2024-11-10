using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ResultView : MonoBehaviour
{
    [SerializeField] private GameObject resultUi;
    [SerializeField] private Text msgText;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text numOfCorrectText;
    [SerializeField] private Transform finishText;
    [SerializeField] private Transform[] rankListBg = new Transform[5];
    [SerializeField] private GameObject rankInTextPrefab;
    private Transform scoreBgTf;
    private Vector2 scoreBgTargetPos;

    private void Start()
    {
        scoreBgTf = scoreText.transform.parent;
        scoreBgTargetPos = scoreBgTf.localPosition;
        scoreBgTf.localPosition = new Vector2(1250f, 0f);
    }

    public void ShowResultUI(ScoreModel scoreModel, ResultModel resultModel, FishColliderModel fishColliderModel)
    {
        resultUi.SetActive(true);
        int score = scoreModel.Score.Value;
        int numOfCorrectAns = scoreModel.NumOfCorrectAns;
        scoreText.text = score + "でした";
        numOfCorrectText.text = "計" + numOfCorrectAns + "問正解で";
        var sequence = DOTween.Sequence()
            .Append(finishText.DOLocalMoveY(120f, 0.75f).SetEase(Ease.OutBack)
            .OnStart(() => SoundManager.Instance.PlaySE("GameFinish")))
            .AppendCallback(() =>
            {
                // 魚の逃げる処理
                foreach(Collider2D col in fishColliderModel.FishColliders)
                {
                    col.GetComponent<FishMove>().ResultEscape();
                }
            })
            .AppendInterval(2f)
            .Append(finishText.GetComponent<CanvasGroup>().DOFade(0,0.5f))
            .Append(scoreBgTf.DOLocalMove(scoreBgTargetPos, 1f).SetEase(Ease.OutCubic)
            .OnStart(() => SoundManager.Instance.PlaySE("ShowCard")))
            .Append(msgText.DOFade(1f, 1f).SetDelay(0.5f))
            .Append(numOfCorrectText.DOFade(1f, 1f).SetDelay(0.5f))
            .Append(scoreText.DOFade(1f, 1f).SetDelay(0.5f))
            .AppendCallback(() =>
            {
                GameObject rankInText = null;
                for(int i = 0; i < rankListBg.Length; i++)
                {
                    float delay = 0.25f * i;
                    var rankText = rankListBg[i].GetChild(0).GetComponent<TextMeshProUGUI>();
                    rankText.text = i < resultModel.ScoreDataList.ScoreList.Count ? 
                        resultModel.ScoreDataList.ScoreList[i].Score + "スコア　" + resultModel.ScoreDataList.ScoreList[i].NumOfCorrectAns + "問正解" : "-----";
                    
                    if (score == resultModel.ScoreDataList.ScoreList[i].Score && rankInText == null)
                    {
                        rankInText = Instantiate(rankInTextPrefab);
                        rankInText.transform.SetParent(rankListBg[i]);
                        rankInText.transform.localPosition = new Vector3(320f, -40f, 0);
                    }

                    rankListBg[i].DOLocalMoveX(-500f, 1f)
                        .SetEase(Ease.OutCubic)
                        .SetDelay(delay)
                        .OnStart(() => SoundManager.Instance.PlaySE("ShowCard"));
                }
                StartCoroutine(ShowRankInText(rankInText));
            });
    }

    private IEnumerator ShowRankInText(GameObject rankInText)
    {
        yield return new WaitForSeconds(1.5f);
        if (rankInText != null)
        {
            Vector3 scale = Vector3.one * 3f;
            rankInText.transform.localScale = scale;
            rankInText.transform.DOScale(Vector3.one, 1f);
            rankInText.GetComponent<CanvasGroup>().DOFade(1f, 1f);
        }
    }
}
