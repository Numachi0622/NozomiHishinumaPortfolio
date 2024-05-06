using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : MonoBehaviour
{
    protected float distance; // プレイヤーと敵の距離

    protected Vector3 positionDiff; // プレイヤーと敵の座標の差分
    protected Vector3 direction; // プレイヤーへの方向

    protected NavMeshAgent agent;
    private Animator animator;
    private EnemyStatus status;
    protected Collider targetCollider;
    [SerializeField] private GameObject vCam;
    [SerializeField] private GameObject enemyUI;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        targetCollider = GameObject.FindWithTag("Player").GetComponent<Collider>();
        if(vCam == null)
            vCam = GameObject.FindGameObjectWithTag("MainCamera");
        if (!GetComponent<EnemyStatus>()) return;
        status = GetComponent<EnemyStatus>();
    }

    protected virtual void Update()
    {
        animator.SetFloat("Speed", agent.velocity.magnitude);
        transform.LookAt(targetCollider.transform);
        enemyUI.transform.LookAt(vCam.transform);
    }

    // プレイヤーを追いかける
    public virtual void ChasePlayer()
    {
        if (!status.isMovable) return;
        // 座標の差分を計算
        positionDiff = targetCollider.transform.position - this.transform.position;

        // 距離を計算
        distance = positionDiff.magnitude;

        // 向く方向を計算
        direction = positionDiff.normalized;

        agent.isStopped = false;
        agent.destination = targetCollider.transform.position;
    }

    // プレイヤーを見失った際の処理
    public void MoveStop()
    { 
        agent.isStopped = true;
    }

    public void SetStartPosition(Vector3 _pos)
    {
        agent.Warp(_pos);
    }
}
