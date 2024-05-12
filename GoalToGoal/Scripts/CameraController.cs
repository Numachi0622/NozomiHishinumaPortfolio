using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{ 
    [SerializeField] CinemachineVirtualCamera vCamGoal; // ゴール時の切り替え後のカメラ


    // カメラを切り替え
    public void ChangeCam()
    {
        vCamGoal.Priority = 100;
    }
}
