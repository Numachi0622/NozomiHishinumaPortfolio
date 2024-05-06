using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishGenerator : MonoBehaviour
{
    [SerializeField] private GameObject[] fishPrefabs; // ê∂ê¨Ç∑ÇÈãõÇÃprefab
    [SerializeField] private int generatePerSeconds = 2;
    [SerializeField] private float generateRange = 5f;
    [SerializeField] private StateManager stateManager;
    private WaitForSeconds interval = new WaitForSeconds(5); // ê∂ê¨ä‘äu
    private bool isGenerate = false;

    private void Start()
    {
        StartCoroutine(Ganerate());
    }

    private IEnumerator Ganerate()
    {
        for (int i = 0; i < 15; i++)
        {
            GameObject fish = Instantiate(fishPrefabs[Random.Range(0, 19)],
                Quaternion.Euler(0, Random.Range(0f, 360f), 0) * Vector3.forward * generateRange + Vector3.up,
                Quaternion.Euler(0, Random.Range(0f, 360f), 0));
        }
        yield return new WaitUntil(() => isGenerate);
        while (true)
        {
            for (int i = 0; i < generatePerSeconds; i++)
            {
                GameObject fish = Instantiate(fishPrefabs[Random.Range(0, 19)],
                    Quaternion.Euler(0, Random.Range(0f, 360f), 0) * Vector3.forward * generateRange + Vector3.up,
                    Quaternion.Euler(0, Random.Range(0f, 360f), 0));
            }
            yield return interval;
        }
    }

    public void SetIsGenerate()
    {
        if (isGenerate) return;
        isGenerate = true;
    }
}
