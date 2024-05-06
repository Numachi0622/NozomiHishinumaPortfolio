using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

// �q�̏������܂Ƃ߂��N���X
public class Customer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI myText; // ���g�̃Z���t��\������text
    [SerializeField] private GameObject chopsticks; // �q�������I�u�W�F�N�g
    [SerializeField] private AudioClip ramenSE; // ���[��������������ʉ�
    private UIDisplayer uiDisplayer; // UIDisplayer�N���X
    private ScoreManager scoreManager; // �X�R�A���Z�o����N���X
    private Animator animator; // ���g��Animator�R���|�[�l���g
    private Vector3 sittingPos = new Vector3(-3.2f, 0.2f, 1.26f); // ����Ƃ����g�̍��W
    private Quaternion sittingRot = Quaternion.Euler(0, -180, 0); // ����Ƃ��̎��g��Quaternion

    [SerializeField] private string orderLine; // ��������Ƃ��̋q�̃Z���t
    [SerializeField] private string[] feedback = new string[6]; // �]���̃Z���t
    private string[] scoreValue = { "S", "A", "B", "C", "D", "E" }; // �X�R�A
    private Dictionary<string, string> resultLine = new Dictionary<string, string>(); // �]���̃Z���t���X�R�A�ƑΉ��t����

    private void Awake()
    {
        // �R���|�[�l���g���擾
        animator = GetComponent<Animator>();
        uiDisplayer = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIDisplayer>();
        scoreManager = GameObject.FindGameObjectWithTag("Canvas").GetComponent<ScoreManager>();

        // ���g��Text�R���|�[�l���g��n��
        uiDisplayer.SetCustomerText(myText);
    }
    private void Start()
    {
        // Dictionary�ɑ��
        for(int i = 0; i < feedback.Length; i++)
        {
            resultLine[scoreValue[i]] = feedback[i];
        }
        WalkToSittingPosition();
    }

    // �֎q�ɍ���܂ł̕��s�A�j���[�V���������s���郁�\�b�h
    public void WalkToSittingPosition()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(transform.DOMoveX(sittingPos.x, 4f).SetEase(Ease.Linear))
            .Append(transform.DORotateQuaternion(sittingRot, 0.5f))
            .Append(transform.DOMove(sittingPos, 1f))
            .OnComplete(() =>
            {
                animator.SetTrigger("Sit");
                uiDisplayer.DisplayCustomerUI(orderLine,false);
            });
    }

    // ���[������H�ׂ�A�j���[�V�������\�b�h
    public void EatAnimation()
    {
        chopsticks.SetActive(true);
        chopsticks.transform.DOLocalMove(Vector3.up * 0.1f, 0.5f)
            .SetEase(Ease.Linear)
            .SetLoops(4, LoopType.Yoyo)
            .SetDelay(2f)
            .OnComplete(() =>
            {
                chopsticks.SetActive(false);
                uiDisplayer.DisplayCustomerUI(resultLine[scoreManager.Score()],true);
            });
        SoundManager.instance.PlaySE(ramenSE);
    }
}
