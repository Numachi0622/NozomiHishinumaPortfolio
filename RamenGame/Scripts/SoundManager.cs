using UnityEngine;
using DG.Tweening;

// 音源再生クラス
public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource bgmSource; // BGM用
    [SerializeField] private AudioSource seSource; // SE用
    [SerializeField] private AudioClip secondBGM;　// 2つ目のBGM
    private float volume = 0.1f; // BGMのボリューム
    public static SoundManager instance { get; private set; } // インスタンス

    private void Awake()
    {
        // インスタンスをシングルトン化
        instance = this;
    }

    // 引数のSEを再生するメソッド
    public void PlaySE(AudioClip _se)
    {
        seSource.PlayOneShot(_se);
    }

    // BGMを停止するメソッド
    public void StopBGM()
    {
        bgmSource.DOFade(0, 2f)
            .OnComplete(() =>
            {
                bgmSource.Stop();
                bgmSource.volume = volume;
            });
    }

    // BGMを再生するメソッド
    public void ResumeBGM()
    {
        bgmSource.clip = secondBGM;
        bgmSource.Play();
    }
}
