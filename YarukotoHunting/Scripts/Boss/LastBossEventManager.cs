using System.Collections;
using UnityEngine;
using DG.Tweening;

public class LastBossEventManager : MonoBehaviour
{
    [SerializeField] private GameObject eventTimeObj,gameTimeObj;
    [SerializeField] private CanvasGroup inCanvas,outCanvas;
    [SerializeField] private AudioSource bgmSource,awakeningSource;
    [SerializeField] private AudioClip secondBGM;
    [SerializeField] private AudioClip[] awakeningSE = new AudioClip[2]; 

    public void InAwakeningEvent()
    {
        inCanvas.DOFade(1,1).OnComplete(() =>
        {
            gameTimeObj.SetActive(false);
            eventTimeObj.SetActive(true);
            outCanvas.DOFade(0,1);
        });
    }

    public void OutAwakeningEvent()
    {
        outCanvas.DOFade(1,1).OnComplete(() =>
        {
            awakeningSource.Stop();
            eventTimeObj.SetActive(false);
            gameTimeObj.SetActive(true);
            inCanvas.DOFade(0,1);
        });
    }
    public void BGMStop()
    {
        float vol = bgmSource.volume;
        bgmSource.DOFade(0,2f).OnComplete(() =>
        {
            bgmSource.volume = vol;
            bgmSource.Stop();
        });
    }

    public void BGMReplay()
    {
        bgmSource.clip = secondBGM;
        bgmSource.Play();
    }

    public void PlayAwakeningSE()
    {
        awakeningSource.PlayOneShot(awakeningSE[0]);
        StartCoroutine(DelayPlay());
    }
    IEnumerator DelayPlay()
    {
        yield return new WaitForSeconds(3f);
        awakeningSource.Stop();
        awakeningSource.PlayOneShot(awakeningSE[1]);
    }

}
