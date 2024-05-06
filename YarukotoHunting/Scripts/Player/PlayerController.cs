using Cinemachine;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    private float moveSpeed = 5f; // 移動スピード
    private float rotBorder = 0.5f; // 回転するための最小移動量
    private float rotControl = 600; // 1フレームの回転量
    private float rotSensitivity = 150; // スワイプによる方向転換の感度
    private bool isBlowAway = false;
    public bool isRotatable { get; private set; } // UpdateとDoTweenによる回転を切り替える

    protected Vector3 moveVelocity; // Playerの移動量のベクトル

    private Quaternion targetRot; // 進行方向に向くようなQuaternion
    private Quaternion horizontalRotation; // スワイプ量によって向きを変えるQuaternion

    private Rigidbody rb;
    private PlayerStatus playerStatus;
    private Transform tf;
    private CinemachinePOV pov;
    private Animator animator;

    [SerializeField] private CinemachineVirtualCamera vCam;
    [SerializeField] private TouchManager touchManager;
    [SerializeField] private Collider phycsicalCollider;
    [SerializeField] private RotateStop rotateStop;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip runSE;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerStatus = GetComponent<PlayerStatus>();
        tf = GetComponent<Transform>();
        pov = vCam.GetCinemachineComponent<CinemachinePOV>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        moveVelocity.y = 0;
        targetRot = transform.rotation;
        isRotatable = true;
    }

    protected virtual void Update()
    {
        if (isBlowAway) return;

        if (playerStatus.isMovable)
        {
            moveVelocity.x = SimpleInput.GetAxis("Horizontal");
            moveVelocity.z = SimpleInput.GetAxis("Vertical");
        }
        else
        {
            moveVelocity.x = 0;
            moveVelocity.z = 0;
        }

        if (touchManager.touchInput.phase == TouchPhase.Moved && touchManager.touchInput.position.x > Screen.width / 2)
        {
            if (touchManager.isTouchJoystic && touchManager.touchCount == 1)
            {
                // ジョイスティックだけタッチしても視点移動しない
                AdjustSensitivity(0);
            }
            else
            {
                AdjustSensitivity(rotSensitivity);
            }
        }
        else
        {
            // 感度を0にすることでスワイプしていない間は視点移動をしないようにする
            AdjustSensitivity(0);
        }

        // スワイプの量によって回転角を計算
        horizontalRotation = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up);

        // 移動量ベクトルを回転も含めたものに計算しなおす
        moveVelocity = horizontalRotation * moveVelocity.normalized;

        // 進行方向を向くようにする
        if (moveVelocity.magnitude > rotBorder) targetRot = Quaternion.LookRotation(moveVelocity.normalized, Vector3.up);

        // 回転制御
        if(isRotatable) tf.rotation = Quaternion.RotateTowards(tf.rotation, targetRot, rotControl * Time.deltaTime);

        // アニメーションの再生
        animator.SetFloat("Speed", moveVelocity.magnitude);

        if (moveVelocity.magnitude > 0)
            PlayRunSE();
        else
            audioSource.Stop();
    }

    protected virtual void FixedUpdate()
    {
        // 移動させる
        rb.velocity = moveVelocity * moveSpeed;
    }

    // 感度を調整
    public void AdjustSensitivity(float _sensitivity)
    {
        pov.m_HorizontalAxis.m_MaxSpeed = _sensitivity;
        pov.m_VerticalAxis.m_MaxSpeed = _sensitivity;
    }

    private bool IsViewpointMovement()
    {
        if (touchManager.isTouchButton && touchManager.isTouchButton && touchManager.touchCount == 1) return false;
        else return true;
    }

    public void MakeRotatable()
    {
        isRotatable = true;
    }
    public void IsDashAttacking()
    {
        isRotatable = false;
    }

    public void BlowAway(Transform _origin)
    {
        rb.useGravity = true;
        vCam.Priority = 1;
        this.enabled = false;
        rb.AddExplosionForce(15f,_origin.position,10,1f,ForceMode.Impulse);
        Vector3 rotDirection = new Vector3(Random.Range(-10,10),0, Random.Range(-10, 10));
        rb.AddTorque(rotDirection,ForceMode.Impulse);
        // アニメーション再生後、プレイヤーとコライダーの向きを調整する
        phycsicalCollider.gameObject.transform.localRotation = Quaternion.Euler(90, 0, 0);
        // 床コライダーを有効化
        rotateStop.ColliderActive();
    }

    public void RotateStop()
    {
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public void Jump(Vector3 _dest)
    {
        tf.DOJump(_dest,3f,1,1.5f).SetEase(Ease.Linear);
        playerStatus.PlayJumpAnim();
    }

    public void PlayRunSE()
    {
        if (audioSource.isPlaying) return;
        audioSource.PlayOneShot(runSE);
    }

    public void StopRunSE()
    {
        audioSource.Stop();
    }

    public void SensitivitySetting(float _sens)
    {
        rotSensitivity = _sens;
    }
}
