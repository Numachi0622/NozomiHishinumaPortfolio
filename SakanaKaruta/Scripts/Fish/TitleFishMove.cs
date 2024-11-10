using UnityEngine;

public class TitleFishMove : FishMove
{
    protected override void Start()
    {
        SetNewTarget();
        SetDirection();
    }

    protected override void SetNewTarget()
    {
        targetPos = new Vector3(Random.Range(minX, maxX), transform.position.y, 0);
    }
}
