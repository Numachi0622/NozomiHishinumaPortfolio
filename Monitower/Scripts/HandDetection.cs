using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus;

public class HandDetection : MonoBehaviour
{ 
    //GameManager��unableSkill�ōU���\���A�X�L�������\���𔻕�
    [SerializeField] GameManager gameManager;

    [SerializeField] SkillEffectManager skillEffectManager;
    [SerializeField] GameInformation gameInformation;

    //���b������ԉ��𔻒�
    public bool strengthenMode;

    //���Transform
    [SerializeField] Transform rightHandTf,leftHandTf;

    //��̍��W
    Vector3 rightHandPos,leftHandPos;

    //���E�̎�̈ړ�����
    public float distanceRight,distanceLeft;

    //�O�t���[���̍��W(�����v�Z�̍ۂɎg�p)
    private Vector3 previousPosRight, previousPosLeft;

    //���P�b�g�����̍��W(����p)
    const float ROCKET_LAUNCH_POINT = 0.35f;

    //���肪���P�b�g���������𖞂����Ă��邩
    private bool leftHandKey;

    [SerializeField] Collider rocketLaunchCollider;

    private void Start()
    {
        strengthenMode = false;
        ResetDistance();
        previousPosRight = rightHandTf.localPosition;
        previousPosLeft = leftHandTf.localPosition;
    }

    private void Update()
    {
        //HandPos�ɗ���̍��W����
        rightHandPos = rightHandTf.localPosition;
        leftHandPos = leftHandTf.localPosition;

        distanceRight = DistanceCalculationRight(distanceRight);
        distanceLeft = DistanceCaluculationLeft(distanceLeft);

        //����̏ꍇ�̃��P�b�g��������
        if(leftHandPos.y > ROCKET_LAUNCH_POINT) leftHandKey = true;
        else leftHandKey = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "LeftHand" && gameManager.usableSkill)
        {
            if (gameInformation.powerUpTimeLevel < 2)
            {
                return;
            }

            //n�b������Ԃɓ���
            strengthenMode = true;
            //���̉��o
            skillEffectManager.StartStrengthenMode();
        }

        if (other.gameObject.CompareTag("RocketLaunchPoint") && leftHandKey && gameManager.usableSkill)
        {
            //���P�b�g����
            skillEffectManager.RocketLaunch();

            //Collider�𖳌���
            rocketLaunchCollider.enabled = false;
        }
    }

    //�E��̈ړ��������v�Z
    private float DistanceCalculationRight(float distance)
    {
        //���݂̍��W����U�ۑ�
        Vector3 currentPos = rightHandPos;

        //���݂̍��W�ƕۑ����Ă��������W�̍����̋������v�Z
        float currentDistance = Vector3.Distance(currentPos,previousPosRight);

        //���ړ������ɉ��Z
        distance += currentDistance;

        //���݂̍��W��ۑ�
        previousPosRight = currentPos;

        return distance;
    }

    //����̈ړ��������v�Z
    private float DistanceCaluculationLeft(float distance)
    {
        //���݂̍��W����U�ۑ�
        Vector3 currentPos = leftHandPos;

        //���݂̍��W�ƕۑ����Ă��������W�̍����̋������v�Z
        float currentDistance = Vector3.Distance(currentPos, previousPosLeft);

        //���ړ������ɉ��Z
        distance += currentDistance;

        //���݂̍��W��ۑ�
        previousPosLeft = currentPos;

        return distance;
    }

    //������0�ɏ���������֐�
    public void ResetDistance()
    {
        distanceLeft = 0;
        distanceRight = 0;
    }
}
