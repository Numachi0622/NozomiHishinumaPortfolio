using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperGenerator : MonoBehaviour
{
    [SerializeField] private GameObject paperPrefab;
    [SerializeField] private Transform playerTf;
    [SerializeField] private AudioSource copierSource,fireSource;
    [SerializeField] private AudioClip copierSE,fireSE;
    private List<GameObject> paperPool = new List<GameObject>();
    private int fireCount = 10;
    private WaitForSeconds generateInterval = new WaitForSeconds(0.5f);
    private WaitForSeconds generateDelay = new WaitForSeconds(1.5f);
    private WaitForSeconds smokeTime = new WaitForSeconds(9);

    private GameObject GetPaperFromPool(Transform _tf)
    {
        for(int i = 0;i < paperPool.Count; i++)
        {
            if (!paperPool[i].activeSelf)
            {
                paperPool[i].transform.position = _tf.position;
                return paperPool[i];
            }
        }
        GameObject newPaper = Instantiate(paperPrefab,_tf.position,Quaternion.identity,gameObject.transform);
        paperPool.Add(newPaper);
        return newPaper;
    }

    public void GeneratePaper(Transform _tf)
    {
        StartCoroutine(FirePaper(_tf));
    }
    IEnumerator FirePaper(Transform _tf)
    {
        Collider col = _tf.GetComponent<Collider>();
        col.enabled = false;
        copierSource.PlayOneShot(copierSE);
        yield return generateDelay;
        for(int i = 0; i < fireCount; i++)
        {
            yield return generateInterval;
            fireSource.PlayOneShot(fireSE);
            GameObject paper = GetPaperFromPool(_tf);
            if (!paper.activeSelf)
                paper.SetActive(true);
            paper.GetComponent<Paper>()?.Fire(playerTf, _tf);

            if (i == fireCount - 1)
                col.enabled = true;
        }
    }

    public void AppearSmoke(ParticleSystem _par)
    {
        StartCoroutine(Appear(_par));
    }
    IEnumerator Appear(ParticleSystem _par)
    {
        _par.Play();
        yield return smokeTime;
        _par.Stop();
    }
}
