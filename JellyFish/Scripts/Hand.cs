using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] private Transform indexTf; // 人差し指の先端のTransform
    private Vector3 previousPos; // 前フレームの座標    
    private Vector3 direction; // 人差し指の方向
    public Vector3 Direction => direction; // 公開用

    private void Start()
    {
        previousPos = indexTf.position;
    }

    private void Update()
    {
        // 前フレームの座標との差分から方向を計算
        direction = (indexTf.position - previousPos).normalized;
        previousPos = indexTf.position;
    }
}
