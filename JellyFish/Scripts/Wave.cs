using UnityEngine;
using DG.Tweening;

public class Wave : MonoBehaviour
{
    private SpriteRenderer sprite; // ���g�̃X�v���C�g
    private Vector3 maxScale = new Vector3(0.15f, 0.15f, 1); // �ő�X�P�[��
    private AudioSource audioSource;
    [SerializeField] private AudioClip[] waveSE = new AudioClip[6]; // ���ʉ��z��

    private void Awake(){
        audioSource = GetComponent<AudioSource>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void OnEnable(){
        audioSource.PlayOneShot(waveSE[Random.Range(0,waveSE.Length)]);

        // �A�N�e�B�u������邲�Ƃɔg�̃A�j���[�V���������s
        transform.localScale = Vector3.zero;
        sprite.material.DOFade(1f,0f);
        var sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(maxScale, 1.5f))
            .Join(sprite.DOFade(1f, 1f))
            .AppendCallback(() => sprite.material.DOFade(0f, 1f)
            .OnComplete(() => transform.gameObject.SetActive(false)));
    }
}
