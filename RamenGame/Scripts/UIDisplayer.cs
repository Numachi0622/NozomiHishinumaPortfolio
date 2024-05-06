using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

// UIを管理するクラス
public class UIDisplayer : MonoBehaviour
{
    [SerializeField] private CanvasGroup[] canvasGroups; // タイトルUIのCanvasGroupコンポーネント
    [SerializeField] private GameManager gameManager; // GameManagerクラス
    [SerializeField] private TimeCounter timeCounter; // 制限時間計測クラス
    [SerializeField] private TextMeshProUGUI countDownText; // 3,2,1のカウントダウンText
    [SerializeField] private TextMeshProUGUI stopText; // 制限時間終了時に表示するText
    [SerializeField] private TextMeshProUGUI scoreText; // スコアを表示するText
    [SerializeField] private TextMeshProUGUI cutCountText; // 切った回数を表示するtext
    [SerializeField] private Button retryButton; // リトライのButtonコンポーネント
    [SerializeField] private Cut cut; // 切った回数を算出するクラス
    [SerializeField] private ScoreManager scoreManager; // スコアを算出するクラス
    [SerializeField] private LineRenderer lineRenderer; // コントローラーのlineRendererコンポーネント（レーザー）

    [SerializeField] private AudioClip textSE,countDownSE,startSE,finishSE,scoreSE; // UIに関連したSE

    private TextMeshProUGUI customerText; // 客のセリフを表示するTextMeshProUGUIコンポーネント
    private WaitForSeconds interval = new WaitForSeconds(0.1f); // 0.05秒毎に文字を表示する
    private WaitForSeconds second = new WaitForSeconds(1f); // カウントダウン用
    public string score { get; private set; } // スコア算出後に格納

    // Customerクラスのインスタンスが生成されるとCustomerTextがセットされる
    public void SetCustomerText(TextMeshProUGUI _text)
    {
        customerText = _text;
    }

    // タイトルUIを非表示にするメソッド
    public void CloseTitleUI(Button _button)
    {
        foreach (var c in canvasGroups)
        {
            if (!c.enabled) c.enabled = true;
            c.DOFade(0, 1.5f);
        }
        lineRenderer.enabled = false;
        _button.enabled = false;
    }

    // 文字を一つずつ表示するコルーチン
    private IEnumerator DisplaySentence(string _sentence)
    {
        foreach (char c in _sentence.ToCharArray())
        {
            // 文字を一つずつ表示
            customerText.text += c;
            SoundManager.instance.PlaySE(textSE);
            yield return interval;
        }
    }

    // 客のセリフを背景と一緒に表示するメソッド
    public void DisplayCustomerUI(string _sentence, bool _isResult)
    {
        if (customerText.text != null) customerText.text = null;
        customerText.transform.parent.DOScaleX(1,1)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                StartCoroutine(DisplaySentence(_sentence));
                customerText.transform.parent.DOScaleX(0, 1)
                .SetDelay(3f)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    if (_isResult)
                    {
                        DisplayScoreText();
                        return;
                    }
                    gameManager.SetStartable();
                });
            });
    }

    // カウントダウンのコルーチン呼び出しメソッド
    public void DisplayCountDown()
    {
        if (!gameManager.gameStartable) return;
        StartCoroutine(CountDown());
    }

    // 切り始める前のカウントダウンのコルーチン
    private IEnumerator CountDown()
    {
        for (int i = 3; i >= 0; i--)
        {
            yield return second;
            if (i == 0)
            {
                countDownText.enabled = false;
                timeCounter.enabled = true;
                gameManager.GoToGameState();
                SoundManager.instance.PlaySE(startSE);
                SoundManager.instance.ResumeBGM();
            }
            else
            {
                countDownText.text = i.ToString();
                SoundManager.instance.PlaySE(countDownSE);
            }
        }
    }

    // 制限時間が来た時のテキストを表示するメソッド
    public void DisplayStopText(GameManager _gameManager)
    {
        Vector3 defaultScale = stopText.transform.localScale;
        stopText.transform.localScale = defaultScale * 1.2f;
        var sequence = DOTween.Sequence();
        sequence.Append(stopText.transform.DOScale(defaultScale, 1f).SetEase(Ease.InCirc))
            .Join(stopText.GetComponent<CanvasGroup>()?.DOFade(1, 1f))
            .OnComplete(() =>
            {
                stopText.GetComponent<CanvasGroup>()?.DOFade(0, 3f).SetDelay(3f);
                _gameManager.RamenEvent();
                score = scoreManager.Score();
                SoundManager.instance.PlaySE(finishSE);
            });
    }

    // スコアを表示するメソッド
    private void DisplayScoreText()
    {
        scoreText.text = score + "ランク";
        Vector3 defaultScale = scoreText.transform.localScale;
        scoreText.transform.localScale = defaultScale * 1.5f;
        var sequence = DOTween.Sequence();
        sequence.Append(scoreText.transform.DOScale(defaultScale, 2f).SetEase(Ease.InCirc).SetDelay(1f))
            .Join(scoreText.GetComponent<CanvasGroup>()?.DOFade(1, 2f))
            .OnComplete(() => 
            { 
                SoundManager.instance.PlaySE(scoreSE);
                DisplayRetryButton();
                DisplayCutCount();
            });
    }

    // リトライボタンを表示するメソッド
    private void DisplayRetryButton()
    {
        if(!retryButton.gameObject.activeSelf) retryButton.gameObject.SetActive(true);
        lineRenderer.enabled = true;
        retryButton.GetComponent<CanvasGroup>()?.DOFade(1,1.5f)
            .OnComplete(() => retryButton.GetComponent<CanvasGroup>().enabled = false);
    }

    // 切った回数を表示するメソッド
    private void DisplayCutCount()
    {
        if(!cutCountText.gameObject.activeSelf) cutCountText.gameObject.SetActive(true);
        cutCountText.text = cut.count.ToString() + "回切れました";
    }
}
