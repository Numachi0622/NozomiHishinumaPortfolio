using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SkillEffectManager : MonoBehaviour
{
    [SerializeField] HandDetection handDetection;

    [SerializeField] GameInformation gameInformation;

    [SerializeField] SkillManager skillManager;

    [SerializeField] ParticleSystem rocketParticle,rightFire,leftFire;

    private MIDDLE_BOSS bossManager;
    [SerializeField] MonitorAppearance monitorAppearance;
    [SerializeField] HPGauge hpGauge;
    [SerializeField] AudioClip powerUpSE, rocketSE;

    //n秒強化時の秒数
    [SerializeField] TextMeshProUGUI timeText;

    //残り秒数
    private float reminingSeconds = 0;

    //レベル別制限時間
    private float limitSeconds;

    //レベル別ロケット残数
    private int rocketNum;

    //既にn秒強化スキルを使ったかを判定
    private bool alreadyUsed = false;

    private WaitForSeconds rocketWait = new WaitForSeconds(1);

    private AudioSource audioSource;


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        //初期状態はテキスト、パーティクルを非表示
        timeText.gameObject.SetActive(false);
        rightFire.gameObject.SetActive(false);
        leftFire.gameObject.SetActive(false);

        //レベル別制限時間を取得
        limitSeconds = skillManager.PowerTimeLimit(gameInformation.powerUpTimeLevel);

        //ロケットの残数を計算
        rocketNum = RocketNumCaluclation(gameInformation.rocketNumLevel);
    }
    private void Update()
    {
        if (handDetection.strengthenMode)
        { 
            //時間計測開始
            reminingSeconds += Time.deltaTime;

            //テキストを表示
            DisplayTimeText(handDetection.strengthenMode);

            if (limitSeconds - reminingSeconds <= 0)
            {
                //0秒になったら強化状態終了
                handDetection.strengthenMode = false;
                FinishStregnthenMode();

                //テキストを非表示
                DisplayTimeText(handDetection.strengthenMode);
            }
        }

        if (monitorAppearance.gameObject.activeSelf && monitorAppearance.BossAppear)
        {
            bossManager = GameObject.FindGameObjectWithTag("MB").GetComponent<MIDDLE_BOSS>();
            monitorAppearance.BossAppear = false;
        }
    }

    //テキストの表示/非表示
    private void DisplayTimeText(bool strMode)
    {
        timeText.gameObject.SetActive(strMode);
        timeText.text = (limitSeconds - reminingSeconds).ToString("n2");
    }

    //強化状態の処理
    public void StartStrengthenMode()
    {
        rightFire.gameObject.SetActive(true);
        rightFire.Play();
        leftFire.gameObject.SetActive(true);
        leftFire.Play();
        audioSource.PlayOneShot(powerUpSE);
    }

    //強化状態終了の処理
    private void FinishStregnthenMode()
    {
        rightFire.Stop();
        leftFire.Stop();
        //もう使えなくする
        alreadyUsed = true;
    }

    //ロケット発動
    public void RocketLaunch()
    {
        //残数が0だったら発動しない
        if (rocketNum <= 0) return;

        //発動中だったら停止させる
        if(rocketParticle.isPlaying) rocketParticle.Stop();

        //ロケット発動処理
        rocketParticle.Play();
        StartCoroutine(RocketDamageDelay());
        audioSource.PlayOneShot(rocketSE);

        //残数を減らす
        rocketNum--;
    }

    //ロケット残数を計算
    private int RocketNumCaluclation(int level)
    {
        //最大残数
        int max = 3;

        //レベル別に残数を計算
        for(int i = 1;i < max + 1; i++)
        {
            if(level == i)
            {
                rocketNum = i - 1;
            }
        }
        return rocketNum;
    }

    //使用するスキルレベルのロード
    public void SkillLevelLoad()
    {
        //レベル別制限時間を取得
        limitSeconds = skillManager.PowerTimeLimit(gameInformation.powerUpTimeLevel);

        //ロケットの残数を計算
        rocketNum = RocketNumCaluclation(gameInformation.rocketNumLevel);
    }

    //ロケットパンチのダメージを遅れて与えさせる処理
    IEnumerator RocketDamageDelay()
    {
        yield return rocketWait;
        skillManager.RDamege();
        bossManager.MiddleBossHp -= skillManager.RLevel;
        hpGauge.GaugeReduction(skillManager.RLevel);
    }
}
