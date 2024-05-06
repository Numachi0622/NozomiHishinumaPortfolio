using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CoinGenerator : MonoBehaviour
{
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private AudioSource coinSource;
    [SerializeField] private AudioClip coinSE;
    private List<GameObject> coinPool = new List<GameObject>();
    private float coinLifeRange = 30f;
    private int coinCount = 10;
    private WaitForSeconds generateInterVal = new WaitForSeconds(0.5f);

    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
            coinPool.Add(transform.GetChild(i).gameObject);
    }

    private GameObject GetCoinFromPool()
    {
        for (int i = 0;i < coinPool.Count;i++)
        {
            if (!coinPool[i].activeSelf)
            {
                coinPool[i].transform.position = transform.position;
                coinPool[i].SetActive(true);
                return coinPool[i];
            }
        }
        GameObject newCoin = Instantiate(coinPrefab, transform.position, Quaternion.identity, transform);
        coinPool.Add(newCoin);
        return newCoin;
    }

    public IEnumerator GenerateCoin(Transform _targetTf)
    {
        for (int i = 0; i < coinCount; i++)
        {
            GameObject coin = GetCoinFromPool();
            Vector3 dest = coin.transform.position + (_targetTf.position - coin.transform.position).normalized * coinLifeRange;
            dest.y = transform.position.y;
            coin.transform.DOMove(dest, 2f).OnComplete(() => coin.SetActive(false));
            coinSource.PlayOneShot(coinSE);
            yield return generateInterVal;
        }
    }

    public void HideCoin(GameObject _coin)
    {
        _coin.SetActive(false);
        _coin.transform.DOKill();
    }
}
