using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{
    [SerializeField] private GameObject buffItem, recoverItem;
    private List<GameObject> itemPool = new List<GameObject>();
    private Transform tf;
    private WaitForSeconds generateInterval = new WaitForSeconds(20);
    private float height = 1.3f;

    private void Awake()
    {
        tf = transform;
    }

    private void Start()
    {
        for (int i = 0; i < tf.childCount; i++)
            itemPool.Add(tf.GetChild(i).gameObject);
    }
    private void OnEnable()
    {
        StartCoroutine(GenerateItem());
    }

    private GameObject GetItemFromPool(Vector3 _pos)
    {
        for(int i = 0;i < itemPool.Count; i++)
        {
            if (!itemPool[i].activeSelf)
            {
                itemPool[i].SetActive(true);
                itemPool[i].transform.position = _pos;
                return itemPool[i];
            }
        }
        GameObject newItem;
        if (Random.Range(0, 2) == 0)
            newItem = Instantiate(buffItem,_pos,Quaternion.identity,tf);
        else
            newItem = Instantiate(recoverItem,_pos,Quaternion.identity,tf);
        itemPool.Add(newItem);
        return newItem;
    }

    private IEnumerator GenerateItem()
    {
        while (true)
        {
            Vector3 pos = new Vector3(Random.Range(-18f, 18), height, Random.Range(-18f, 18f));
            GameObject item = GetItemFromPool(pos);
            yield return generateInterval;
        }
    }

    public void HideItem(GameObject _item)
    {
        _item.SetActive(false);
    }
}
