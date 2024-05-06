using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchTarget : MonoBehaviour
{
    private Transform tf;
    [SerializeField] private List<Transform> targets = new List<Transform>();
    public bool inArea => targets.Count > 0;
    public Vector3 targetPos { get; private set; }
    public float distance { get; private set; }

    private void Start()
    {
        tf = transform;
    }

    private void Update()
    {
        if (!inArea) return;
        targetPos = targets[0].position;
        distance = (targetPos - tf.position).magnitude;

        if (targets.Count == 1) return;
        for(int i = 1; i < targets.Count; i++)
        {
            if ((targets[i].position - tf.position).magnitude < distance)
                targetPos = targets[i].position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        targets.Add(other.transform);
    }
    private void OnTriggerExit(Collider other)
    {
        targets.Remove(other.transform);
    }

    public void ClearTarget(Transform _target)
    {
        targets.Remove(_target);
    }
}
