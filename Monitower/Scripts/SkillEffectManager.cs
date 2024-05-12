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

    //n�b�������̕b��
    [SerializeField] TextMeshProUGUI timeText;

    //�c��b��
    private float reminingSeconds = 0;

    //���x���ʐ�������
    private float limitSeconds;

    //���x���ʃ��P�b�g�c��
    private int rocketNum;

    //����n�b�����X�L�����g�������𔻒�
    private bool alreadyUsed = false;

    private WaitForSeconds rocketWait = new WaitForSeconds(1);

    private AudioSource audioSource;


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        //������Ԃ̓e�L�X�g�A�p�[�e�B�N�����\��
        timeText.gameObject.SetActive(false);
        rightFire.gameObject.SetActive(false);
        leftFire.gameObject.SetActive(false);

        //���x���ʐ������Ԃ��擾
        limitSeconds = skillManager.PowerTimeLimit(gameInformation.powerUpTimeLevel);

        //���P�b�g�̎c�����v�Z
        rocketNum = RocketNumCaluclation(gameInformation.rocketNumLevel);
    }
    private void Update()
    {
        if (handDetection.strengthenMode)
        { 
            //���Ԍv���J�n
            reminingSeconds += Time.deltaTime;

            //�e�L�X�g��\��
            DisplayTimeText(handDetection.strengthenMode);

            if (limitSeconds - reminingSeconds <= 0)
            {
                //0�b�ɂȂ����狭����ԏI��
                handDetection.strengthenMode = false;
                FinishStregnthenMode();

                //�e�L�X�g���\��
                DisplayTimeText(handDetection.strengthenMode);
            }
        }

        if (monitorAppearance.gameObject.activeSelf && monitorAppearance.BossAppear)
        {
            bossManager = GameObject.FindGameObjectWithTag("MB").GetComponent<MIDDLE_BOSS>();
            monitorAppearance.BossAppear = false;
        }
    }

    //�e�L�X�g�̕\��/��\��
    private void DisplayTimeText(bool strMode)
    {
        timeText.gameObject.SetActive(strMode);
        timeText.text = (limitSeconds - reminingSeconds).ToString("n2");
    }

    //������Ԃ̏���
    public void StartStrengthenMode()
    {
        rightFire.gameObject.SetActive(true);
        rightFire.Play();
        leftFire.gameObject.SetActive(true);
        leftFire.Play();
        audioSource.PlayOneShot(powerUpSE);
    }

    //������ԏI���̏���
    private void FinishStregnthenMode()
    {
        rightFire.Stop();
        leftFire.Stop();
        //�����g���Ȃ�����
        alreadyUsed = true;
    }

    //���P�b�g����
    public void RocketLaunch()
    {
        //�c����0�������甭�����Ȃ�
        if (rocketNum <= 0) return;

        //���������������~������
        if(rocketParticle.isPlaying) rocketParticle.Stop();

        //���P�b�g��������
        rocketParticle.Play();
        StartCoroutine(RocketDamageDelay());
        audioSource.PlayOneShot(rocketSE);

        //�c�������炷
        rocketNum--;
    }

    //���P�b�g�c�����v�Z
    private int RocketNumCaluclation(int level)
    {
        //�ő�c��
        int max = 3;

        //���x���ʂɎc�����v�Z
        for(int i = 1;i < max + 1; i++)
        {
            if(level == i)
            {
                rocketNum = i - 1;
            }
        }
        return rocketNum;
    }

    //�g�p����X�L�����x���̃��[�h
    public void SkillLevelLoad()
    {
        //���x���ʐ������Ԃ��擾
        limitSeconds = skillManager.PowerTimeLimit(gameInformation.powerUpTimeLevel);

        //���P�b�g�̎c�����v�Z
        rocketNum = RocketNumCaluclation(gameInformation.rocketNumLevel);
    }

    //���P�b�g�p���`�̃_���[�W��x��ė^�������鏈��
    IEnumerator RocketDamageDelay()
    {
        yield return rocketWait;
        skillManager.RDamege();
        bossManager.MiddleBossHp -= skillManager.RLevel;
        hpGauge.GaugeReduction(skillManager.RLevel);
    }
}
