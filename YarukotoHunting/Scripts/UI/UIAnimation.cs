using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UIAnimation : MonoBehaviour
{
    [SerializeField] private CameraController camController;

    [SerializeField] private SceneTransition sceneTransition;
    [SerializeField] private Transform compMessageTf;
    [SerializeField] private Text compMessageText;
    [SerializeField] private CanvasGroup compTextCG;
    [SerializeField] private GameObject[] compPerticles;

    [SerializeField] private RectTransform[] rankUpTextsTf;
    [SerializeField] private CanvasGroup rankUpTextsCG;
    private WaitForSeconds fallDownDelay = new WaitForSeconds(0.3f);
    private WaitForSeconds fadeDelay = new WaitForSeconds(1.5f);

    [SerializeField] private Text gameOverText;
    [SerializeField] private CanvasGroup gameOverCG;
    [SerializeField] private CanvasGroup continueButtonCG;

    [SerializeField] private Image[] buttonImages;

    [SerializeField] private AudioSource bgm, audioSource;
    [SerializeField] private AudioClip gameOverSE;

    [SerializeField] private GameObject[] eraseObjects;

    [SerializeField] private Transform[] toBossWinTf = new Transform[4];
    [SerializeField] private BeforeBossEvent beforeBossEvent;

    private void Start()
    {
        sceneTransition.FadeIn();
    }

    // 警告メッセージの表示アニメーション
    public void MessageAnimation(GameObject panel, GameObject message)
    {
        panel.SetActive(true);
        message.SetActive(true);
        Vector3 defaultPos = panel.transform.localPosition;
        CanvasGroup alpha = panel.GetComponent<CanvasGroup>();
        DOVirtual.DelayedCall(1.5f, () =>
        {
            alpha.DOFade(0, 1);
            panel.transform.DOLocalMoveY(640, 1).OnComplete(() =>
            {
                alpha.alpha = 1;
                panel.SetActive(false);
                message.SetActive(false);
                panel.transform.localPosition = defaultPos;
            });
        });
    }

    // タスク完了のメッセージ表示アニメーション
    public void CompleteMessageAnim(string _name, bool _isBoss = false)
    {
        string comp;
        if (_isBoss) comp = "討伐！";
        else comp = "完了！";
        foreach (GameObject obj in eraseObjects)
            obj.SetActive(false);
        compMessageTf.DOScaleX(1, 0.5f).OnComplete(() =>
        {
            foreach (GameObject obj in compPerticles)
                obj.SetActive(true);
            compMessageText.text = _name + "\n" + comp;
            compTextCG.DOFade(1, 0.5f).OnComplete(() =>
            {
                DOVirtual.DelayedCall(1f, () =>
                {
                    compTextCG.DOFade(0, 0.3f).OnComplete(() => { compMessageTf.DOScaleX(0, 0.5f); });
                });
            });
        });
    }

    // ランクアップの文字アニメーション
    public void RankUpAnim()
    {
        StartCoroutine(FallDownCoroutine());
        camController.ChangedDistanceByEvent();
    }
    IEnumerator FallDownCoroutine()
    {
        foreach (RectTransform tf in rankUpTextsTf)
        {
            yield return fallDownDelay;
            tf.DOLocalMoveY(0, 0.8f).SetEase(Ease.OutBack);
        }
        yield return fadeDelay;
        rankUpTextsCG.DOFade(0, 1f).OnComplete(() =>
        {
            foreach (RectTransform tf in rankUpTextsTf)
                tf.DOLocalMoveY(450, 0)
                .OnComplete(() =>
                {
                    rankUpTextsCG.alpha = 1;
                    beforeBossEvent.Event();
                });
        });
    }

    // ゲームオーバー時のUIアニメーション
    public void GameOverAnim()
    {
        bgm.DOFade(0, 0.2f).OnComplete(() => audioSource.PlayOneShot(gameOverSE));
        foreach (Image _img in buttonImages)
            _img.raycastTarget = false;
        foreach (GameObject obj in eraseObjects)
            obj.SetActive(false);
        gameOverCG.DOFade(0.3f, 0.5f);
        gameOverText.transform.DOLocalMoveY(168f, 1.5f)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                continueButtonCG.gameObject.SetActive(true);
                continueButtonCG.DOFade(1f, 1f)
                .OnComplete(() =>
                {
                    continueButtonCG.transform.DOScale(Vector3.one * 1.2f, 0.4f)
                    .SetLoops(-1, LoopType.Yoyo);
                });
            });
    }

    public void DisplayToBossWindow(int _bossNum)
    {
        if (_bossNum < 0 || _bossNum > 6) return;
        toBossWinTf[_bossNum - 1].gameObject.SetActive(true);
    }

    public void CloseToBossWindow(GameObject _win)
    {
        _win.SetActive(false);
    }
}
