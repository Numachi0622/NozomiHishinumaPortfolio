using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;

public class UIDisplay : MonoBehaviour
{
    [SerializeField] private CanvasGroup[] titleTextCG;

    [SerializeField] private Image rodGauge;
    [SerializeField] private Image fishGauge;
    [SerializeField] private GameObject hitTimeUI;
    [SerializeField] private Transform fishParent;
    [SerializeField] private Transform feedParent;
    [SerializeField] private CanvasGroup backgroundCG;
    [SerializeField] private GameObject[] getTimeEffects;
    [SerializeField] private Text fishNameText;
    [SerializeField] private CanvasGroup textCG;
    [SerializeField] private UnityEvent waitTimeEvent;
    [SerializeField] private AudioClip getSE,finishSE,scoreSE;

    [SerializeField] private UnityEvent resultEvent;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private Transform finishTextTf;
    [SerializeField] private TextMeshProUGUI[] scoreTexts;
    [SerializeField] private List<GameObject> gottenFishObjects;
    [SerializeField] private GameObject infoPrefab;
    [SerializeField] private Transform infoParent;
    [SerializeField] private Transform resultFishParent;
    [SerializeField] private Transform fishableSign;
    [SerializeField] private Transform messageTf;
    private int selectFishNum = 0;
    private bool isSelectable = false;
    private Vector3 resultFishPos = new Vector3(-400, -150, 0);
    private WaitForSeconds waitTime = new WaitForSeconds(3f);

    private void Start()
    {
        gottenFishObjects.Clear();
        //DisplayResultUI();
        DisplayTitleUI();
    }

    private void DisplayTitleUI()
    {
        titleTextCG[0].DOFade(0, 1.5f).SetLoops(-1, LoopType.Yoyo);
    }

    public void HideTitleUI()
    {
        titleTextCG[0].DOKill();
        titleTextCG[2].DOKill();
        foreach (CanvasGroup _cg in titleTextCG)
            _cg.DOFade(0,0.5f);
    }

    public void DisplayPreparationUI()
    {
        titleTextCG[2].DOFade(1,1.5f).SetLoops(-1,LoopType.Yoyo);
    }

    public void UpdateFishGauge(float _value)
    {
        fishGauge.fillAmount = _value * 0.1f;
    }

    public void UpdateRodGauge(float _value)
    {
        rodGauge.fillAmount = _value * 0.25f;
    }

    public void GaugeReset()
    {
        fishGauge.fillAmount = 0;
        rodGauge.fillAmount = 0;
    }

    public void UpdateFeedUI(int _i)
    {
        if (_i < 0) return;
        for(int i = feedParent.childCount; i > _i; i--)
        {
            feedParent.GetChild(i - 1).gameObject.SetActive(false);
        }
    }

    public void DisplayHitTimeUI(bool _isDisplay)
    {
        GaugeReset();
        hitTimeUI.SetActive(_isDisplay);
    }

    public void DisplayFishableSign()
    {
        if (fishableSign.gameObject.activeSelf) return;
        fishableSign.gameObject.SetActive(true);
        fishableSign.localScale = Vector3.one;
        fishableSign.DOScale(Vector3.one * 1.2f, 0.5f).SetLoops(-1, LoopType.Yoyo);
    }

    public void HideFishableSign()
    {
        fishableSign.gameObject.SetActive(false);
        fishableSign.DOKill();
    }

    public IEnumerator DisplayGetFishUI(Fish _fish)
    {
        SoundManager.instance.PlaySE(getSE);
        // 釣った魚のFishクラスを保存
        scoreManager.SetFishList(_fish);
        // 釣った魚の情報をセット
        SetFishInfo(_fish);
        // 釣った後、魚をUIとして表示
        fishParent.GetChild(_fish.Id).gameObject.SetActive(true);
        // リザルト時に釣った魚をUIとして表示するため、新たにInstantiateしておく
        GameObject resultGetFish = Instantiate(fishParent.GetChild(_fish.Id).gameObject,Vector3.zero,Quaternion.Euler(0,-90,0),resultFishParent);
        resultGetFish.SetActive(false);
        // 釣った魚のGameObjectを保存しておく
        gottenFishObjects.Add(resultGetFish);
        DisplayHitTimeUI(false);
        fishNameText.text = _fish.Name;
        textCG.DOFade(1, 0.5f);
        backgroundCG.DOFade(1, 0.5f);
        foreach(GameObject _effects in getTimeEffects)
            _effects.gameObject.SetActive(true);
        yield return waitTime;
        fishParent.GetChild(_fish.Id).gameObject.SetActive(false);
        waitTimeEvent?.Invoke();
        DisplayPreparationUI();
        textCG.DOFade(0, 0.5f);
        backgroundCG.DOFade(0, 0.5f);
        foreach (GameObject _effects in getTimeEffects)
            _effects.gameObject.SetActive(false);
    }

    public void DisplayResultUI()
    {
        resultEvent?.Invoke();
        foreach(CanvasGroup _c in titleTextCG)
            _c.gameObject.SetActive(false);
        scoreTexts[1].text = scoreManager.Score();
        foreach (GameObject _fish in gottenFishObjects)
            _fish.transform.localPosition = resultFishPos;
        SoundManager.instance.PlaySE(finishSE);

        var sequence = DOTween.Sequence();
        sequence.Append(finishTextTf.DOLocalMoveY(0, 1f).SetEase(Ease.OutBack))
            .Append(finishTextTf.GetComponent<CanvasGroup>().DOFade(0, 0.5f).SetDelay(2f))
            .Append(backgroundCG.DOFade(1, 1f))
            .Append(scoreTexts[0].GetComponent<CanvasGroup>().DOFade(1, 1f))
            .Append(scoreTexts[1].transform.DOScale(Vector3.one, 1f).SetEase(Ease.OutBack).SetDelay(1f))
            .Join(scoreTexts[1].GetComponent<CanvasGroup>().DOFade(1, 0.7f))
            .AppendCallback(() =>
            {
                SoundManager.instance.PlaySE(scoreSE);
                isSelectable = true;
                SelectGottenFish();
                messageTf.gameObject.SetActive(true);
                messageTf.DOScale(Vector3.one * 1.1f, 0.5f).SetLoops(-1, LoopType.Yoyo);
            });
    }

    public void SelectGottenFish()
    {
        if (!isSelectable) return;
        for(int i = 0;i < gottenFishObjects.Count; i++)
        {
            if (i == selectFishNum % gottenFishObjects.Count)
            {
                Debug.Log(gottenFishObjects[i]);
                gottenFishObjects[i].SetActive(true);
                infoParent.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                gottenFishObjects[i].SetActive(false);
                infoParent.GetChild(i).gameObject.SetActive(false);
            }
        }
        selectFishNum++;
    }

    private void SetFishInfo(Fish _fish)
    {
        GameObject info = Instantiate(infoPrefab,Vector3.zero,Quaternion.identity,infoParent);
        info.transform.localPosition = Vector3.zero;
        info.transform.GetChild(0).GetComponent<Text>().text = _fish.Name;
        info.transform.GetChild(1).GetComponent<Text>().text = _fish.Explanation;
    }
}
