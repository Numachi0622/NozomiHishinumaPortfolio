using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SelectUIManager : MonoBehaviour
{
    //13個のボタンを配列として格納
    public GameObject[] ToNextStageButton = new GameObject[13];

    //ボタンの現在のスケールを保存
    Vector2 currentScale;

    public GameObject wipeImage;

    public GameObject selectText;

    AudioSource audioSource;
    public AudioClip buttonSE;

    int progress;　//現在の進捗度

    void Start()
    {
        //現在の進捗度をロード
        progress = PlayerPrefs.GetInt("PROGRESS");
        Debug.Log(progress);


        ShowButton();
        audioSource = GetComponent<AudioSource>();
        FirstWipe();
        SelectTextAnim();
    }

    //テキストを点滅させるアニメーション
    void SelectTextAnim()
    {
        selectText.GetComponent<CanvasGroup>().DOFade(0, 1f)
            .SetLoops(-1, LoopType.Yoyo);
    }

    //ボタンが押された時の関数
    public void ButtonDown(int ObjNum)
    {
        //ボタンの現在のスケールを保存
        currentScale = ToNextStageButton[ObjNum].transform.localScale;

        //ボタンのスケールを元の大きさの0.9倍にする
        ToNextStageButton[ObjNum].transform.DOScale(currentScale * 0.9f,0.3f);
    }

    //ボタンから手が離れた時の関数
    public void ButtonUP(int ObjNum)
    {
        //ボタンのスケールを元の大きさに戻す
        ToNextStageButton[ObjNum].transform.DOScale(currentScale, 0.3f);
    }

    //進捗度によってボタンを表示する関数
    void ShowButton()
    {
        for(int i = 1;i < progress; i++)
        {
            ToNextStageButton[i].GetComponent<CanvasGroup>().alpha = 1f;
            ToNextStageButton[i].GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
    }

    //最初に実行されるワイプ関数
    void FirstWipe()
    {
        wipeImage.GetComponent<Image>().fillAmount = 1;
        wipeImage.GetComponent<Image>().DOFillAmount(0, 1f);
    }

    //最後に実行されるワイプコルーチン
    IEnumerator LastWipe()
    {
        yield return new WaitForSeconds(0.3f);
        wipeImage.GetComponent<Image>().fillOrigin = 0;
        wipeImage.GetComponent<Image>().DOFillAmount(1, 1f);
    }

    //少し遅れてシーン遷移するコルーチン
    IEnumerator NewScene(string scene)
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(scene);
    }

    //シーン遷移関数
    public void SceneTransition(string scene)
    {
        audioSource.PlayOneShot(buttonSE);
        StartCoroutine(LastWipe());
        StartCoroutine(NewScene(scene));
    }
}
