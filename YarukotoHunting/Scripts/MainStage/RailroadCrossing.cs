using UnityEngine;
using DG.Tweening;

public class RailroadCrossing : MonoBehaviour
{
    private float endPosX = -40;
    private Vector3 startPos = new Vector3(50,0,0);
    private bool isClosing;
    [SerializeField] private MeshRenderer[] mr;
    [SerializeField] private Animator[] animators;
    [SerializeField] private Transform trainTf;
    [SerializeField] private AudioSource rc, train;
    [SerializeField] private AudioClip rcSE,trainSE;

    public void RailroadCrossingEvent()
    {
        if (isClosing) return;
        if (Random.Range(0, 2) > 0) return;
        isClosing = true;
        rc.PlayOneShot(rcSE);
        foreach (MeshRenderer _mr in mr)
            _mr.material.SetFloat("_tennmetutime",2f);
        foreach(Animator _anim in animators)
            _anim.SetTrigger("Down");
        DOVirtual.DelayedCall(3, () =>
        {
            train.PlayOneShot(trainSE);
            float previousVol = train.volume;
            train.DOFade(0, 2f).SetDelay(2f).OnComplete(() => train.Stop());
            trainTf.gameObject.SetActive(true);
            trainTf.DOMoveX(endPosX, 4f).SetEase(Ease.Linear).OnComplete(() =>
            {
                foreach (MeshRenderer _mr in mr)
                    _mr.material.SetFloat("_tennmetutime", 0);
                foreach (Animator _anim in animators)
                    _anim.SetTrigger("Up");
                trainTf.position = startPos;
                trainTf.gameObject.SetActive(false);
                train.volume = previousVol;
                rc.Stop();
                isClosing = false;
            });
        });
    }
}
