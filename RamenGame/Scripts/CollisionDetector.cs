using UnityEngine;
using UnityEngine.Events;

// “–‚½‚è”»’è‚Ì”Ä—pƒNƒ‰ƒX
public class CollisionDetector : MonoBehaviour
{
    [SerializeField] private UnityEvent<Collider> onTriggerEnter = new UnityEvent<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        onTriggerEnter.Invoke(other);
    }
}
