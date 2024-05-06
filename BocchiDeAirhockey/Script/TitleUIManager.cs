using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleUIManager : MonoBehaviour
{
    public GameObject title1;
    public GameObject title2;
    public GameObject startText;
    public GameObject wipeImage;

    CanvasGroup startTextCG;

    AudioSource audioSource;
    public AudioClip startSE;

    //TapToStartが押せるかの判定（タイトルテキストアニメーションが全て終わったらtrue）
    bool tapKey = false;

    private void Start()
    {
        Application.targetFrameRate = 60;
        audioSource = GetComponent<AudioSource>(); 

        startTextCG = startText.GetComponent<CanvasGroup>();

        //タイトルテキストのスケールの初期値を0にする
        title1.transform.localScale = new Vector2(0, 0);
        title2.transform.localScale = new Vector2(0, 0);

        //テキストのアニメーション
        StartCoroutine(ShowTitle());

    }

    private void Update()
    {
        Debug.Log(tapKey);
        ToSelectScene();
    }

    //タイトルの表示アニメーションコルーチン
    IEnumerator ShowTitle()
    {
        title1.transform.DOScale(new Vector2(1f,1f),1f);
        yield return new WaitForSeconds(1f);
        title2.transform.DOScale(new Vector2(0.7f,0.7f), 1f);
        yield return new WaitForSeconds(1f);
        ShowTitleText();
    }

    //TapToStartというテキストを点滅させる関数
    void ShowTitleText()
    {
        //セレクトシーンへ遷移可能
        tapKey = true;

        startTextCG.DOFade(1, 1f)
            .SetLoops(-1,LoopType.Yoyo);
    }

    //ワイプコルーチン
    IEnumerator Wipe()
    {
        yield return new WaitForSeconds(0.3f);
        wipeImage.GetComponent<Image>().fillOrigin = 0;
        wipeImage.GetComponent<Image>().DOFillAmount(1, 1f);
    }

    //遅れてシーン遷移するコルーチン(セレクトシーンへ)
    IEnumerator NewScene()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Select");
    }

    //セレクトシーンへ遷移する関数
    void ToSelectScene()
    {
        if (tapKey == true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                audioSource.PlayOneShot(startSE);
                StartCoroutine(Wipe());
                StartCoroutine(NewScene());
            }
        }
    }
}
