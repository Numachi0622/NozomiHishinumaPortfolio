using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private SoundData soundData;
    [SerializeField] private AudioSource se;
    private float lastPlayTime = 0f;
    private float coolDownTime = 0.05f;
    private string lastPlaySeKey;
    public static SoundManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        soundData.Init();
    }

    public void PlaySE(string key)
    {
        if (lastPlaySeKey == key && Time.time - lastPlayTime < coolDownTime)
        {
            return;
        }
        Debug.Log(key);
        se.PlayOneShot(soundData.Sounds[key]);
        lastPlaySeKey = key;
        lastPlayTime = Time.time;
    }
}
