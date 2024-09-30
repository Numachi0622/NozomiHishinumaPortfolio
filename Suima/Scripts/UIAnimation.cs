using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using System.Drawing.Text;
using System;

public class UIAnimation : MonoBehaviour
{
    [SerializeField] private Animator[] resultUIAnimators = new Animator[2];
    [SerializeField] private Transform[] scoreListTfs = new Transform[7];
    [SerializeField] private Image[] blurPanelImage = new Image[2];
    [SerializeField] private Transform timeUiTf, scoreUiTf;
    [SerializeField] private CanvasGroup[] aimingCG = new CanvasGroup[2];
    [SerializeField] private DynamicUIParam dynamicUIParam;
    private VertexGradient[] timeTextGradient = new VertexGradient[2];
    private Color rankInTextColor;
    private int rank;
    private int countDownThreshold = 5;
    private bool isScoreUpdatable;
    private Tween scoreUpdateAnim;
    private Tween timeTextAnim;

    private void Start()
    {
        float startPosX = -1000;
        for(int i = 0; i < scoreListTfs.Length; i++)
        {
            scoreListTfs[i].localPosition = new Vector3(startPosX, -250 - 170 * i);
        }

        // ぼかしシェーダーの初期化
        foreach(var img in blurPanelImage){
            img.material.SetFloat("_Blur", 0);
        }

        for (int i = 0; i < timeTextGradient.Length; i++)
        {
            timeTextGradient[i] = dynamicUIParam.Gradient[i];
        }
        rankInTextColor = dynamicUIParam.Gradient[1].topLeft;
    }

    // リザルトでプレイヤーのスコアをアニメーションさせる
    public void ScoreTextAnim(TextMeshProUGUI tMPro)
    {
        Transform textTf = tMPro.transform;
        Vector3 endVec = textTf.localScale * 1.3f;
        textTf.localScale = Vector3.one;
        textTf.DOScale(endVec, 0.5f).SetLoops(2,LoopType.Yoyo);
    }

    // 時間UIアニメーション
    public void TimeTextAnim(TextMeshProUGUI tmPro)
    {
        var textTf = tmPro.transform;
        var endVec = textTf.localScale * 2f;

        if (timeTextAnim != null && timeTextAnim.IsPlaying()) return;

        var currentTime = int.Parse(tmPro.text);
        if (currentTime > countDownThreshold) return;

        textTf.localScale = Vector3.one;
        timeTextAnim = DOTween.Sequence()
            .Append(textTf.DOScale(endVec, 0.3f).SetEase(Ease.OutQuint))
            .Join(DOColorGradient(tmPro, timeTextGradient[1], 0.3f).SetEase(Ease.OutQuint))
            .Append(textTf.DOScale(Vector3.one, 0.7f).SetEase(Ease.InQuint))
            .Join(DOColorGradient(tmPro, timeTextGradient[0], 0.7f).SetEase(Ease.InQuint));

        SoundManager.Instance.PlaySE("CountDown");
    }

    // ColorGradient用カスタムアニメーションメソッド
    Tween DOColorGradient(TextMeshProUGUI tmPro, VertexGradient targetGradient, float duration)
    {
        return DOTween.Sequence()
            .Append(DOTween.To(() => tmPro.colorGradient.topLeft,
                x => tmPro.colorGradient = new VertexGradient(x, tmPro.colorGradient.topRight,
                    tmPro.colorGradient.bottomLeft, tmPro.colorGradient.bottomRight),
                targetGradient.topLeft, duration))
            .Join(DOTween.To(() => tmPro.colorGradient.topRight,
                x => tmPro.colorGradient = new VertexGradient(tmPro.colorGradient.topLeft, x,
                    tmPro.colorGradient.bottomLeft, tmPro.colorGradient.bottomRight),
                targetGradient.topRight, duration))
            .Join(DOTween.To(() => tmPro.colorGradient.bottomLeft,
                x => tmPro.colorGradient = new VertexGradient(tmPro.colorGradient.topLeft, tmPro.colorGradient.topRight,
                    x, tmPro.colorGradient.bottomRight),
                targetGradient.bottomLeft, duration))
            .Join(DOTween.To(() => tmPro.colorGradient.bottomRight,
                x => tmPro.colorGradient = new VertexGradient(tmPro.colorGradient.topLeft, tmPro.colorGradient.topRight,
                    tmPro.colorGradient.bottomLeft, x),
                targetGradient.bottomRight, duration));
    }
    

