using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ExperiencePointSystem : MonoBehaviour
{
    public float baseExp = 0.1f; // 基礎経験値
    private float exp; // 獲得経験値
    private int rankUpBorder = 1; // ランクアップする基準値
    private List<GameObject> expPool = new List<GameObject>();
    private int rank,maxRank = 10000;
    public int Rank { get => rank; } // 現在のランク
    private List<float> expStorehouse = new List<float>();

    [SerializeField] private Image rankGauge; // ランクアップのための経験値を貯めるゲージ
    [SerializeField] private Text rankText; // ランクを表示するText
    [SerializeField] private Transform[] expEndPoint = new Transform[2]; // EXPパーティクルの終着点
    [SerializeField] private GameObject expPrefab; // 生成するEXPパーティクルのPrefab
    [SerializeField] private GameObject getExpObj; // 経験値をゲットしたときのParticle
    [SerializeField] private Transform expParentTf; // EXPパーティクルの親のtransform
    [SerializeField] private PlayerStatus playerStatus;
    [SerializeField] private UIAnimation UIAnim;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip expSE,rankUpSE;

    private void Awake()
    {
        // fillAmount,Rankの値をロード
        rankGauge.fillAmount = PlayerPrefs.GetFloat("RankGauge", 0);
        rank = PlayerPrefs.GetInt("Rank", 1);
    }

    private void Start()
    {
        rankText.text = rank.ToString();
    }

    // 登録した時点の時刻から現在までの経過時間を計算
    public float ElapsedTimeCalculation(int[] _registeredTime)
    {
        // タスクを登録した時刻を取得
        DateTime pastTime = new DateTime(_registeredTime[0], _registeredTime[1], _registeredTime[2], _registeredTime[3], _registeredTime[4], 0);
        // 現在の時刻を取得
        DateTime currentTime = DateTime.Now;
        // 登録した時点の時刻から現在までの経過時間を計算
        TimeSpan elapsedTime = currentTime - pastTime;
        return (float)elapsedTime.TotalHours;
    }

    // 経験値をゲージに反映
    public void GetExp(float _elapsedTime)
    {
        exp = ExpCalculation(_elapsedTime);
        expStorehouse.Add(exp);
        if(rankGauge.fillAmount + ExpTotal() < rankUpBorder)
        {
            audioSource.PlayOneShot(expSE);
            if (rank >= maxRank) return;
            rankGauge.DOFillAmount(rankGauge.fillAmount + ExpTotal(), 1f).OnComplete(() =>
            {
                expStorehouse.Clear();
                //現在のfillAmountの値を保存
                PlayerPrefs.SetFloat("RankGauge", rankGauge.fillAmount);
            });
        }
        else
        {
            // ランクアップ時の経験値の超過分を保存しておく
            float excess = rankGauge.fillAmount + ExpTotal() - rankUpBorder;
            audioSource.PlayOneShot(expSE);
            rankGauge.DOFillAmount(rankUpBorder, 1f).OnComplete(() =>
            {
                RankUp();
                if (rank >= maxRank)
                {
                    expStorehouse.Clear();
                    PlayerPrefs.SetFloat("RankGauge", rankGauge.fillAmount);
                    return;
                }
                rankGauge.fillAmount = 0;
                rankGauge.DOFillAmount(excess, 1f).OnComplete(() =>
                {
                    expStorehouse.Clear();
                    //現在のfillAmountの値を保存
                    PlayerPrefs.SetFloat("RankGauge", rankGauge.fillAmount);
                });
            });
        }

    }

    // 経過時間から獲得経験値を計算
    private float ExpCalculation(float _elapsedTime)
    {
        if (_elapsedTime < 24) return baseExp * 1.5f;
        else if (_elapsedTime >= 24 && _elapsedTime < 48) return baseExp;
        else if (_elapsedTime >= 48 && _elapsedTime < 72) return baseExp * 0.75f;
        else if (_elapsedTime >= 72 && _elapsedTime < 168) return baseExp * 0.5f;
        else return baseExp * 0.1f;
    }

    // ランクの加算、保存を行う
    public void RankUp()
    {
        if (rank == maxRank) return;
        rank++;
        rankText.text = rank.ToString();
        playerStatus.StatusSettingFromRank(rank,true);
        UIAnim.RankUpAnim();
        playerStatus.Dance(2);
        PlayerPrefs.SetInt("Rank", rank);
        audioSource.PlayOneShot(rankUpSE);
    }

    // 経験値の演出オブジェクトをオブジェクトプールを用いて生成
    private GameObject GetExpObjectFromPool()
    {
        for(int i = 0; i < expPool.Count; i++)
        {
            if (!expPool[i].activeSelf) return expPool[i];
        }
        GameObject newExp = Instantiate(expPrefab,Vector3.zero,Quaternion.identity,expParentTf);
        return newExp;
    }

    // 経験値取得時の演出
    public void ExpEffect(GameObject _exp)
    {
        _exp.transform.DOMove(expEndPoint[0].position,1).OnComplete(() =>
        {
            GameObject expObj = GetExpObjectFromPool();
            if (!expObj.activeSelf) expObj.SetActive(true);
            // 最後に敵のいた位置をスクリーン座標に変換し、そこにパーティクルを移動させる
            expObj.transform.localPosition = new Vector3(650,0,0);
            // アニメーション
            expObj.transform.DOMove(expEndPoint[1].position, 1).OnComplete(() => 
            { 
                expObj.SetActive(false);
                expPool.Add(expObj);
                getExpObj.SetActive(true);
            });
        });
    }

    private float ExpTotal()
    {
        float _exp = 0;
        foreach (float e in expStorehouse)
            _exp += e;
        return _exp;
    }
}
