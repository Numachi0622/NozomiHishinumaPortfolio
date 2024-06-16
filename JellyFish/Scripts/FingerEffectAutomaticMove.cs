using UnityEngine;

public class FingerEffectAutomaticMove : MonoBehaviour
{
    private Vector3 direction = Vector3.zero; // 自動で進む方向
    private Transform tf;
    private float speed = 2f; // 移動スピード

    private void Awake()
    {
        tf = transform;
    }

    private void Update()
    {
        tf.position += direction * Time.deltaTime * speed;
    }

    // 指を話した際に方向をセットする
    public void SetDirection(Vector3 dir)
    {
        direction = dir;
    }
}