    // リザルトでスコアのランキングリストをアニメーションさせる
    public void ScoreListAnim(){
        StartCoroutine(ScoreListAnimCoroutine());
    }
    private IEnumerator ScoreListAnimCoroutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.15f);
        float endPosX = 20f;
        for(int i = 0;i < scoreListTfs.Length; i++)
        {
            scoreListTfs[i].DOLocalMoveX(endPosX, 0.5f).SetEase(Ease.OutCubic);
            SoundManager.Instance.PlaySE("ScoreList");
            yield return wait;
        }
        RankInAnimation();
    }

    // 終了のテキストをアニメーションさせる（リザルトアニメーション開始のメソッド）
    public void FinishTextAnim(Transform textTf, Transform bgTf)
    {
        textTf.parent.gameObject.SetActive(true);
        Vector3 textMaxScale = Vector3.one * 1.3f;
        textTf.localScale = textMaxScale;
        var cg = textTf.GetComponent<CanvasGroup>();
        cg.alpha = 0;
        bgTf.localScale = Vector3.zero;

        var sequence = DOTween.Sequence()
            .Append(textTf.DOScale(Vector3.one, 1f).SetEase(Ease.InCubic).OnStart(() => SoundManager.Instance.PlaySE("Finish")))
            .Join(textTf.GetComponent<CanvasGroup>().DOFade(1f, 0.75f))
            .Append(bgTf.DOScale(Vector3.one, 1f).SetEase(Ease.OutBack))
            .AppendInterval(0.75f)
            .Append(textTf.parent.DOScale(Vector3.zero, 1f).SetEase(Ease.InBack))
            .Join(scoreUiTf.DOLocalMoveX(1200f, 1f))
            .Join(timeUiTf.DOLocalMoveX(-800f, 1f))
            .JoinCallback(() =>
            {
                foreach (var cg in aimingCG)
                {
                    cg.DOFade(0f, 1f);
                }
            })
            .AppendInterval(0.5f)
            .OnComplete(() =>
            {
                foreach(var anim in resultUIAnimators)
                {
                    anim.enabled = true;
                }
            });
    }

    // チュートリアルのテキスト背景をオープン
    public void TutorialTextBgOpenAnim(Transform tf, Action endAction = null)
    {
        tf.DOScale(Vector3.one, 1f)
            .SetEase(Ease.OutBack)
            .OnComplete(() => endAction?.Invoke());
    }

    // チュートリアルのテキスト背景をクローズ
    public void TutorialTextBgCloseAnim(Transform tf, Action endAction = null)
    {
        tf.DOScale(Vector3.zero, 1f)
            .SetEase(Ease.InBack)
            .OnComplete(() => endAction?.Invoke());
    }

    // 複数ヒットのテキストアニメーション
    public void MultipleHitTextAnim(Transform tf)
    {
        float startPos = tf.localPosition.y + 50f;
        float endPos = tf.localPosition.y + 100f;
        tf.localPosition = new Vector3(tf.localPosition.x, startPos, 0);
        var canvasGroup = tf.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        var sequence = DOTween.Sequence()
            .Append(tf.DOLocalMoveY(endPos, 0.5f).SetEase(Ease.OutCubic))
            .Join(canvasGroup.DOFade(1, 0.5f))
            .AppendCallback(() =>
            {
                canvasGroup.DOFade(0f, 0.5f).OnComplete(() => Destroy(tf.gameObject));
            });
    }

    // スコア加算量のテキストアニメーション
    public void IncreaseScoreTextAnim(Transform tf)
    {
        float startPos = tf.localPosition.y + 200f;
        float endPos = tf.position.y + 250f;
        tf.localPosition = new Vector3(tf.localPosition.x, startPos, 0);
        var canvasGroup = tf.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        var sequence = DOTween.Sequence()
            .Append(tf.DOLocalMoveY(endPos, 0.5f).SetEase(Ease.OutCubic))
            .Join(canvasGroup.DOFade(1, 0.5f))
            .AppendCallback(() =>
            {
                canvasGroup.DOFade(0, 0.5f).OnComplete(() => Destroy(tf.gameObject));
            });
    }

    // rankセッター
    public void SetRank(int rank)
    {
        this.rank = rank;
    }

    // ランクインアニメーション
    public void RankInAnimation()
    {
        if (rank > scoreListTfs.Length) return;
        Vector3 endScale = scoreListTfs[rank - 1].localScale * 1.3f;
        scoreListTfs[rank - 1].SetAsLastSibling();
        scoreListTfs[rank - 1].DOScale(endScale, 1f).SetLoops(-1, LoopType.Yoyo);

        // 4位からは色を変える
        if (rank < 4) return;
        var tmPro = scoreListTfs[rank - 1].GetChild(1).GetComponent<TextMeshProUGUI>();
        tmPro.DOColor(rankInTextColor, 0.5f);
    }

    public void AddScoreAnim(TextMeshProUGUI text)
    {
        if (!isScoreUpdatable)
        {
            isScoreUpdatable = true;
            return;
        }
        if(scoreUpdateAnim != null && scoreUpdateAnim.IsPlaying())
        {
            scoreUpdateAnim.Kill();
        }
        Transform tf = text.transform;
        Vector3 defaultScale = Vector3.one;
        tf.localScale = defaultScale;
        scoreUpdateAnim = tf.DOScale(defaultScale * 1.3f, 0.2f)
                .OnStart(() => tf.localScale = defaultScale)
                .SetLoops(2, LoopType.Yoyo);
    }

    public void HighlightAnim(CanvasGroup cg, bool isShow)
    {
        if (isShow)
        {
            cg.DOFade(0.8f, 0.7f);
        }
        else
        {
            cg.DOFade(0, 0.7f);
        }
    }
}
