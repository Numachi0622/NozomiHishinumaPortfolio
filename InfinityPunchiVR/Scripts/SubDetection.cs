using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubDetection : MonoBehaviour
{
    private Vector3 hitPos; // 接触した座標
    [SerializeField] private MainDetection main; // MainDetectionクラス
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Main")) return;
        hitPos = other.ClosestPoint(transform.position);
        main.SetInFrontPosition(hitPos);
    }
}
