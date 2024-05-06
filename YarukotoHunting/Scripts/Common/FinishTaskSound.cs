using UnityEngine;
using DG.Tweening;

public class FinishTaskSound : MonoBehaviour
{
    [SerializeField] private AudioSource main, sub;
    private float previousVolume;

    public void Play(bool _isLoop = false)
    {
        previousVolume = main.volume;
        main.DOFade(0,0.2f).OnComplete(() => sub.enabled = true);
        if (_isLoop) return;
        DOVirtual.DelayedCall(4f,() => Resume());
    }

    private void Resume()
    {
        sub.DOFade(0,0.2f).OnComplete(() =>
        {
            sub.volume = previousVolume;
            sub.enabled = false;
            main.DOFade(previousVolume, 1f);
        });
    }
}
