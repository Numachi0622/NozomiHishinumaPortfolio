using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class KickAnimation : MonoBehaviour
{
    private Animator animator;
    private AudioSource audio;

    [SerializeField] BallController ballController;
    [SerializeField] StateManager stateManager;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] GameObject ballParticle;
    [SerializeField] GameObject boostPerticle;
    [SerializeField] AudioClip kick;

    private void Start()
    {
        audio = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        this.gameObject.transform.parent = ballController.gameObject.transform;
    }

    public void StartKickAnimation()
    {
        animator.SetTrigger("Kick");
        HideKickButton();
        this.gameObject.transform.parent = null;
    }

    public void KickingJudgement()
    {
        ballController.isKickingUp = true;
        stateManager.GoToGameState();
        ballParticle.SetActive(true);
        boostPerticle.SetActive(true);
        audio.PlayOneShot(kick);
    }

    private void HideKickButton()
    {
        canvasGroup.DOFade(0,1);
    }
}
