using UnityEngine;

public class BossMove : EnemyMove
{
    private StatusManager status;
    private float defaultRadius;
    private float magnification;
    private float speed;
    [SerializeField] private SphereCollider forMoveCollider;

    protected override void Awake()
    {
        base.Awake();
        status = GetComponent<BossStatus>();
        if (status == null)
            status = GetComponent<LastBossStatus>();
    }

    private void Start()
    {
        defaultRadius = forMoveCollider.radius;
        magnification = 4f;
        speed = 2f;
    }
    protected override void Update()
    {
        base.Update();
        transform.LookAt(targetCollider.transform);
    }

    public override void ChasePlayer()
    {
        if (!status.isMovable) return;
        // 座標の差分を計算
        positionDiff = targetCollider.transform.position - this.transform.position;

        // 距離を計算
        distance = positionDiff.magnitude;

        // 向く方向を計算
        direction = positionDiff.normalized;

        agent.destination = targetCollider.transform.position;
        ResumeMove();
    }
    public void IdentifyPlayer(bool _isAble)
    {
        if (_isAble)
            forMoveCollider.radius = defaultRadius * magnification;
        else
            forMoveCollider.radius = defaultRadius;
    }

    // 移動を再開
    public void ResumeMove()
    {
        agent.isStopped = false;
    }

    public void SpeedToZero()
    {
        agent.speed = 0;
    }
    public void SetSpeed()
    {
        agent.speed = speed;
    }

}
