using System.Collections;
using UnityEngine;
using Cinemachine;

public class BeforeBossEvent : MonoBehaviour
{
    [SerializeField] private ProgressManager progressManager;
    [SerializeField] private ExperiencePointSystem expSystem;
    [SerializeField] private CinemachineVirtualCamera eventVCam;
    [SerializeField] private Collider[] eventCollider = new Collider[5];
    [SerializeField] private GameObject portal, allBossFightableWindow;
    private int[] borderRank = { 10, 30, 50, 80 };
    private int priority = 11,basePriority = 4;
    private bool firstBossFightable => borderRank[0] <= expSystem.Rank && progressManager.progress == 0;
    private bool secondBossFightable => borderRank[1] <= expSystem.Rank && progressManager.progress == 1;
    private bool thirdBossFightable => borderRank[2] <= expSystem.Rank && progressManager.progress == 2;
    private bool lastBossFightable => borderRank[3] <= expSystem.Rank && progressManager.progress == 3;
    private bool allBossFightable => progressManager.progress == 4;
    private bool isOpen;

    private void Start()
    {
        if (firstBossFightable)
        {
            eventCollider[0].enabled = true;
            isOpen = true;
            portal.SetActive(true);
        }
        else if (secondBossFightable)
        {
            eventCollider[1].enabled = true;
            isOpen = true;
            portal.SetActive(true);
        }
        else if (thirdBossFightable)
        {
            eventCollider[2].enabled = true;
            isOpen = true;
            portal.SetActive(true);
        }
        else if (lastBossFightable)
        {
            eventCollider[3].enabled = true;
            isOpen = true;
            portal.SetActive(true);
        }
        else if (allBossFightable)
        {
            eventCollider[4].enabled = true;
            isOpen = true;
            portal.SetActive(true);
            allBossFightableWindow.SetActive(true);
            progressManager.UpdateProgress();
        }
        else return;
    }

    public void Event()
    {
        if (isOpen) return;
        if (firstBossFightable)
        {
            StartCoroutine(CameraControll());
            eventCollider[0].enabled = true;
            isOpen = true;
        }
        else if (secondBossFightable)
        {
            StartCoroutine(CameraControll());
            eventCollider[1].enabled = true;
            isOpen = true;
        }
        else if (thirdBossFightable)
        {
            StartCoroutine(CameraControll());
            eventCollider[2].enabled = true;
            isOpen = true;
        }
        else if (lastBossFightable)
        {
            StartCoroutine(CameraControll());
            eventCollider[3].enabled = true;
            isOpen = true;
        }
        else return;
    }

    IEnumerator CameraControll()
    {
        eventVCam.Priority = priority;
        yield return new WaitForSeconds(2);
        portal.SetActive(true);
        yield return new WaitForSeconds(4);
        eventVCam.Priority = basePriority;
    }
}
