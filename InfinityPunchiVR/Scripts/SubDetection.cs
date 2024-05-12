using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubDetection : MonoBehaviour
{
    private Vector3 hitPos; // ÚG‚µ‚½À•W
    [SerializeField] private MainDetection main; // MainDetectionƒNƒ‰ƒX
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Main")) return;
        hitPos = other.ClosestPoint(transform.position);
        main.SetInFrontPosition(hitPos);
    }
}
