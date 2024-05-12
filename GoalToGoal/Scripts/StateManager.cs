using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class StateManager : MonoBehaviour
{
    public enum STATE
    {
        START, // ゲーム開始前
        GAME, // ゲーム中
        FINISH // ゲーム終了
    }
    public STATE state;

    private char[] scoreRank = { 'S', 'A', 'B', 'C', 'E' }; // スコア

    public float currentTime; // 経過時間
    public float totalScore; // スコアの数値

    private AudioSource audio;

    [SerializeField] GameObject result;
    [SerializeField] BallController ballController;

    private void Start()
    {
        audio = GetComponent<AudioSource>();
        state = STATE.START; // 後からSTARTの変更
        currentTime = 0;
    }

    private void Update()
    {
        if(state == STATE.GAME) currentTime += Time.deltaTime;
    }

    // 可能ならゲーム中状態に遷移
    public void GoToGameState()
    {
        if (state == STATE.GAME) return;
        state = STATE.GAME;
        audio.Play();
    }

    // 可能ならゲーム終了状態に遷移
    public void GoToFinishState()
    {
        if(state == STATE.FINISH) return;
        state = STATE.FINISH;
        totalScore = ScoreCaluculation(currentTime,HitCounter());
        result.SetActive(true);
        audio.Stop();
    }

    public float ScoreCaluculation(float _time,int _count)
    {
        return _time + _count;
    }

    public char ScoreJudgement(float _totalScore)
    {
        if (_totalScore >= 20 && _totalScore < 30) return scoreRank[0];
        else if(_totalScore >= 30 && _totalScore < 40) return scoreRank[1];
        else if(_totalScore >= 40 && _totalScore < 50) return scoreRank[2];
        else if(_totalScore >= 50 && _totalScore < 60) return scoreRank[3];
        else if(_totalScore >= 60) return scoreRank[4];
        else return scoreRank[0];
    }

    public int HitCounter()
    {
        return ballController.hitCount;
    }

    public void GoToTitle()
    {
        SceneManager.LoadScene("Title");
    }
}
