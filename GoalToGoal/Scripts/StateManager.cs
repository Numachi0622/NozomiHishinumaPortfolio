using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class StateManager : MonoBehaviour
{
    public enum STATE
    {
        START, // �Q�[���J�n�O
        GAME, // �Q�[����
        FINISH // �Q�[���I��
    }
    public STATE state;

    private char[] scoreRank = { 'S', 'A', 'B', 'C', 'E' }; // �X�R�A

    public float currentTime; // �o�ߎ���
    public float totalScore; // �X�R�A�̐��l

    private AudioSource audio;

    [SerializeField] GameObject result;
    [SerializeField] BallController ballController;

    private void Start()
    {
        audio = GetComponent<AudioSource>();
        state = STATE.START; // �ォ��START�̕ύX
        currentTime = 0;
    }

    private void Update()
    {
        if(state == STATE.GAME) currentTime += Time.deltaTime;
    }

    // �\�Ȃ�Q�[������ԂɑJ��
    public void GoToGameState()
    {
        if (state == STATE.GAME) return;
        state = STATE.GAME;
        audio.Play();
    }

    // �\�Ȃ�Q�[���I����ԂɑJ��
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
