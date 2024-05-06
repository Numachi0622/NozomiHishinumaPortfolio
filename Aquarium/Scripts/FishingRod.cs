using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;
using UnityEngine.Events;

public class FishingRod : MonoBehaviour
{
    [SerializeField] private UIDisplay ui;
    [SerializeField] private InputManager input;
    [SerializeField] private Collider[] fishingRotColliders; // 竿のコライダー
    [SerializeField] private UnityEvent hitTimeEvent, waitTimeEvent;
    [SerializeField] private AudioClip fishingSE,reelSE;
    private Fish targetFish;
    private FishController targetFishController;
    private bool isHit = false;
    private float currentHittingTime = 0f;
    private float maxHittingTime = 10f;
    private float currentRotateCount = 0;
    private float maxRotateCount = 4;
    private float rotateTimeValue = 0;
    private float rotateIgnoreBorder = 0.3f;
    private int feedCount = 5;
    public bool isOnWater { get; private set; } // 水に入っているかを判定

    private const int DEFAULT_PRI = 10;
    private const float BASE_POS = 3.5f;

    private void Start()
    { 
        PositionReset();
    }

    private void Update()
    {
        if (!isHit) return;
        currentHittingTime += Time.deltaTime;
        rotateTimeValue += Time.deltaTime;
        ui.UpdateFishGauge(currentHittingTime);
        ui.UpdateRodGauge(currentRotateCount);
        if(currentHittingTime > maxHittingTime)
        {
            isHit = false;
            GaugeReset();
            targetFishController.ResumeMove();
            targetFishController = null;
            HitTimeCameraWorkOut();
            ui.DisplayHitTimeUI(false);
            feedCount--;
            ui.UpdateFeedUI(feedCount);
            ui.HideFishableSign();
            waitTimeEvent?.Invoke();
            if (feedCount < 1)
            {
                ui.DisplayResultUI();
                Debug.Log("koko");
                return;
            }
            DOVirtual.DelayedCall(1f, () => {
                foreach (Collider _c in fishingRotColliders)
                    _c.enabled = true;
            });
        }
        else if(currentRotateCount >= maxRotateCount)
        {
            ui.DisplayFishableSign();
            if (!input.isRodExit) return;
            isHit = false;
            targetFish.transform.parent = transform;
            PlayExitAnim();
            feedCount--;
            ui.UpdateFeedUI(feedCount);
        }
    }

    private void PositionReset()
    {
        transform.parent.position = Vector3.up * BASE_POS * 2;
    }

    private void GaugeReset()
    {
        currentHittingTime = 0;
        currentRotateCount = 0;
    }

    public void PlayEnterAnim()
    {
        if (isOnWater) return;
        isOnWater = true;
        if(feedCount < 1)
        {
            ui.DisplayResultUI();
            return;
        }
        transform.parent.DOMoveY(BASE_POS,0.5f).OnComplete(() => {
            foreach (Collider _c in fishingRotColliders)
                _c.enabled = true;
        });
    }

    public void PlayExitAnim()
    {
        isOnWater = false;
        transform.parent.DOMoveY(BASE_POS * 2, 0.5f).OnComplete(() =>
        {
            StartCoroutine(ui.DisplayGetFishUI(targetFish));
            targetFish.gameObject.SetActive(false);
            GaugeReset();
        });
        ui.HideFishableSign();
        SoundManager.instance.PlaySE(fishingSE);
    }


    public void HitTimeCameraWorkIn()
    {
        Camera.main.transform.DOMoveZ(-1.3f,0.5f);
    }

    public void HitTimeCameraWorkOut()
    {
        Camera.main.transform.DOMoveZ(-1.7f,0.5f);
        if (feedCount < 1) ui.DisplayResultUI();
    }

    public void SetRotateCount()
    {
        if (rotateTimeValue < rotateIgnoreBorder) return;
        currentRotateCount += 1 / rotateTimeValue;
        rotateTimeValue = 0;
        SoundManager.instance.PlaySE(reelSE);
    }

    private void OnTriggerEnter(Collider other)
    {
        isHit = true;
        foreach(Collider _c in fishingRotColliders)
            _c.enabled = false;
        targetFish = other.GetComponent<Fish>();
        targetFishController = other.GetComponent<FishController>();
        targetFishController?.MoveStop();
        //targetFish.transform.parent = this.transform;
        hitTimeEvent?.Invoke();
        HitTimeCameraWorkIn();
        ui.DisplayHitTimeUI(true);
    }
}
