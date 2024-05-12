using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BallController : MonoBehaviour
{
    private float targetHeight = 2f; // �ۂ���������
    private float heightDifference; // �ۂ����������ƌ��݂̍����̍���
    private float forceMultiplier = 100f; // �͂̔{��
    private float delayTime = 1f; // �S�[�����{�[������������܂ł̒x������
    private float defaultSpeed; // �X�s�[�h�̏����l
    private float highSpeed; // �X�s�[�h�A�b�v���̃X�s�[�h�l
    private float lowSpeed; // �������̃X�s�[�h�l
    private Vector3 kickPos = new Vector3(0, 0.5f, -110); // �{�[���̃L�b�N�ʒu
    private Vector3 moveVelocity; // �{�[���̈ړ��ʂ̃x�N�g��
    private Vector3 force; // �d�͂ɔ������
    private Vector3 WallHitForce = new Vector3(1000,0,0);
    private Vector3 hitPos; // ��Q���ɐG�ꂽ���W
    private Vector3 direction; // �p�[�e�B�N���̌�������
    private Vector3 origin = new Vector3(0, 2, 0); // ��O���莞�̍ĊJ�ꏊ�̍��W
    private Quaternion rotation;
    private Quaternion target;

    private WaitForSeconds fallDownDelay; // �S�[�����{�[����������܂ł̒x��
    private WaitForSeconds speedChangeTime;
    private bool isHit; // ��Q���ɐG�ꂽ������
    private bool isSpeedUp; // �X�s�[�h�A�b�v��Ԃ��𔻒�
    private bool isSpeedDown; // �X�s�[�h�_�E����Ԃ��𔻒�

    private bool isPositionSelectable
        => StateManager.STATE.START == stateManager.state; // �ꏊ�I���\��

    private bool isMovable 
        => StateManager.STATE.GAME == stateManager.state && !isHit; // �ړ��\��

    private Rigidbody rb;
    private BlowAwayObject blowAway;
    private Animator animator;
    private Transform tf;

    public int hitCount; // �G�Ƀq�b�g�������̃J�E���g
    public bool isKickingUp; // �R��グ�Ă����Ԃ��𔻒�


    [SerializeField] float moveSpeed = 5; // �ړ��X�s�[�h
    [SerializeField] float moveZValue = 2; // �O�ɐi�ވړ���
    [SerializeField] float moveXValue = 2; // ���ɐi�ވړ���

    [SerializeField] Rigidbody childRb;
    [SerializeField] StateManager stateManager;
    [SerializeField] CameraController camController;
    [SerializeField] Transform childBallTf;
    [SerializeField] Transform boostTf;
    [SerializeField] Transform cameraTf;
    [SerializeField] AudioSource audio;
    [SerializeField] AudioSource subAudio;
    [SerializeField] GameObject[] myPerticle; // 0:��Q���q�b�g���@1:�X�s�[�h�A�b�v���@
    [SerializeField] AudioClip[] audioClips; // 0:��Q���q�b�g���@1:�X�s�[�h�A�b�v���@2:�X�s�[�h�_�E�����@3:�d����

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

        // �p�[�e�B�N����i�s�����Ɍ�������
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

            // �d�͂ɋt����Ĉ��̍�����ۂ�
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
