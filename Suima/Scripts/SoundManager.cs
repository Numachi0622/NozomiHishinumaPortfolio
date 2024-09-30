using UnityEngine;
using DG.Tweening;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioSource bgm;
    [SerializeField] private AudioSource se;
    [SerializeField] private SoundData soundData;
    private float bgmVol = 0.5f;
    private float coolDownTime = 0.05f;
    private float lastPlayTime = 0f;
    private string lastPlaySeKey;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        soundData.Init();
    }

    public void PlayBGM(string bgmKey)
    {
        bgm.DOFade(0f, 1f)
            .OnComplete(() => {
                bgm.Stop();
                bgm.volume = bgmVol;
                bgm.clip = soundData.BGMData[bgmKey];
                bgm.Play();
            });
    }

    public void InitBGM()
    {
        if (soundData.BGMData == null) return;
        bgm.clip = soundData.BGMData["Title"];
        bgm.Play();
    }

    public void PlaySE(string seKey)
    {
        if (lastPlaySeKey == seKey && Time.time - lastPlayTime < coolDownTime)
        {
            return;
        }
        se.PlayOneShot(soundData.SEData[seKey]);
        lastPlayTime = Time.time;
        lastPlaySeKey = seKey;
    }
}
