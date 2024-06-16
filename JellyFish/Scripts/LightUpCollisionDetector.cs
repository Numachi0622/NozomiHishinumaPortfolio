using UnityEngine;
using DG.Tweening;

public class LightUpCollisionDetector : MonoBehaviour
{
    [SerializeField] private LightUpManager lightUpManager;
    private Collider myCollider; // 自身のコライダー
    private AudioSource audioSource;
    private bool isChecking = false; // 移動中しながらクラゲの情報をチェックしているか
    private float checkTime = 1f; // 移動時間

    private void Awake()
    {
        myCollider = GetComponent<Collider>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var jellyFishColor = other.transform.parent.GetComponent<JellyFishColor>();
        // 触れたクラゲのJellyFishColorを渡し、順にListに格納
        lightUpManager.AddJellyFish(jellyFishColor);
    }

    // 自信を始点から終点まで移動させる（移動中に触れたクラゲの情報を得る）
    public void CheckJellyFishOrder(Vector3 startPos, Vector3 endPos)
    {
        if (isChecking) return;
        isChecking = true;
        transform.position = startPos;
        transform.rotation = Quaternion.LookRotation(endPos);
        myCollider.enabled = true;
        transform.DOMove(endPos, checkTime)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                isChecking = false;
                myCollider.enabled = false;
                audioSource.Play();
                // 移動終了後、渡したクラゲの情報を元にライトアップを開始する
                StartCoroutine(lightUpManager.JellyFishLightUp());
            });
    }
}
