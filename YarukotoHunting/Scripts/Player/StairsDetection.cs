using UnityEngine;

public class StairsDetection : MonoBehaviour
{
    [SerializeField] private Transform destinationTf;
    private Vector3 destination;
    private float difference;
    private bool isUp => this.transform.position.z < destinationTf.position.z;
    private bool isUnder => this.transform.position.z > destinationTf.position.z;

    private void Start()
    {
        if (isUp) difference = 2f;
        else if (isUnder) difference = -2;
        destination = destinationTf.position + Vector3.forward * difference;
    }

    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<PlayerController>().Jump(destination);
    }
}
