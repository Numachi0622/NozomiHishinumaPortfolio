using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
    public GameObject clearUI;
    public GameObject loseUI;
    public UIManager UIM;
    public PlayerManager PlayerManager;

    public string currentStage;  //現在のステージ名

    public AudioClip clearSE;
    public AudioClip loseSE;
    AudioSource audioSource;

    int progress;　//最新の進捗度
    int currentProgress;　//現在の進捗度

    private void Start()
    {
        //フレームレートを60fpsに設定
        Application.targetFrameRate = 60;

        //最新の進捗度をロード
        progress = PlayerPrefs.GetInt("PROGRESS");

        //現在の進捗度を更
        currentProgress = progress;

        clearUI.SetActive(false);
        loseUI.SetActive(false);
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        DataSave();
    }

    //クリア時のUIを表示する関数
    public void ShowClearUI(bool isClear)
    {
        clearUI.SetActive(isClear);
        UIM.GetComponent<UIManager>().ClearUIAnimation();
        audioSource.PlayOneShot(clearSE);
    }

    //ゲームオーバー時のUIを表示する関数
    public void ShowLoseUI(bool isLose)
    {
        loseUI.SetActive(isLose);
        UIM.GetComponent<UIManager>().LoseUIAnimation();
        audioSource.PlayOneShot(loseSE);
    }

    //進捗度を保存する関数
    void DataSave()
    {
        //ステージをクリアしたら
        if(PlayerManager.isClear == true)
        {
            switch (currentStage)
            {
                //初期値は1、何もしていない状態
                default:
                    progress = 1;
                    break;
                case "Tutorial01":
                    progress = 2;
                    break;
                case "Tutorial02":
                    progress = 3;
                    break;
                case "Tutorial03":
                    progress = 4;
                    break;
                case "Stage01":
                    progress = 5;
                    break;
                case "Stage02":
                    progress = 6;
                    break;
                case "Stage03":
                    progress = 7;
                    break;
                case "Stage04":
                    progress = 8;
                    break;
                case "Stage05":
                    progress = 9;
                    break;
                case "Stage06":
                    progress = 10;
                    break;
                case "Stage07":
                    progress = 11;
                    break;
                case "Stage08":
                    progress = 12;
                    break;
                case "Stage09":
                    progress = 13;
                    break;
                case "Stage10":
                    break;
            }
            if (currentProgress < progress)
            {
                //保存されていた進捗度を超えたら、新しい進捗度を保存
                PlayerPrefs.SetInt("PROGRESS", progress);
            }
        }
    }
}
