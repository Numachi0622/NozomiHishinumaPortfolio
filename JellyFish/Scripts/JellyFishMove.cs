using UnityEngine;
using DG.Tweening;

public class JellyFishMove : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f; // クラゲの移動スピード
    [SerializeField] private Collider physicalCollider; // 物理コライダー
    private const float minMoveSpeed = 0.5f; // 最小移動スピード
    private const float maxMoveSpeed = 1.5f; // 最大移動スピード
    private Vector3 direction; // 移動方向
    private Rigidbody rb;

    [SerializeField] private Renderer headRenderer, skirtRenderer; // 頭部, スカート部分のRenderer
    [SerializeField] private Renderer[] footRenderer = new Renderer[8]; // 足のRenderer配列
    private const float minWaveSpeed = 4.0f; // クラゲのシェーダー部分の動きのスピード（波で制御）

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        direction = transform.forward;
        moveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);
        MaterialInit();
    }
    private void Update()
    {
        rb.velocity = direction * moveSpeed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Hand>())
        {
            HandHitBounce(collision);
            return;
        }
        WallHitBounce(collision);
    }

    // 手に触れたときに少しだけバウンドさせる
    public void HandHitBounce(Collision collision)
    {
        physicalCollider.enabled = false;
        // 手の動かした方向を取得
        Vector3 dir = collision.gameObject.GetComponent<Hand>().Direction;
        rb.DOMove(dir * 0.1f, 2f)
            .SetRelative()
            .SetEase(Ease.OutCubic)
            .OnComplete(() => physicalCollider.enabled = true);
    }

    // 壁に触れたときに反射させる
    public void WallHitBounce(Collision collision)
    {
        Vector3 normal = collision.contacts[0].normal;
        // 法線ベクトルから反射ベクトルを計算
        Vector3 reflect = Vector3.Reflect(direction, normal);
        direction = reflect;
        // 移動方向に向かせる
        transform.DOLookAt(direction, 3f);
    }

    private void MaterialInit()
    {
        // シェーダーに渡す波のパラメータを移動スピードをもとに計算
        float waveSpeed = minWaveSpeed + moveSpeed * 2f;
        headRenderer.material.SetFloat("_Speed", waveSpeed);
        skirtRenderer.material.SetFloat("_Speed", waveSpeed);
        foreach(Renderer r in footRenderer)
        {
            r.material.SetFloat("_Speed", waveSpeed);
        }
    }
}
