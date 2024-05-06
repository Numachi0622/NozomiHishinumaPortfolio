using UnityEngine;
using DG.Tweening;

// �����Đ��N���X
public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource bgmSource; // BGM�p
    [SerializeField] private AudioSource seSource; // SE�p
    [SerializeField] private AudioClip secondBGM;�@// 2�ڂ�BGM
    private float volume = 0.1f; // BGM�̃{�����[��
    public static SoundManager instance { get; private set; } // �C���X�^���X

    private void Awake()
    {
        // �C���X�^���X���V���O���g����
        instance = this;
    }

    // ������SE���Đ����郁�\�b�h
    public void PlaySE(AudioClip _se)
    {
        seSource.PlayOneShot(_se);
    }

    // BGM���~���郁�\�b�h
    public void StopBGM()
    {
        bgmSource.DOFade(0, 2f)
            .OnComplete(() =>
            {
                bgmSource.Stop();
                bgmSource.volume = volume;
            });
    }

    // BGM���Đ����郁�\�b�h
    public void ResumeBGM()
    {
        bgmSource.clip = secondBGM;
        bgmSource.Play();
    }
}
