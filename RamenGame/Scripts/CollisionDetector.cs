using UnityEngine;
using UnityEngine.Events;

// 当たり判定の汎用クラス
public class CollisionDetector : MonoBehaviour
{
    [SerializeField] private UnityEvent<Collider> onTriggerEnter = new UnityEvent<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        onTriggerEnter.Invoke(other);
    }
}
