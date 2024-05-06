using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public GameObject LoseUI;
    public GameObject ClearUI;
    public GameObject loseText;
    public GameObject clearText;
    public CanvasGroup loseCG;
    public CanvasGroup clearCG;
    public CanvasGroup fadePanelCG;

    public GameObject image;

    public PlayerManager playerManager;

    public GameObject menuUI;
    public GameObject menu;
    public HockeyManager hockeyManager;

    AudioSource audioSource;
    public AudioClip buttonSE;

    bool showMenuKey; //メニューボタンが押されているか、押されていないかの判定

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        //初期状態はスケールを0にしておく
        loseText.transform.localScale = new Vector3(0, 0, 0);
        clearText.transform.localScale = new Vector3(0, 0, 0);

        loseCG.alpha = 0;
        clearCG.alpha = 0;

        //シーンがロードされちゃらまずワイプから始まる
        FirstWipe();

        //少し遅らせてメニューUIのボタンを表示
        menuUI.SetActive(true);
        StartCoroutine(ShowMenuButton());
    }

    //ゲームオーバーUIのアニメーション
    public void LoseUIAnimation()
    {
        loseText.transform.DOScale(new Vector2(0.5f,0.76f), 1f);
        Fade();
        StartCoroutine(LoseButtonAnimation());
    }
    IEnumerator LoseButtonAnimation()
    {
        yield return new WaitForSeconds(1f);
        loseCG.DOFade(1, 1);
    }

    //クリアUIのアニメーション
    public void ClearUIAnimation()
    {
        clearText.transform.DOScale(new Vector3(0.5f,0.76f), 1f);
        Fade();
        StartCoroutine(ClearButtonAnimation());
    }
    IEnumerator ClearButtonAnimation()
    {
        yield return new WaitForSeconds(1f);
        clearCG.DOFade(1, 1);
    }

    //フェードアニメーション
    void Fade()
    {
        fadePanelCG.DOFade(0.5f, 1);
    }


    Vector2 currentScale;
    //ボタンが押された時のアニメーション
    public void ButtonDown(GameObject button)
    {
        //ボタンのスケールを保存
        currentScale = button.gameObject.transform.localScale;

        //ボタンが押された時に元の大きさの0.9倍にする
        button.transform.DOScale(currentScale * 0.9f, 0.3f);
    }
    //ボタンから手を話した時のアニメーション
    public void ButtonUp(GameObject button)
    {
        //ボタンの大きさを元のスケールに戻す
        button.transform.DOScale(currentScale, 0.3f);
    }

    //ボタンが押された時のSE再生関数
    public void PlaySE()
    {
        audioSource.PlayOneShot(buttonSE);
    }

    /*
    public void OnCloseButton()
    {
        Debug.Log("ok");
    }
    */

    //UIを徐々に透明にするコルーチン
    IEnumerator UIFade()
    {
        yield return new WaitForSeconds(0.3f);
        LoseUI.GetComponent<CanvasGroup>().DOFade(0, 0.3f);
        ClearUI.GetComponent<CanvasGroup>().DOFade(0, 0.3f);
        menuUI.transform.GetChild(0).gameObject.GetComponent<CanvasGroup>().DOFade(0, 0.3f);
        menuUI.transform.GetChild(1).gameObject.GetComponent<CanvasGroup>().DOFade(0, 0.3f);
    }

    //最初に実行するワイプ関数
    void FirstWipe()
    {
        image.GetComponent<Image>().fillAmount = 1;
        image.GetComponent<Image>().DOFillAmount(0, 1f);
    }

    //シーン遷移前に行うワイプコルーチン
    IEnumerator Wipe()
    {
        yield return new WaitForSeconds(0.3f);
        image.GetComponent<Image>().fillOrigin = 0;
        image.GetComponent<Image>().DOFillAmount(1, 1f);
    }

    //次のシーンをロードするコルーチン
    IEnumerator NewScene(string scene)
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(scene);
    }

    //シーン遷移を行う関数
    public void SceneTransition(string scene)
    {
        StartCoroutine(UIFade());
        StartCoroutine(Wipe());
        StartCoroutine(NewScene(scene));
    }

    //メニューボタンが押されたときの関数
    public void OnMenuButton()
    {
        //メニューボタンの初期位置を保存（隠れた状態）
        float menuPosX = menu.gameObject.transform.position.x;

        if (playerManager.isLose == false && playerManager.isClear == false && playerManager.isPlay == true)
        { 
            if (showMenuKey == false)
            {
                //押されていない状態で押されたらメニューのパネルを左に移動（見えるところに表示）
                showMenuKey = true;
                menu.gameObject.transform.DOLocalMoveX(0, 0.5f);

            }
            else
            {
                //押されている状態で押されたらメニューパネルを右に移動（隠す）
                showMenuKey = false;
                menu.gameObject.transform.DOLocalMoveX(800f, 0.5f);
            }
        }
    }

    //リセットボタンが押されたらホッケーの位置をリセット
    public void OnResetButton()
    {
        hockeyManager.ResetPosition();
    }

    //メニューボタンを表示するコルーチン
    IEnumerator ShowMenuButton()
    {
        yield return new WaitForSeconds(1.5f);
        menuUI.transform.GetChild(0).gameObject.SetActive(true);
        menuUI.transform.GetChild(1).gameObject.SetActive(true);
    }
}