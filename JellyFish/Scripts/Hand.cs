using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] private Transform indexTf; // �l�����w�̐�[��Transform
    private Vector3 previousPos; // �O�t���[���̍��W    
    private Vector3 direction; // �l�����w�̕���
    public Vector3 Direction => direction; // ���J�p

    private void Start()
    {
        previousPos = indexTf.position;
    }

    private void Update()
    {
        // �O�t���[���̍��W�Ƃ̍�������������v�Z
        direction = (indexTf.position - previousPos).normalized;
        previousPos = indexTf.position;
    }
}
