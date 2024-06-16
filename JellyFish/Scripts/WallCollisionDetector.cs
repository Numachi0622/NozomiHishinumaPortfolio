using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WallCollisionDetector : MonoBehaviour
{
    [SerializeField] private GameObject wavePrefab; // 波スプライトのプレファブ
    private List<GameObject> wavePool = new List<GameObject>(); // Instantiateした分の波を格納

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 hitPos = collision.contacts[0].point;
        // 衝突座標から6方向分加算したベクトル配列を生成
        Vector3[] directions =
        {
            hitPos + Vector3.right,
            hitPos - Vector3.right,
            hitPos + Vector3.forward,
            hitPos - Vector3.forward,
            hitPos + Vector3.up,
            hitPos - Vector3.up
        };
        // 6方向のうち(x,z) = (0,0) に近いものを選別
        Vector3 dir = NearestDirection(directions, hitPos);

        // 波をプール内から取得
        GameObject wave = GetWaveFromPool(hitPos);
        wave.transform.LookAt(dir);
    }

    // 衝突した座標のyが同じでx,zが0のベクトル
    public Vector3 TargetPos(Vector3 hitPos)
    {
        Vector3 targetPos = new Vector3(0, hitPos.y, 0);
        return targetPos;
    }

    // 引数に指定したベクトル配列の中からターゲットのベクトルに最も近いものを返す
    public Vector3 NearestDirection(Vector3[] dirs, Vector3 hitPos)
    {
        Vector3 dir = dirs
            .OrderBy(v => Vector3.Distance(v, TargetPos(hitPos)))
            .First();
        return dir;
    }

    // 波の生成をオブジェクトプールで実装
    private GameObject GetWaveFromPool(Vector3 hitPos)
    {
        for(int i = 0; i < wavePool.Count; i++)
        {
            if (!wavePool[i].activeSelf)
            {
                // プール内で非アクティブなものがあればそれを返す
                wavePool[i].transform.position = hitPos;
                wavePool[i].SetActive(true);
                return wavePool[i];
            }
        }
        
        // プール内から取得できるオブジェクトがなければ新たに生成
        GameObject newWave = Instantiate(wavePrefab, hitPos, Quaternion.identity);
        wavePool.Add(newWave);
        return newWave;
    }
}
