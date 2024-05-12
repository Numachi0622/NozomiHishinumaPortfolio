using OVR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainDetection : MonoBehaviour
{
    private Vector3 mainHitPos; // メインの接触座標
    private Vector3 inFrontHitPos; // 手前の接触座標
    private Vector3 direction; // 残像の方向ベクトル
    private const float difference = 0.5f;
    private List<GameObject> hitEffectPool = new List<GameObject>(); // ヒットエフェクトのリスト（オブジェクトプール使用）
    [SerializeField] private AfterimageManager afterimageManager;
    [SerializeField] private AppearAfterimage appearAfterimage;
    [SerializeField] private Collider[] handCollider; // 0=>左 1=>右
    [SerializeField] private GameObject hitEffectPrefab; // ヒットエフェクトのプレファブ
    [SerializeField] private GameManager gameManager;

    [SerializeField] private SoundManager soundManager;
    [SerializeField] private AudioClip hitSE;

    // テスト用
    [SerializeField] private bool isVRTest;
    private void OnTriggerEnter(Collider other)
    {
        if (inFrontHitPos == null) return;
        // メインの接触座標を取得
        mainHitPos = other.ClosestPoint(transform.position);

        // ヒットエフェクトを表示
        GameObject hitEffect = GetHitEffectsFromPool(mainHitPos);
        if (!hitEffect.activeSelf) hitEffect.SetActive(true);

        // ヒットSEを再生
        soundManager.PlaySound(hitSE);

        // 練習状態では残像は表示されない
        if (!gameManager.SJudge) return;
        // 残像を表示
        direction = mainHitPos - inFrontHitPos;
        Quaternion targetRot = Quaternion.FromToRotation(transform.forward,direction);
        appearAfterimage.Appear(inFrontHitPos,targetRot);

        // リザルトに出す残像は少し後ろから表示する
        //inFrontHitPos.z -= difference;
        afterimageManager.Store(inFrontHitPos,targetRot);
        if (!isVRTest) return; // テスト用、後で消す
        if (other.CompareTag("LeftHand"))
        {
            handCollider[0].enabled = false;
            handCollider[1].enabled = true;
        }
        else
        {
            handCollider[0].enabled = true;
            handCollider[1].enabled = false;
        }
    }

    // 手前の当たり判定を一時的に格納
    public void SetInFrontPosition(Vector3 _pos)
    {
        inFrontHitPos = _pos;
    }

    // ゲーム終了後強制的に殴れなくする
    public void HandColliderOff()
    {
        foreach (Collider collider in handCollider) 
            collider.enabled = false;
    }

    // ヒットエフェクトをプールから取得
    private GameObject GetHitEffectsFromPool(Vector3 _pos)
    {
        for (int i = 0; i < hitEffectPool.Count; i++)
        {
            if (!hitEffectPool[i].activeSelf) return hitEffectPool[i];
        }
        GameObject newEffect = Instantiate(hitEffectPrefab,_pos,Quaternion.identity);
        hitEffectPool.Add(newEffect);
        return newEffect;
    }
}
