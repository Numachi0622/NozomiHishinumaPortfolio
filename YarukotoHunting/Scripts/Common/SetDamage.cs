using UnityEngine;

public class SetDamage : CollisionDetector
{
    private StatusManager myStatus;

    private void Awake()
    {
        myStatus = transform.parent.GetComponent<StatusManager>();
    }
    protected override void OnTriggerEnter(Collider other)
    {
        if (!other.transform.parent.GetComponent<StatusManager>()) return;
        myStatus.SetDamage(other.transform.parent.GetComponent<StatusManager>().Attack);
        base.OnTriggerEnter(other);
    }
}
