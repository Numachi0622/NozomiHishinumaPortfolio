using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketStopper : MonoBehaviour
{
    [SerializeField] Collider rocketLaunchCollider;

    private void OnParticleSystemStopped()
    {
        rocketLaunchCollider.enabled = true;
    }
}
