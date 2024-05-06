using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

// �Q�[���̏�Ԃ��Ǘ�����enum
public enum State
{
    Title,      // �^�C�g��
    Animation,  // �q�̗��X���̃A�j���[�V�������Đ�
    Game,       // ���ۂɖ˂�؂�
    Result      // ���U���g��\��
}

// �Q�[���̏�Ԃ��Ǘ�����N���X
public class GameManager : MonoBehaviour
{
    [SerializeField] private UIDisplayer uiDisplayer; // UI�\���p��UIDisplayer�N���X
    [SerializeField] private Collider knifeColider; // �i�C�t��Collider
    [SerializeField] private Transform ramenTf; // �ړ������郉�[������Transform
    [SerializeField] private GameObject cutTarget; // �؂�^�[�Q�b�g�I�u�W�F�N�g�i�ˁj
    [SerializeField] private State state; // ���݂̏�Ԃ��i�[
    [SerializeField] private UnityEvent customerEvent; // �q�̃��\�b�h���C�x���g�Ƃ��ČĂяo��
    public bool gameStartable { get; private set; } // �Q�[�����n�߂��邩�ǂ���

    private void Start()
    {
        // �ŏ��̓^�C�g����Ԃɂ��Ă���
        state = State.Title;
    }

    // Animation��Ԃɕω������郁�\�b�h
    public void GoToAnimationState()
    {
        state = State.Animation;
    }

    // Game��Ԃɕω������郁�\�b�h
    public void GoToGameState()
    {
        state = State.Game;
        knifeColider.enabled = true;
    }

    // Result��Ԃɕω������郁�\�b�h
    public void GoToResultState()
    {
        state = State.Result;
        knifeColider.gameObject.SetActive(false);
        cutTarget.SetActive(false);
        uiDisplayer.DisplayStopText(this);
    }
    
    public void SetStartable()
    {
        gameStartable = true;
    }

    // ���[������񋟂���A�j���[�V�������\�b�h
    public void RamenEvent()
    {
        ramenTf.gameObject.SetActive(true);
        Vector3 deskPos = new Vector3(-3.2f,1,0.5f);
        var sequence = DOTween.Sequence();
        sequence.Append(ramenTf.DOMoveX(-3.2f, 2f).SetDelay(2f))
            .Append(ramenTf.DOMove(new Vector3(-3.2f, 1.5f, -0.5f), 1f).SetEase(Ease.Linear).SetDelay(1f))
            .Append(ramenTf.DOMove(deskPos, 1f).SetEase(Ease.Linear))
            .OnComplete(() => customerEvent?.Invoke());
    }
}
