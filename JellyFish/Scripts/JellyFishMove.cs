using UnityEngine;
using DG.Tweening;

public class JellyFishMove : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f; // �N���Q�̈ړ��X�s�[�h
    [SerializeField] private Collider physicalCollider; // �����R���C�_�[
    private const float minMoveSpeed = 0.5f; // �ŏ��ړ��X�s�[�h
    private const float maxMoveSpeed = 1.5f; // �ő�ړ��X�s�[�h
    private Vector3 direction; // �ړ�����
    private Rigidbody rb;

    [SerializeField] private Renderer headRenderer, skirtRenderer; // ����, �X�J�[�g������Renderer
    [SerializeField] private Renderer[] footRenderer = new Renderer[8]; // ����Renderer�z��
    private const float minWaveSpeed = 4.0f; // �N���Q�̃V�F�[�_�[�����̓����̃X�s�[�h�i�g�Ő���j

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        direction = transform.forward;
        moveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);
        MaterialInit();
    }
    private void Update()
    {
        rb.velocity = direction * moveSpeed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Hand>())
        {
            HandHitBounce(collision);
            return;
        }
        WallHitBounce(collision);
    }

    // ��ɐG�ꂽ�Ƃ��ɏ��������o�E���h������
    public void HandHitBounce(Collision collision)
    {
        physicalCollider.enabled = false;
        // ��̓��������������擾
        Vector3 dir = collision.gameObject.GetComponent<Hand>().Direction;
        rb.DOMove(dir * 0.1f, 2f)
            .SetRelative()
            .SetEase(Ease.OutCubic)
            .OnComplete(() => physicalCollider.enabled = true);
    }

    // �ǂɐG�ꂽ�Ƃ��ɔ��˂�����
    public void WallHitBounce(Collision collision)
    {
        Vector3 normal = collision.contacts[0].normal;
        // �@���x�N�g�����甽�˃x�N�g�����v�Z
        Vector3 reflect = Vector3.Reflect(direction, normal);
        direction = reflect;
        // �ړ������Ɍ�������
        transform.DOLookAt(direction, 3f);
    }

    private void MaterialInit()
    {
        // �V�F�[�_�[�ɓn���g�̃p�����[�^���ړ��X�s�[�h�����ƂɌv�Z
        float waveSpeed = minWaveSpeed + moveSpeed * 2f;
        headRenderer.material.SetFloat("_Speed", waveSpeed);
        skirtRenderer.material.SetFloat("_Speed", waveSpeed);
        foreach(Renderer r in footRenderer)
        {
            r.material.SetFloat("_Speed", waveSpeed);
        }
    }
}
