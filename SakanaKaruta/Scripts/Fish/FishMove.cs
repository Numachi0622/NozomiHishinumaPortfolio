using System.Collections;
using UniRx;
using UnityEngine;
using DG.Tweening;

public enum FishDirection
{
    Left,
    Right
}

public class FishMove : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    private float baseSpeed;
    protected float minX = -15f;
    protected float maxX = 15f;
    private float minY = -7f;
    private float maxY = 5f;
    private float[] generatePosX = { -13f, 13f };
    private float escapeDistance = 10f;
    private Transform tf;
    protected Vector3 targetPos;
    private Vector3[] generatePos = new Vector3[2] { new Vector3(1,1,1), new Vector3(0,0,0)};
    private WaitForSeconds speedUpTime = new WaitForSeconds(1f);
    private ReactiveProperty<FishDirection> direction = new ReactiveProperty<FishDirection>();
    public ReactiveProperty<FishDirection> Direction => direction;
    private bool isMoveStop = false;
    public bool IsEscaping = false;

    private void Awake()
    {
        tf = transform;
    }

    protected virtual void Start()
    {
        baseSpeed = moveSpeed;
        generatePos = new Vector3[2] { 
            new Vector3(generatePosX[0], UnityEngine.Random.Range(minY, maxY), 0), 
            new Vector3(generatePosX[1], UnityEngine.Random.Range(minY, maxY), 0) 
        };
        tf.position = generatePos[UnityEngine.Random.Range(0,2)];
        SetNewTarget();
        SetDirection();
    }

    private void Update()
    {
        if (StateManager.Instance.State.Value == State.Result) return;
        Move();
    }

    private void Move()
    {
        if (isMoveStop) return;
        tf.position = Vector3.MoveTowards(tf.position, targetPos, moveSpeed * Time.deltaTime);
        if (IsFlip())
        {
            SetNewTarget();
            SetDirection();
        }
    }

    protected virtual void SetNewTarget()
    {
        targetPos = new Vector3(
            UnityEngine.Random.Range(minX, maxX),
            UnityEngine.Random.Range(minY, maxY),
            0
        );
    }

    private bool IsFlip()
    {
        return Vector3.Distance(tf.position, targetPos) < 0.1f;
    }

    protected void SetDirection()
    {
        direction.Value = tf.position.x < targetPos.x ? FishDirection.Right : FishDirection.Left;
    }

    public void Escape()
    {
        if(IsEscaping) return;
        targetPos = direction.Value == FishDirection.Right ?
            new Vector3(UnityEngine.Random.Range(minX, tf.position.x - 5.0f), UnityEngine.Random.Range(minY, maxY), 0)
            : new Vector3(UnityEngine.Random.Range(tf.position.x + 5.0f, maxX), UnityEngine.Random.Range(minY, maxY), 0);
        SetDirection();
        StartCoroutine(SpeedUp());
    }

    private IEnumerator SpeedUp()
    {
        if(moveSpeed > baseSpeed) yield break;
        moveSpeed *= 3f;
        IsEscaping = true;
        yield return speedUpTime;
        moveSpeed = baseSpeed;
        IsEscaping = false;
    }

    public void ResultEscape()
    {
        targetPos = direction.Value == FishDirection.Right ?
            new Vector3(minX, UnityEngine.Random.Range(minY, maxY), 0)
            : new Vector3(maxX, UnityEngine.Random.Range(minY, maxY), 0);
        SetDirection();
        tf.DOMove(targetPos, 2f);
    }

    public void MoveStop()
    {
        isMoveStop = true;
    }
}
