using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class QuestionView : MonoBehaviour
{
    [SerializeField] private Image questionTimerImg;
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private TextMeshProUGUI previousQuestionText;
    [SerializeField] private TextMeshProUGUI[] hintTexts;
    private Transform questionTf;
    private Transform previousQuestionTf;
    private bool isHintTime = false;

    private void Start()
    {
        questionTf = questionText.transform.parent.parent;
        previousQuestionTf = previousQuestionText.transform.parent;
        questionTf.gameObject.SetActive(false);
    }

    public void UpdateFillAmount(float value)
    {
        questionTimerImg.fillAmount = value;
    }

    public void UpdateQuestion(FishData fishData, FishData previousData)
    {
        var startPos = previousQuestionText.transform.parent.localPosition;
        questionText.text = fishData.GetKanjiName;

        var sequence = DOTween.Sequence()
            .Append(previousQuestionText.transform.parent.DOLocalMove(new Vector2(1100f, -100f), 0.7f))
                .OnStart(() =>
                 {
                     SoundManager.Instance.PlaySE("TurnOverCard");
                     previousQuestionText.text = previousData.GetKanjiName;
                     previousQuestionTf.gameObject.SetActive(true);
                     questionTf.localScale *= 0.7f;
                 })
                .OnComplete(() =>
                {
                    previousQuestionTf.gameObject.SetActive(false);
                    previousQuestionTf.localPosition = startPos;
                })
            .Join(questionTf.DOScale(new Vector2(-1, 1), 1f));
    }

    public void ShowQuestion(FishData fishData)
    {
        SoundManager.Instance.PlaySE("ShowCard");
        var startPos = questionTf.localPosition;
        questionTf.localPosition = new Vector2(0, -800f);
        questionTf.DOLocalMove(startPos, 0.75f)
            .OnStart(() =>
            {
                questionTf.gameObject.SetActive(true);
                questionText.text = fishData.GetKanjiName;
            });
    }

    public void HideQuestion()
    {
        questionTf.DOLocalMove(new Vector2(0, -800f), 0.75f);
    }

    public void HideHint()
    {
        var hintTf = hintTexts[0].transform.parent;
        hintTf.DOLocalMove(new Vector2(740f, -800f), 0.75f);
    }

    public void ShowHint(FishData fishData, bool isShow)
    {
        var endPos = new Vector2(740f, -343f);
        var startPos = new Vector2(740f, -800f);
        var textTf = hintTexts[0].transform.parent;
        if (isShow)
        {
            if (isHintTime) return;
            isHintTime = true;
            int textIdx = 0;
            for(int i = 0; i < fishData.GetHint.Length; i++)
            {
                char c = fishData.GetHint[i];
                if (c == ' ')
                {
                    textIdx++;
                    continue;
                }
                hintTexts[textIdx].text += c;
            }
            textTf.DOLocalMove(endPos, 0.75f);
            SoundManager.Instance.PlaySE("ShowCard");
        }
        else
        {
            if (!isHintTime) return;
            isHintTime = false;
            textTf.DOLocalMove(startPos, 0.75f)
                .OnComplete(() =>
                {
                    foreach(var text in hintTexts)
                    {
                        text.text = "";
                    }
                });
        }
    }
}
