using System.Collections.Generic;
using UnityEngine;

public class DamageEffect : MonoBehaviour
{
    [SerializeField] private GameObject hitParticlePrefab;
    private List<GameObject> pool = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        GameObject hitPerticle = GetEffectFromPool();
        if(!hitPerticle.activeSelf) hitPerticle.SetActive(true);
        hitPerticle.transform.position = other.ClosestPointOnBounds(transform.position);
    }

    private GameObject GetEffectFromPool()
    {
        for(int i = 0;i < pool.Count; i++)
        {
            if (!pool[i].activeSelf) return pool[i];
        }
        GameObject newObj = Instantiate(hitParticlePrefab,Vector3.zero,Quaternion.identity,transform.parent);
        pool.Add(newObj);
        return newObj;
    }
}
