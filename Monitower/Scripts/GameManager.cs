using Meta.WitAi;
using Oculus.Voice.Core.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Runtime.CompilerServices;

public class GameManager : MonoBehaviour
{
    //管理する状態
    public enum STATE
    {
        TITLE, //タイトル
        ON_THE_WAY, //道中
        MIDDLE_BOSS, //中ボス
        LAST_BOSS, //大ボス
        CLEAR, //ゲームクリア
        GAME_OVER　//ゲームオーバー
    }

    //PlayerPrefsのキー
    private string[] key = { "Coinup", "GoldEnemy", "BossTime", "RocketM", "Powerup", "PowerupTime", "RocketNum", "WeakNum", "WeakPointM" ,"NUMBER_OF_PLAYS", "PROGRESS", "TOTAL_COIN" };

    //ゲーム中に所持したコイン
    private int currentCoin;

    //総周回数、進捗度
    private int _numberOfPlays = 0, _progress = 0;

    [SerializeField] NormalMonitorManager normalMonitorManager;
    [SerializeField] SkillEffectManager skillEffectManager;
    [SerializeField] MonitorAppearance monitorAppearance;

    [SerializeField] GameClearManagaer clearManager;
    [SerializeField] GameOverManager gameOverManager;
    [SerializeField] GameInformation gameInformation;
    [SerializeField] HandDetection handDetection;
    [SerializeField] Animator animator;
    [SerializeField] GameObject movie;

    [SerializeField] ParticleSystem lightParticle;
    [SerializeField] GameObject princess;

    [SerializeField] AudioClip[] BGM;

    [SerializeField] public STATE state;

    [SerializeField] LineRenderer lineRenderer;

    private MIDDLE_BOSS bossManager;

    private Transform princessTf;

    private AudioSource audioSource;

    public bool usableSkill; //レーザー、n秒強化が使えるかの判定 <= ボス戦のみ使用可能

    // 学園祭用モードチェンジ
    [SerializeField] private bool festivalMode;
    public bool FestivalMode { get => festivalMode; }


    private void Start()
    { 
        normalMonitorManager.gameObject.SetActive(false);
        usableSkill = false;
        currentCoin = 0;
        princessTf = princess.transform;
        audioSource = GetComponent<AudioSource>();

        //進捗度、周回数をロード
        _progress = gameInformation.progress;
        _numberOfPlays = gameInformation.numberOfPlays;

        Debug.Log("所持コイン:" + gameInformation.havingTotalCoin + " 進行度:" + gameInformation.progress + " 周回数:" + gameInformation.numberOfPlays);

        SetState(STATE.TITLE);
    }

    public void SetState(STATE _state)
    {
        //switch分で状態遷移を管理
        switch (_state)
        {
            //titleの処理
            case STATE.TITLE:
                if (festivalMode) _numberOfPlays = 0;
                lineRenderer.enabled = true;
                if (_numberOfPlays == 0)
                {
                    //初めてプレイしたときだけタイトルアニメーションを再生
                    animator.enabled = true;
                    movie.SetActive(true);
                }
                else
                {
                    animator.enabled = false;
                    movie.SetActive(false);
                    TitleBGMPlay();

                }

                break;

            //道中の処理
            case STATE.ON_THE_WAY:
                lineRenderer.enabled = false;

                //NormalMonitorManagerによるモニターの生成開始
                normalMonitorManager.gameObject.SetActive(true);

                //BGMを再生 
                if (audioSource.isPlaying) audioSource.Stop();
                audioSource.clip = BGM[1];
                audioSource.Play();

                break;

            //中ボスの処理
            case STATE.MIDDLE_BOSS:
                //道中で稼いだコインを更新
                currentCoin = normalMonitorManager._currentCoin;

                //n秒強化、ロケットパンチのレベルをロード
                skillEffectManager.SkillLevelLoad();

                //HandDetectionを有効化
                handDetection.enabled = true;

                //Bossモニターの生成
                monitorAppearance.gameObject.SetActive(true);

                //BGMを再生 
                if (audioSource.isPlaying) audioSource.Stop();
                audioSource.clip = BGM[2];
                audioSource.Play();

                break;

            //大ボスの処理
            case STATE.LAST_BOSS:
                if (festivalMode) gameInformation.progress = 3;
                _progress = gameInformation.progress;

                //道中で稼いだコインを更新
                currentCoin = normalMonitorManager._currentCoin;

                //HandDetectionを有効化
                handDetection.enabled = true;

                //n秒強化、ロケットパンチのレベルをロード
                skillEffectManager.SkillLevelLoad();

                //Bossモニターの生成
                monitorAppearance.gameObject.SetActive(true);

                //BGMを再生
                if(audioSource.isPlaying) audioSource.Stop();
                audioSource.clip = BGM[2];
                audioSource.Play();

                break;

            //クリア時の処理
            case STATE.CLEAR:
                bossManager = GameObject.FindWithTag("MB").GetComponent<MIDDLE_BOSS>();

                usableSkill = false;

                //獲得コインの更新
                currentCoin += bossManager.CoinGet(_progress);
                Debug.Log("獲得コイン" + currentCoin);

                //HandDetectionの無効化
                handDetection.enabled = false;

                //獲得コインを所持コインへ
                PlayerPrefs.SetInt("TOTAL_COIN", gameInformation.havingTotalCoin + currentCoin);

                //周回数をカウント
                _numberOfPlays++;
                PlayerPrefs.SetInt("NUMBER_OF_PLAYS", _numberOfPlays);
                PlayerPrefs.Save();

                if(_progress == 3)
                {
                    //ラスボス(4体目のボス)を倒したら特殊演出
                    lightParticle.Play();
                    LastAnimation();

                    //クリアしたら初めからになる
                    foreach(string _key in key) 
                        PlayerPrefs.DeleteKey( _key );
                    PlayerPrefs.Save();
                    StartCoroutine(clearManager.SceneChange(10));
                    return;
                }
                else
                {
                    //進行度を進める
                    _progress++;
                    PlayerPrefs.SetInt("PROGRESS", _progress);
                }

                //クリアのテキストを表示
                clearManager.Coin_Text(currentCoin);

                //タイトルへシーン遷移
                StartCoroutine(clearManager.SceneChange(3));

                break;

            //ゲームオーバー時の処理
            case STATE.GAME_OVER:
                usableSkill = false;
                lineRenderer.enabled = true;

                //HandDetectionの無効化
                handDetection.enabled = false;

                //獲得コインを所持コインへ
                PlayerPrefs.SetInt("TOTAL_COIN", gameInformation.havingTotalCoin + currentCoin);

                //周回数をカウント
                _numberOfPlays++;
                PlayerPrefs.SetInt("NUMBER_OF_PLAYS", _numberOfPlays);
                PlayerPrefs.Save();

                //ゲームオーバーのUIを表示
                gameOverManager.Coin_Text(currentCoin);
                StartCoroutine(gameOverManager.ButtonDisplay());

                break;
        }
    }

    //最後のプリンセスのアニメーション
    private void LastAnimation()
    {
        Vector3 firstPos = new Vector3(0, 0, 4);
        princess.SetActive(true);
        Vector3 lastPos = new Vector3(0, 0, 0.05f);
        princessTf.localPosition = firstPos;
        princessTf.DOLocalMove(lastPos, 3)
            .OnComplete(() =>
            {
                DOVirtual.DelayedCall(1, () => clearManager.LastClearText());
            });
            
    }

    public void TitleBGMPlay()
    {
        audioSource.clip = BGM[0];
        audioSource.Play();
    }
}
