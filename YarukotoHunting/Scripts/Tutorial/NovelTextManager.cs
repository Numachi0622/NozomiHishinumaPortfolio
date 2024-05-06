using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

public class NovelTextManager : MonoBehaviour
{
    private int sentenceNum; // 表示するテキスト番号（何番目か）
    private int[] eventNum = { 3, 4, 5, 6, 8, 9, 11, 12, 14, 18, 19, 21, 22, 26, 31, 39 };// イベントの発生するテキスト番号
    private bool isEnd; // 文章が全て表示されたか
    private Vector3 buttonTapSignDefaultPos; // 文字が完成したら表示する逆三角形のUIの初期座標
    private WaitForSeconds interval;
    private WaitUntil conditions;
    [SerializeField] private Text displayText;
    [SerializeField] private string[] sentence;
    [SerializeField] private Button textButton;
    [SerializeField] private TutorialManager tutorialManager;
    [SerializeField] private GameObject buttonTapSign; // 文字が完成したら表示する逆三角形のUI
    [SerializeField] private bool isTappable = false;
    [SerializeField] private UnityEvent buttonSE;

    private void Start()
    {
        buttonTapSignDefaultPos = new Vector3(0, -140, 0);
        interval = new WaitForSeconds(0.05f);
        conditions = new WaitUntil(() => isEnd);
        StartCoroutine(DisplaySentence());
    }

    private void Update()
    {
        if(displayText.text == sentence[sentenceNum])
        {
            // 文章が完成したらボタンを有効化
            isTappable = true;
            if (sentenceNum == sentence.Length - 1)
            {
                // 最後の文章ならボタンを無効化
                isTappable = false;
            }
            else if (IsEventNumber())
            {
                if (sentenceNum == 9 || sentenceNum == 22)
                    tutorialManager.ButtonEnabled(0);
                else if (sentenceNum == 23)
                    tutorialManager.ButtonEnabled(4);
                else if (sentenceNum == 13)
                    tutorialManager.ButtonEnabled(1);
                else if (sentenceNum == 15 || sentenceNum == 27)
                    tutorialManager.ButtonEnabled(2);
            }
            else isEnd = true;
        }
        else isTappable = false;
    }
    IEnumerator DisplaySentence()
    {
        isEnd = false;
        buttonTapSign.SetActive(false);
        foreach (char _sentence in sentence[sentenceNum].ToCharArray())
        {
            // 一文字ずつ表示していく
            displayText.text += _sentence;
            yield return interval;
        }
        yield return conditions;
        DisplayButtonTapSign();
    }

    // イベント後に背景ごとテキストを再表示
    public void Resume()
    {
        // 一度文字をリセット
        displayText.text = "";
        DOVirtual.DelayedCall(0.5f, () =>
        {
            // 再び文字を表示していく
            StartCoroutine(DisplaySentence());
        });
    }
    // イベント後にテキストのみを再表示
    public void ResumeText()
    {
        // 一度文字をリセット
        displayText.text = "";
        // 再び文字を表示していく
        StartCoroutine(DisplaySentence());
    }

    // 背景を押したときの処理 (ノーマル時)
    public void OnClickNextButton()
    {
        if (Input.touchCount >= 2) return;
        if (!isTappable || IsEventNumber()) return;
        tutorialManager.FingerHide();
        buttonSE?.Invoke();
        if (sentenceNum < sentence.Length - 1)
        {
            sentenceNum++;
            if (sentenceNum == eventNum[tutorialManager.eventCount] + 1)
            {
                tutorialManager.Event(eventNum[tutorialManager.eventCount]);
                return;
            }
            ResumeText();
        }
    }

    // イベントボタンを押した時の処理（イベント時）
    public void OnClickNextButton(Button _button)
    {
        if (Input.touchCount >= 2) return;
        if (!isTappable && _button == textButton) return;
        if(_button != null) buttonSE?.Invoke();
        tutorialManager.FingerHide();
        if (sentenceNum < sentence.Length - 1)
        {
            sentenceNum++;
            if (sentenceNum == eventNum[tutorialManager.eventCount] + 1)
            {
                tutorialManager.Event(eventNum[tutorialManager.eventCount]);
                return;
            }
            ResumeText();
        }
    }

    // 文字が全て表示されたら逆三角形のUIを表示/非表示にする
    public void DisplayButtonTapSign(bool _isDisplay = true)
    {
        buttonTapSign.transform.DOKill();
        buttonTapSign.transform.localPosition = buttonTapSignDefaultPos;
        if (_isDisplay)
        {
            if (!buttonTapSign.activeSelf) buttonTapSign.SetActive(true);
            tutorialManager.TextEndAnim(buttonTapSign);
            return;
        }
        if (buttonTapSign.activeSelf) buttonTapSign.SetActive(false);
    }

    // テキスト表示後にボタンを無効化にするイベント番号
    private bool IsEventNumber()
    {
        return sentenceNum == 9 || sentenceNum == 12 ||
            sentenceNum == 13 || sentenceNum == 15 ||
            sentenceNum == 22 || sentenceNum == 23 ||
            sentenceNum == 27;
    }
}

