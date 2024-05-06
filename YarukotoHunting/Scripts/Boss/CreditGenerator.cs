using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditGenerator : MonoBehaviour
{
    [SerializeField] private BossStatus bossStatus;
    [SerializeField] private GameObject creditPrefab;
    private List<GameObject> pool = new List<GameObject>();
    private Vector3[] appearPoint = new Vector3[2];
    private WaitForSeconds interval = new WaitForSeconds(3);
    private int maxCreditNum = 22;

    private void Start()
    {
        for (int i = 0; i < appearPoint.Length; i++)
            appearPoint[i] = transform.GetChild(i).position;
        StartCoroutine(Generate());
    }

    private GameObject GetCreditFromPool()
    {
        for(int i = 0;i < pool.Count; i++)
        {
            if (!pool[i].activeSelf)
            {
                pool[i].SetActive(true);
                pool[i].transform.position = FirstPos();
                return pool[i];
            }
        }
        GameObject newCredit = Instantiate(creditPrefab,FirstPos(),Quaternion.identity,transform);
        pool.Add(newCredit);
        return newCredit;

    }

    private Vector3 FirstPos()
    {
        return appearPoint[Random.Range(0, 2)];
    }

    IEnumerator Generate()
    {
        while (bossStatus.state != StatusManager.STATE.DIE)
        {
            yield return interval;
            if (ActiveCount() < maxCreditNum)
            {
                GameObject credit = GetCreditFromPool();
            }
        }
    }

    private int ActiveCount()
    {
        int count = 0;
        for(int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.activeSelf) count++;
        }
        return count;
    }
}
