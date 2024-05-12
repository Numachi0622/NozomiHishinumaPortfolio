using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubDetection : MonoBehaviour
{
    private Vector3 hitPos; // �ڐG�������W
    [SerializeField] private MainDetection main; // MainDetection�N���X
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Main")) return;
        hitPos = other.ClosestPoint(transform.position);
        main.SetInFrontPosition(hitPos);
    }
}
