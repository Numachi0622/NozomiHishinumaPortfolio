using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Copier : MonoBehaviour
{
    private Transform tf;
    private WaitForSeconds interval = new WaitForSeconds(3);

    private void Awake()
    {
        tf = transform;
    }

    private void Start()
    {
        StartCoroutine(Shake());
    }

    IEnumerator Shake()
    {
        while (true)
        {
            tf.DOShakePosition(2f, 0.2f, 10, 1, false, true).SetDelay(3);
            yield return interval;
        }
    }
}
