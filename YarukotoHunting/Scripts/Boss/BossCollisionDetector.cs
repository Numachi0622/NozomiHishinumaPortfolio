using UnityEngine;

public class BossCollisionDetector : CollisionDetector
{
    [SerializeField] private BossStatus status;

    protected override void OnTriggerEnter(Collider other)
    {
        status.isPhysicalAttackable = true;
        base.OnTriggerEnter(other);
    }
    protected override void OnTriggerExit(Collider other)
    {
        status.isPhysicalAttackable = false;
        base.OnTriggerExit(other);
    }
}
