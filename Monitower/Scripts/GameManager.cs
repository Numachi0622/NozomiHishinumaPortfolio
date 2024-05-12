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
    //�Ǘ�������
    public enum STATE
    {
        TITLE, //�^�C�g��
        ON_THE_WAY, //����
        MIDDLE_BOSS, //���{�X
        LAST_BOSS, //��{�X
        CLEAR, //�Q�[���N���A
        GAME_OVER�@//�Q�[���I�[�o�[
    }

    //PlayerPrefs�̃L�[
    private string[] key = { "Coinup", "GoldEnemy", "BossTime", "RocketM", "Powerup", "PowerupTime", "RocketNum", "WeakNum", "WeakPointM" ,"NUMBER_OF_PLAYS", "PROGRESS", "TOTAL_COIN" };

    //�Q�[�����ɏ��������R�C��
    private int currentCoin;

    //�����񐔁A�i���x
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

    public bool usableSkill; //���[�U�[�An�b�������g���邩�̔��� <= �{�X��̂ݎg�p�\

    // �w���՗p���[�h�`�F���W
    [SerializeField] private bool festivalMode;
    public bool FestivalMode { get => festivalMode; }


    private void Start()
    { 
        normalMonitorManager.gameObject.SetActive(false);
        usableSkill = false;
        currentCoin = 0;
        princessTf = princess.transform;
        audioSource = GetComponent<AudioSource>();

        //�i���x�A���񐔂����[�h
        _progress = gameInformation.progress;
        _numberOfPlays = gameInformation.numberOfPlays;

        Debug.Log("�����R�C��:" + gameInformation.havingTotalCoin + " �i�s�x:" + gameInformation.progress + " ����:" + gameInformation.numberOfPlays);

        SetState(STATE.TITLE);
    }

    public void SetState(STATE _state)
    {
        //switch���ŏ�ԑJ�ڂ��Ǘ�
        switch (_state)
        {
            //title�̏���
            case STATE.TITLE:
                if (festivalMode) _numberOfPlays = 0;
                lineRenderer.enabled = true;
                if (_numberOfPlays == 0)
                {
                    //���߂ăv���C�����Ƃ������^�C�g���A�j���[�V�������Đ�
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

            //�����̏���
            case STATE.ON_THE_WAY:
                lineRenderer.enabled = false;

                //NormalMonitorManager�ɂ�郂�j�^�[�̐����J�n
                normalMonitorManager.gameObject.SetActive(true);

                //BGM���Đ� 
                if (audioSource.isPlaying) audioSource.Stop();
                audioSource.clip = BGM[1];
                audioSource.Play();

                break;

            //���{�X�̏���
            case STATE.MIDDLE_BOSS:
                //�����ŉ҂����R�C�����X�V
                currentCoin = normalMonitorManager._currentCoin;

                //n�b�����A���P�b�g�p���`�̃��x�������[�h
                skillEffectManager.SkillLevelLoad();

                //HandDetection��L����
                handDetection.enabled = true;

                //Boss���j�^�[�̐���
                monitorAppearance.gameObject.SetActive(true);

                //BGM���Đ� 
                if (audioSource.isPlaying) audioSource.Stop();
                audioSource.clip = BGM[2];
                audioSource.Play();

                break;

            //��{�X�̏���
            case STATE.LAST_BOSS:
                if (festivalMode) gameInformation.progress = 3;
                _progress = gameInformation.progress;

                //�����ŉ҂����R�C�����X�V
                currentCoin = normalMonitorManager._currentCoin;

                //HandDetection��L����
                handDetection.enabled = true;

                //n�b�����A���P�b�g�p���`�̃��x�������[�h
                skillEffectManager.SkillLevelLoad();

                //Boss���j�^�[�̐���
                monitorAppearance.gameObject.SetActive(true);

                //BGM���Đ�
                if(audioSource.isPlaying) audioSource.Stop();
                audioSource.clip = BGM[2];
                audioSource.Play();

                break;

            //�N���A���̏���
            case STATE.CLEAR:
                bossManager = GameObject.FindWithTag("MB").GetComponent<MIDDLE_BOSS>();

                usableSkill = false;

                //�l���R�C���̍X�V
                currentCoin += bossManager.CoinGet(_progress);
                Debug.Log("�l���R�C��" + currentCoin);

                //HandDetection�̖�����
                handDetection.enabled = false;

                //�l���R�C���������R�C����
                PlayerPrefs.SetInt("TOTAL_COIN", gameInformation.havingTotalCoin + currentCoin);

                //���񐔂��J�E���g
                _numberOfPlays++;
                PlayerPrefs.SetInt("NUMBER_OF_PLAYS", _numberOfPlays);
                PlayerPrefs.Save();

                if(_progress == 3)
                {
                    //���X�{�X(4�̖ڂ̃{�X)��|��������ꉉ�o
                    lightParticle.Play();
                    LastAnimation();

                    //�N���A�����珉�߂���ɂȂ�
                    foreach(string _key in key) 
                        PlayerPrefs.DeleteKey( _key );
                    PlayerPrefs.Save();
                    StartCoroutine(clearManager.SceneChange(10));
                    return;
                }
                else
                {
                    //�i�s�x��i�߂�
                    _progress++;
                    PlayerPrefs.SetInt("PROGRESS", _progress);
                }

                //�N���A�̃e�L�X�g��\��
                clearManager.Coin_Text(currentCoin);

                //�^�C�g���փV�[���J��
                StartCoroutine(clearManager.SceneChange(3));

                break;

            //�Q�[���I�[�o�[���̏���
            case STATE.GAME_OVER:
                usableSkill = false;
                lineRenderer.enabled = true;

                //HandDetection�̖�����
                handDetection.enabled = false;

                //�l���R�C���������R�C����
                PlayerPrefs.SetInt("TOTAL_COIN", gameInformation.havingTotalCoin + currentCoin);

                //���񐔂��J�E���g
                _numberOfPlays++;
                PlayerPrefs.SetInt("NUMBER_OF_PLAYS", _numberOfPlays);
                PlayerPrefs.Save();

                //�Q�[���I�[�o�[��UI��\��
                gameOverManager.Coin_Text(currentCoin);
                StartCoroutine(gameOverManager.ButtonDisplay());

                break;
        }
    }

    //�Ō�̃v�����Z�X�̃A�j���[�V����
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
