using UnityEngine;
using UnityEngine.Events;

// �����蔻��̔ėp�N���X
public class CollisionDetector : MonoBehaviour
{
    [SerializeField] private UnityEvent<Collider> onTriggerEnter = new UnityEvent<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        onTriggerEnter.Invoke(other);
    }
}
