using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{ 
    [SerializeField] CinemachineVirtualCamera vCamGoal; // �S�[�����̐؂�ւ���̃J����


    // �J������؂�ւ�
    public void ChangeCam()
    {
        vCamGoal.Priority = 100;
    }
}
