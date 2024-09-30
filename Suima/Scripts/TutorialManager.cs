using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private TextAsset tutorialSentences;
    private List<string> sentences = new List<string>();
    private List<int> stopSentenceNumList = new List<int>();
    private int currentSentenceNum = 0;

    [SerializeField] private UIAnimation uiAnim;
    [SerializeField] private EnemyGenerator enemyGenerator;
    [SerializeField] private GameManager gameManager;

    [SerializeField] private Transform[] textBgTfs = new Transform[2];
    [SerializeField] private TextMeshProUGUI[] mainText = new TextMeshProUGUI[2];

    private int currentDeviceId = 1;
    private WaitForSeconds textSendingInterval = new WaitForSeconds(0.05f); // ?e?L?X?g???????????C???^?[?o??
    private WaitForSeconds sentenceInterval = new WaitForSeconds(1.2f); // ?e?L?X?g?????Z???e???X????C???^?[?o??
    private WaitForSeconds attackCoolTime = new WaitForSeconds(1f);
    private bool isTutorialStart = false;
    private bool isLeftTextSendingStart = false;
    private bool isRightTextSendingStart = false;
    private bool isAttackLeftEnemy = false;
    private bool isAttackRightEnemy = false;
    private bool isTutorialSkip = false;
    private Vector3[] titleCharTargetPos = new Vector3[4]
    {
        new Vector3(0, 480f, 0),
        new Vector3(0, -480f, 0),
        new Vector3(-480f, 0, 0),
        new Vector3(480f, 0, 0)
    };
    private List<Transform> mainTitleCharLists = new List<Transform>();
    
    [SerializeField] private Transform[] titleCharTfs = new Transform[8];
    [SerializeField] private CanvasGroup[] highlightCG = new CanvasGroup[2];
    [SerializeField] private Transform[] startTextTfs = new Transform[2];
    [SerializeField] private CanvasGroup[] titleCG, aimingCG = new CanvasGroup[2];
    [SerializeField] private Transform timeUiTf, scoreUiTf;
    
    [SerializeField] private GameParam gameParam;

    private void Awake()
    {
        StringReader reader = new StringReader(tutorialSentences.text);
        while (reader.Peek() != -1)
        {
            string sentence = reader.ReadLine();
            if(sentence == "")
            {
                stopSentenceNumList.Add(sentences.Count);
                continue;
            }
            sentences.Add(sentence);
        }
    }

    private void Start()
    {
        scoreUiTf.localPosition = new Vector3(1200f, -390f, 0);
        timeUiTf.localPosition = new Vector3(-800f, 0, 0);
        foreach (var cg in aimingCG)
        {
            cg.alpha = 0;
        }
        
        isTutorialSkip = gameParam.IsTutorialSkip;
        if (isTutorialSkip)
        {
            SceneManager.LoadScene("Phase1");
            SoundManager.Instance.PlayBGM("Battle");
        }
        for (int i = 0; i < textBgTfs.Length; i++)
        {
            textBgTfs[i].localScale = Vector3.zero;
        }
        SoundManager.Instance.InitBGM();
        StartTextAnim();
    }

    private void StartTextAnim()
    {
        foreach (var tf in startTextTfs)
        {
            float target = tf.localPosition.y + 50f;
            tf.DOLocalMoveY(target, 1f).SetLoops(-1, LoopType.Yoyo);
        }
    }

    public void HideStartUI(int id)
    {
        if (id != 0 && id != 1) return;

        titleCG[id]?.DOFade(0f, 0.5f);
        startTextTfs[id].GetComponent<CanvasGroup>().DOFade(0f, 0.5f);

        int offset = id == 0 ? 4 : 6;
        for (int i = 0; i < 2; i++)
        {
            titleCharTfs[i + offset].GetComponent<CanvasGroup>().DOFade(0f, 0.5f);
        }
        offset -= 4;
        float scale = 3.5f;
        var sequence = DOTween.Sequence()
            .Append(titleCharTfs[offset].DOLocalMove(titleCharTargetPos[offset], 0.3f)
                .OnStart(() => SoundManager.Instance.PlaySE("Impact"))
                .OnComplete(() => mainTitleCharLists.Add(titleCharTfs[offset])))
            .Join(titleCharTfs[offset].DOScale(Vector3.one * scale,0.3f))
            .Join(titleCharTfs[offset + 1].DOLocalMove(titleCharTargetPos[offset + 1], 0.3f).SetDelay(0.25f)
                .OnStart(() => SoundManager.Instance.PlaySE("Impact"))
                .OnComplete(() => mainTitleCharLists.Add(titleCharTfs[offset + 1])))
            .Join(titleCharTfs[offset + 1].DOScale(Vector3.one * scale, 0.3f));
    }

    public void TutorialStart()
    {
        StartCoroutine(ProceedFromWindow());
        SoundManager.Instance.PlayBGM("Battle");
    }

    private IEnumerator ProceedFromWindow()
    {
        yield return new WaitUntil(() => mainTitleCharLists.Count == 4);
        SoundManager.Instance.PlaySE("Fire");
        foreach (var tf in mainTitleCharLists)
        {
            Vector3 target = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0);
            tf.GetChild(0)?.GetComponent<ParticleSystem>().Play();
            tf.DOShakePosition(2.5f, new Vector3(1f, 1f, 0) * 10f, 20, 0, false, false)
                .OnComplete(() => tf.DOBlendableMoveBy(target.normalized * 2f, 0.5f)
                    .OnComplete(() =>
                    {
                        tf.gameObject.SetActive(false);
                        if(!isTutorialStart) isTutorialStart = true;
                    }));
        }
        yield return new WaitUntil(() => isTutorialStart);
        
        scoreUiTf.DOLocalMoveX(420f, 1f).SetEase(Ease.OutCubic);
        timeUiTf.DOLocalMoveX(0f, 1f).SetEase(Ease.OutCubic);
        foreach (var cg in aimingCG)
        {
            cg.DOFade(1f, 1f);
        }
        
        enemyGenerator.TutorialGenerate(currentDeviceId);

        uiAnim.TutorialTextBgOpenAnim(textBgTfs[currentDeviceId], () =>
        {
            SetRightTextSendingStart();
        });
        uiAnim.HighlightAnim(highlightCG[currentDeviceId], true);
        
        yield return new WaitUntil(() => isRightTextSendingStart);

        mainText[currentDeviceId].text = "";
        foreach (char c in sentences[currentSentenceNum].ToCharArray())
        {
            mainText[currentDeviceId].text += c == ' ' ? "\n" : c;
            SoundManager.Instance.PlaySE("TextSending");
            yield return textSendingInterval;
        }
        currentSentenceNum++;

        yield return new WaitUntil(() => isAttackRightEnemy);

        uiAnim.HighlightAnim(highlightCG[currentDeviceId], false);
        
        yield return attackCoolTime;
        uiAnim.TutorialTextBgCloseAnim(textBgTfs[currentDeviceId], () =>
        {
            currentDeviceId = 0;
            enemyGenerator.TutorialGenerate(currentDeviceId);
            uiAnim.TutorialTextBgOpenAnim(textBgTfs[currentDeviceId], () =>
            {
                SetLeftTextSendingStart();
            });
            uiAnim.HighlightAnim(highlightCG[currentDeviceId], true);
        });

        yield return new WaitUntil(() => isLeftTextSendingStart);

        mainText[currentDeviceId].text = "";
        foreach (char c in sentences[currentSentenceNum].ToCharArray())
        {
            mainText[currentDeviceId].text += c == ' ' ? "\n" : c;
            SoundManager.Instance.PlaySE("TextSending");
            yield return textSendingInterval;
        }
        currentSentenceNum++;

        yield return new WaitUntil(() => isAttackLeftEnemy);
        
        uiAnim.HighlightAnim(highlightCG[currentDeviceId], false);

        yield return attackCoolTime;
        for (; currentSentenceNum < sentences.Count; currentSentenceNum++)
        {
            mainText[currentDeviceId].text = "";
            foreach (char c in sentences[currentSentenceNum].ToCharArray())
            {
                mainText[currentDeviceId].text += c == ' ' ? "\n" : c;
                SoundManager.Instance.PlaySE("TextSending");
                yield return textSendingInterval;
            }
            yield return sentenceInterval;
        }
        uiAnim.TutorialTextBgCloseAnim(textBgTfs[currentDeviceId], () =>
        {
            StartCoroutine(gameManager.GoToNextPhase());
        });
    }
    
    private void SetLeftTextSendingStart()
    {
        isLeftTextSendingStart = true;
    }

    private void SetRightTextSendingStart()
    {
        isRightTextSendingStart = true;
    }

    public void SetAttackLeftEnemy()
    {
        isAttackLeftEnemy = true;
    }

    public void SetAttackRightEnemy()
    {
        isAttackRightEnemy = true;
    }
}
