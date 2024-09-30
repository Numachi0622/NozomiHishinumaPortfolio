using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class ResultEnemyGenerator : MonoBehaviour
{
    [SerializeField] private GameObject resultEnemy;
    [SerializeField] private BoxCollider attackCollider;

    public void GenerateStart()
    {
        StartCoroutine(Generate(KnockDownCounter.TotalKnockDownCount));
    }

    public IEnumerator Generate(int count)
    {
        attackCollider.transform.position = Vector3.right * 100f;
        attackCollider.size = Vector3.zero;
        
        WaitForSeconds wait = new WaitForSeconds(2f / count);
        for (int i = 0; i < count; i++)
        {
            GameObject go = Instantiate(resultEnemy, GeneratePos(), Quaternion.Euler(0, 180f, 0), transform);
            yield return wait;
        }

        attackCollider.size = new Vector3(20f, 20f, 1);
    }

    private Vector3 GeneratePos()
    {
        float x = 100f;
        return new Vector3(Random.Range(x - 9f, x + 9f), 10, 0);
    }
}
