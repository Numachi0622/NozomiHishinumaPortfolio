using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDelete : MonoBehaviour
{
    private WaitForSeconds deleteTime;

    private void Start()
    {
        deleteTime = new WaitForSeconds(1);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.GetComponent<BlowAwayObject>()) return;
        if (transform.root.position.z >= 0) return;
        BlowAwayObject blowAway = other.gameObject.GetComponent<BlowAwayObject>();
        StartCoroutine(Delete(blowAway));
    }
    IEnumerator Delete(BlowAwayObject b)
    {
        yield return deleteTime;
        b.InActiveObject();
    }
}
