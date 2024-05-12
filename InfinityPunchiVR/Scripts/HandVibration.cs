using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Hand
{
    Right,
    Left
}
public class HandVibration : MonoBehaviour
{
    [SerializeField] private Hand hand;
    private WaitForSeconds vibrationTime = new WaitForSeconds(0.1f);

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Main")) return;
        StartCoroutine(Vibration());
    }

    private IEnumerator Vibration()
    {
        OVRInput.SetControllerVibration(0f, 1f, hand == Hand.Left ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch);
        yield return vibrationTime;
        OVRInput.SetControllerVibration(0f, 0f, hand == Hand.Left ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch);
    }
}
