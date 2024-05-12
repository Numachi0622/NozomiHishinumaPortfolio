using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{ 
    [SerializeField] CinemachineVirtualCamera vCamGoal; // ƒS[ƒ‹‚ÌØ‚è‘Ö‚¦Œã‚ÌƒJƒƒ‰


    // ƒJƒƒ‰‚ğØ‚è‘Ö‚¦
    public void ChangeCam()
    {
        vCamGoal.Priority = 100;
    }
}
