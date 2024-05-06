using UnityEngine;

// �q��I�o����@�\���������N���X
public class CustomerSelect : MonoBehaviour
{
    [SerializeField] private GameObject[] customerPrefab = new GameObject[5]; // 5��ނ̋q��Prefab
    private GameObject customer; // ��������q�I�u�W�F�N�g
    private int customerId; // �ǂ̋q�������ʂ���ϐ�

    // �Q�[���J�n���ɋq��I�o���郁�\�b�h
    public void Select()
    {
        customerId = Random.Range(0, customerPrefab.Length);
        customer = Instantiate(customerPrefab[customerId], transform.position, Quaternion.Euler(0,-90,0));
    }

    // GameManager��EventTrigger����Ă΂�郁�\�b�h
    public void EatAnimationEvent()
    {
        customer.GetComponent<Customer>()?.EatAnimation();
    }

}
