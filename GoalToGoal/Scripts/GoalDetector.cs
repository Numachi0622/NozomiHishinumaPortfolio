using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GoalDetector : MonoBehaviour
{
    private AudioSource audio;

    [SerializeField] CameraController camController;
    [SerializeField] StateManager stateManager;
    [SerializeField] GameObject[] ballPerticle;
    [SerializeField] GameObject[] goalPerticle;
    [SerializeField] AudioClip whistle;

    private void Start()
    {
        audio = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ball"))
        {
            stateManager.GoToFinishState();
            camController.ChangeCam();
            foreach (var per in ballPerticle)
            {
                per.SetActive(false);
            }
            foreach(var per in goalPerticle)
            {
                per.SetActive(true);
            }
            audio.PlayOneShot(whistle);
        }
    }
}
