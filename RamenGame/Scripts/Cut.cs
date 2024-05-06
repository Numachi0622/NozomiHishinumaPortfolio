using UnityEngine;

// �؂�A�N�V�����֘A�̏������܂Ƃ߂��N���X
public class Cut : MonoBehaviour
{
    [SerializeField] private MeshRenderer myKnifeRenderer; // ���g�̎�Ɏ����MeshRenderer�R���|�[�l���g
    [SerializeField] private GameObject staticKnife; // ���ɒu����Ă����I�u�W�F�N�g
    [SerializeField] private GameManager gameManager; // GameManager�N���X
    [SerializeField] private AudioClip cutSE; // �؂����Ƃ���SE
    public int count { get; private set; } // �؂�����

    private void Start()
    {
        count = 0;
    }

    // ��ɕ���������郁�\�b�h
    public void SetKnife()
    {
        if (!gameManager.gameStartable) return;
        // ���̕���A�N�e�B�u��
        staticKnife.SetActive(false);
        // ��̕��\��
        myKnifeRenderer.enabled = true;   
    }

    // �؂����Ƃ��̏����̎��s���郁�\�b�h
    public void CutAction()
    {
        count++;
        SoundManager.instance.PlaySE(cutSE);
        // VR�̃R���g���[���[��U��������
        OVRInput.SetControllerVibration(0, 1f, OVRInput.Controller.RTouch);
    }
}
