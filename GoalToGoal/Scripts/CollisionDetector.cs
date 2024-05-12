using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionDetector : MonoBehaviour
{
    [SerializeField] UnityEvent<Collider> onTriggerEnter;

    private void OnTriggerEnter(Collider other)
    {
        onTriggerEnter.Invoke(other);
    }
}
