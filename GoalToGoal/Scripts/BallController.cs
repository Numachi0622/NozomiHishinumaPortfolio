using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BallController : MonoBehaviour
{
    private float targetHeight = 2f; // 保ちたい高さ
    private float heightDifference; // 保ちたい高さと現在の高さの差分
    private float forceMultiplier = 100f; // 力の倍率
    private float delayTime = 1f; // ゴール時ボールが落下するまでの遅延時間
    private float defaultSpeed; // スピードの初期値
    private float highSpeed; // スピードアップ時のスピード値
    private float lowSpeed; // 減速時のスピード値
    private Vector3 kickPos = new Vector3(0, 0.5f, -110); // ボールのキック位置
    private Vector3 moveVelocity; // ボールの移動量のベクトル
    private Vector3 force; // 重力に反する力
    private Vector3 WallHitForce = new Vector3(1000,0,0);
    private Vector3 hitPos; // 障害物に触れた座標
    private Vector3 direction; // パーティクルの向く方向
    private Vector3 origin = new Vector3(0, 2, 0); // 場外判定時の再開場所の座標
    private Quaternion rotation;
    private Quaternion target;

    private WaitForSeconds fallDownDelay; // ゴール時ボールが落ちるまでの遅延
    private WaitForSeconds speedChangeTime;
    private bool isHit; // 障害物に触れたか判定
    private bool isSpeedUp; // スピードアップ状態かを判定
    private bool isSpeedDown; // スピードダウン状態かを判定

    private bool isPositionSelectable
        => StateManager.STATE.START == stateManager.state; // 場所選択可能か

    private bool isMovable 
        => StateManager.STATE.GAME == stateManager.state && !isHit; // 移動可能か

    private Rigidbody rb;
    private BlowAwayObject blowAway;
    private Animator animator;
    private Transform tf;

    public int hitCount; // 敵にヒットした時のカウント
    public bool isKickingUp; // 蹴り上げている状態かを判定


    [SerializeField] float moveSpeed = 5; // 移動スピード
    [SerializeField] float moveZValue = 2; // 前に進む移動量
    [SerializeField] float moveXValue = 2; // 横に進む移動量

    [SerializeField] Rigidbody childRb;
    [SerializeField] StateManager stateManager;
    [SerializeField] CameraController camController;
    [SerializeField] Transform childBallTf;
    [SerializeField] Transform boostTf;
    [SerializeField] Transform cameraTf;
    [SerializeField] AudioSource audio;
    [SerializeField] AudioSource subAudio;
    [SerializeField] GameObject[] myPerticle; // 0:障害物ヒット時　1:スピードアップ時　
    [SerializeField] AudioClip[] audioClips; // 0:障害物ヒット時　1:スピードアップ時　2:スピードダウン時　3:硬直時

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        tf = GetComponent<Transform>();
        fallDownDelay = new WaitForSeconds(delayTime);
        speedChangeTime = new WaitForSeconds(1);
        moveVelocity.y = 0;
        moveVelocity.z = moveZValue;
        defaultSpeed = moveSpeed;
        highSpeed = moveSpeed * 3;
        lowSpeed = moveSpeed / 2;
        transform.position = kickPos;
        isKickingUp = false;
    }

    void Update()
    {
        if(isMovable)
        {
            moveVelocity.x = Input.GetAxis("Horizontal") * moveXValue;
            moveVelocity.z = moveZValue;
            if (isSpeedUp) moveSpeed = highSpeed;
            else if (isSpeedDown) moveSpeed = lowSpeed;
            else moveSpeed = defaultSpeed;
        }
        else if (isPositionSelectable)
        {
            moveVelocity.x = Input.GetAxis("Horizontal");
            moveVelocity.z = 0;
            moveSpeed = defaultSpeed;
        }
        else
        {
            moveSpeed = 0;
        }

        // パーティクルを進行方向に向かせる
        rotation = Quaternion.Euler(0, cameraTf.eulerAngles.y, 0);
        direction = rotation * Vector3.forward;
        target = rotation * cameraTf.rotation;
        boostTf.rotation = Quaternion.RotateTowards(boostTf.rotation, target, 100 * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        rb.velocity = moveVelocity * moveSpeed;

        if (isMovable)
        {
            childRb.AddRelativeTorque(moveVelocity * moveSpeed * 5, ForceMode.Acceleration);

            // 重力に逆らって一定の高さを保つ
            if (isKickingUp)
            {
                heightDifference = targetHeight - transform.position.y;
                force = new Vector3(0, heightDifference * forceMultiplier, 0);
                rb.AddForce(force);
            }
        }
        else
        {
            childRb.angularVelocity = Vector3.zero;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            StartCoroutine(BallFallDown());
        }
        else if (other.CompareTag("KickingUpStopPoint"))
        {
            isKickingUp = false;
        }
        else if (other.CompareTag("SpeedBuff"))
        {
            StartCoroutine(SpeedUp());
            myPerticle[1].SetActive(true);
            audio.PlayOneShot(audioClips[1]);
            other.gameObject.SetActive(false);
        }
        else if (other.CompareTag("SpeedDebuff"))
        {
            StartCoroutine(SpeedDown());
            other.gameObject.SetActive(false);
            audio.PlayOneShot(audioClips[2]);
        }
        else if (other.CompareTag("Corner"))
        {
            ReturnPosition();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            if (tf.position.x > 0) rb.AddForce(-WallHitForce, ForceMode.Impulse);
            else rb.AddForce(WallHitForce, ForceMode.Impulse);
        }
        else if(!collision.gameObject.CompareTag("Goal"))
        {
            isHit = true;
            blowAway = collision.gameObject.GetComponent<BlowAwayObject>();
            collision.gameObject.layer = 6;
            hitPos = collision.contacts[0].point;
            if (collision.gameObject.CompareTag("SmallObstacle"))
            {
                isHit = false;
                Rigidity(0.01f, hitPos, blowAway);
            }
            else if (collision.gameObject.CompareTag("BigObstacle"))
            {
                Rigidity(3f, hitPos, blowAway);
                subAudio.PlayOneShot(audioClips[3]);
            }
            else if (collision.gameObject.CompareTag("Enemy"))
            {
                Rigidity(1f, hitPos, blowAway);
                subAudio.PlayOneShot(audioClips[3]);
            }
            hitCount++;
            audio.PlayOneShot(audioClips[0]);
            myPerticle[0].SetActive(true);
        }
    }

    IEnumerator BallFallDown()
    {
        yield return fallDownDelay;
        rb.useGravity = true;
        this.enabled = false;
    }

    public void Rigidity(float time, Vector3 pos, BlowAwayObject _blowAway)
    {
        if (stateManager.state == StateManager.STATE.GAME)
        {
            transform.DOShakePosition(time, 0.1f, 30, 1, false, false).OnComplete(() =>
            {
                isHit = false;
                _blowAway.BlowAway(hitPos);
            });
        }
    }
    IEnumerator SpeedUp()
    {
        isSpeedUp = true;
        yield return speedChangeTime;
        isSpeedUp = false;
    }

    IEnumerator SpeedDown()
    {
        isSpeedDown = true;
        yield return speedChangeTime;
        isSpeedDown = false;
    }

    private void ReturnPosition()
    {
        tf.position = origin;
    }
}
