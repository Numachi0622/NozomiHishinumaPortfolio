using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LightUpManager : MonoBehaviour
{
    [SerializeField] private LightUpCollisionDetector lightUpCollisionDetector;
    [SerializeField] private float lightUppablefingerDistance = 0.01f; // 人差し指と親指の距離（この値以下でライトアップのトリガーとなる）
    [SerializeField] private float lightUpTime = 3f; // ライトアップの全体の時間
    private List<JellyFishColor> jellyFishColors = new List<JellyFishColor>(); // クラゲのリスト
    private Vector3 startPos, endPos; // 始点, 終点
    private float widenDistanceValue = 5f; // 始点と終点の距離
    [SerializeField] private Transform indexTf, thumbTf; // 人差し指と親指のTransform
    [SerializeField] private GameObject fingerEffectPrefab; // 指に追従するエフェクトのプレファブ
    private GameObject fingerEffect; // 指に追従するエフェクトを格納
    private WaitForSeconds interval; // 順にライトアップする際のクラゲ毎のインターバル
    [SerializeField] private Collider handCollider; // 手のコライダー

    private void Start()
    {
        startPos = endPos = Vector3.zero;
        interval = new WaitForSeconds(0.05f);
    }

    private void Update()
    {
        // 人差し指と親指の距離を計算
        float dist = Vector3.Distance(indexTf.position, thumbTf.position);
        if (dist < lightUppablefingerDistance)
        {
            if (fingerEffect == null)
            {
                fingerEffect = Instantiate(fingerEffectPrefab, indexTf.position, Quaternion.identity, indexTf);
                fingerEffect.GetComponent<ParticleSystem>().Play();
            }

            if (startPos == Vector3.zero)
            {
                // 最初に始点を設定
                startPos = indexTf.position;
            }
            else
            {
                // 指を離すまで終点を計算する
                endPos = indexTf.position;
            }

            if (handCollider.enabled)
            {
                handCollider.enabled = false;
            }
        }
        else if (dist >= lightUppablefingerDistance && startPos != Vector3.zero)
        {
            // 指を離した瞬間に始点と終点の座標から方向を計算する
            Vector3 direction = (endPos - startPos).normalized;
            // 始点と終点の距離が短いため、距離を広げる
            startPos -= direction * widenDistanceValue;
            endPos += direction * widenDistanceValue;
            // 始点と終点を渡し、その間をコライダーが動き、ライトアップする順を決定する
            lightUpCollisionDetector.CheckJellyFishOrder(startPos, endPos);

            startPos = endPos = Vector3.zero;
            if (fingerEffect != null)
            {
                // 指に追従していたエフェクトを子から外し、指定した方向に飛ばす
                fingerEffect.transform.SetParent(null);
                fingerEffect.GetComponent<FingerEffectAutomaticMove>().SetDirection(direction);
                Destroy(fingerEffect,3);
            }

            handCollider.enabled = true;
        }
    }

    // Listの順番でライトアップを実行する
    public IEnumerator JellyFishLightUp()
    {
        var lightUpJellyFish = new List<JellyFishColor>(jellyFishColors);
        // どこかのクラゲが光っていたら実行しない
        if (lightUpJellyFish.Any(jf => jf.IsLightingUp)) yield break;

        foreach(var jellyFish in lightUpJellyFish) 
        {
            StartCoroutine(jellyFish.ColorLerp());
            yield return interval;
        }

        // ライトアップ終了時にListの情報をクリア
        jellyFishColors.Clear();
    }

    // 外部からクラゲのListに値を加える
    public void AddJellyFish(JellyFishColor jf)
    {
        jellyFishColors.Add(jf);
    }
}
