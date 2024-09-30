using System.Collections;
using UnityEngine;
using UnityEngine.Splines;

public class TitleEnemyGenerator : MonoBehaviour
{
    [SerializeField] private GameObject titleEnemyPrefab;
    [SerializeField] private SplineContainer[] leftSplines, rightSplines;
    [SerializeField] private Transform parent;
    [SerializeField] private BoxCollider[] titleColliders = new BoxCollider[2];
    [SerializeField] private Transform windowSplinesParent;
    private SplineContainer[][] splines;
    private WaitForSeconds interval;
    private WaitForSeconds coolDown;
    private bool[] isClosed = new bool[2] { false, false };
    private bool isStart => isClosed[0] && isClosed[1];
    private int colliderSize = 2;
    private const int Size = 20;

    private Vector3[] position = new Vector3[2]
    {
        new Vector3(-100f, 0f, 0f),
        new Vector3(95.5f, 0f, -1.5f)
    };

    private void Start()
    {
        splines = new SplineContainer[2][];
        splines[0] = new SplineContainer[Size];
        splines[1] = new SplineContainer[Size];
        for (int i = 0; i < leftSplines.Length; i++)
        {
            splines[0][i] = leftSplines[i];
            splines[1][i] = rightSplines[i];
        }

        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                Debug.Log(splines[i][j]);
            }
        }

        interval = new WaitForSeconds(20f);
        coolDown = new WaitForSeconds(1f);

        Vector3 startPos = position[1];
        startPos.y = -1f;
        windowSplinesParent.position = startPos;

        StartCoroutine(GenerateCycle());
    }

    private IEnumerator GenerateCycle()
    {
        while (!isStart)
        {
            if (!isClosed[0])
            {
                Generate(0);
            }
            if (!isClosed[1])
            {
                Generate(1);
            }
            yield return interval;
            Reset();
            yield return coolDown;
        }
    }
    public void Generate(int idx)
    {
        Enemy enemy = Instantiate(titleEnemyPrefab).GetComponent<Enemy>();
        SplineContainer spline = splines[idx][Random.Range(0, Size)];
        enemy.SetSplineContainer(spline);
        enemy.SetTargetDeviceId(idx);
        enemy.transform.SetParent(parent);
    }

    private void Reset()
    {
        for (int i = 0; i < splines.GetLength(0); i++)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }

    public void ResetTitleCollider(int idx)
    {
        if (titleColliders[idx].size == Vector3.one * colliderSize) return;
        titleColliders[idx].size = Vector3.one * colliderSize;
    }
}
