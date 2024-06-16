using UnityEngine;

public class JellyFishGenerator : MonoBehaviour
{
    [SerializeField] private GameObject jellyFishPrefab; // クラゲのプレファブ
    [SerializeField] private int generateNum = 50; // 生成数

    private void Start()
    {
        for (int i = 0; i < generateNum; i++)
        {
            GameObject jellyFish = Instantiate(jellyFishPrefab, Rand(), Random.rotation);
        }
    }

    private Vector3 Rand()
    {
        return new Vector3(
            Random.Range(-0.1f, 0.1f),
            Random.Range(0.75f, 2.5f),
            Random.Range(-0.1f, 0.75f)
        );
    }
}
