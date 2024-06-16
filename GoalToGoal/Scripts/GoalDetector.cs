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
    [SerializeField] GameObject[] ballParticle;
    [SerializeField] GameObject[] goalParticle;
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
            foreach (var par in ballParticle)
            {
                par.SetActive(false);
            }
            foreach(var par in goalParticle)
            {
                par.SetActive(true);
            }
            audio.PlayOneShot(whistle);
        }
    }
}
