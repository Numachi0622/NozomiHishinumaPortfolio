using UnityEngine;
using DG.Tweening;

public class LightUpCollisionDetector : MonoBehaviour
{
    [SerializeField] private LightUpManager lightUpManager;
    private Collider myCollider; // ���g�̃R���C�_�[
    private AudioSource audioSource;
    private bool isChecking = false; // �ړ������Ȃ���N���Q�̏����`�F�b�N���Ă��邩
    private float checkTime = 1f; // �ړ�����

    private void Awake()
    {
        myCollider = GetComponent<Collider>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var jellyFishColor = other.transform.parent.GetComponent<JellyFishColor>();
        // �G�ꂽ�N���Q��JellyFishColor��n���A����List�Ɋi�[
        lightUpManager.AddJellyFish(jellyFishColor);
    }

    // ���M���n�_����I�_�܂ňړ�������i�ړ����ɐG�ꂽ�N���Q�̏��𓾂�j
    public void CheckJellyFishOrder(Vector3 startPos, Vector3 endPos)
    {
        if (isChecking) return;
        isChecking = true;
        transform.position = startPos;
        transform.rotation = Quaternion.LookRotation(endPos);
        myCollider.enabled = true;
        transform.DOMove(endPos, checkTime)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                isChecking = false;
                myCollider.enabled = false;
                audioSource.Play();
                // �ړ��I����A�n�����N���Q�̏������Ƀ��C�g�A�b�v���J�n����
                StartCoroutine(lightUpManager.JellyFishLightUp());
            });
    }
}
