using UnityEngine;
using UnityEngine.AI;

public class CreditMove : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform targetTf;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        targetTf = GameObject.FindGameObjectWithTag("PlayerStatus").GetComponent<Transform>();
    }

    private void Update()
    {
        agent.destination = targetTf.position;
    }
}
