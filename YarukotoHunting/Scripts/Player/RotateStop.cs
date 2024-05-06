using System.Collections;
using UnityEngine;

public class RotateStop : MonoBehaviour
{
    [SerializeField] private PlayerStatus playerStatus;
    private Collider floor;

    private void Awake()
    {
        floor = GetComponent<Collider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (playerStatus.state != PlayerStatus.STATE.DIE) return;
        collision.gameObject.GetComponent<PlayerController>().RotateStop();
    }

    public void ColliderActive()
    {
        StartCoroutine(DelayActive());
    }
    IEnumerator DelayActive()
    {
        yield return null;
        floor.enabled = true;
    }
}
