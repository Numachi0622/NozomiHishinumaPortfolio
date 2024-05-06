using UnityEngine;
using UnityEngine.Events;

public class CollisionDetector : MonoBehaviour
{
    [SerializeField] UnityEvent<Collider> onTriggerEnter = new UnityEvent<Collider>();
    [SerializeField] UnityEvent<Collider> onTriggerStay = new UnityEvent<Collider>();
    [SerializeField] UnityEvent<Collider> onTriggerExit = new UnityEvent<Collider>();

    protected virtual void OnTriggerEnter(Collider other)
    {
        onTriggerEnter.Invoke(other);
    }

    protected void OnTriggerStay(Collider other)
    {
        onTriggerStay.Invoke(other);
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        onTriggerExit.Invoke(other);
    }
}
