using UnityEngine;

public class FingerEffectAutomaticMove : MonoBehaviour
{
    private Vector3 direction = Vector3.zero; // �����Ői�ޕ���
    private Transform tf;
    private float speed = 2f; // �ړ��X�s�[�h

    private void Awake()
    {
        tf = transform;
    }

    private void Update()
    {
        tf.position += direction * Time.deltaTime * speed;
    }

    // �w��b�����ۂɕ������Z�b�g����
    public void SetDirection(Vector3 dir)
    {
        direction = dir;
    }
}
