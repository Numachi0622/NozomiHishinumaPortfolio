using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastBossCollisionDetector : CollisionDetector
{
    [SerializeField] private LastBossStatus status;

    protected override void OnTriggerEnter(Collider other)
    {
        status.SetAttackJudge(true);
        base.OnTriggerEnter(other);
    }
    protected override void OnTriggerExit(Collider other)
    {
        status.SetAttackJudge(false);
        base.OnTriggerExit(other);
    }
}
