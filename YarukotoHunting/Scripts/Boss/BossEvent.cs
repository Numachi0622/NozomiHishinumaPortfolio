using UnityEngine;
using DG.Tweening;

public class BossEvent : MonoBehaviour
{
    [SerializeField] private Transform signTf;

    public void DisplayEventSign()
    {
        if(!signTf.gameObject.activeSelf)
            signTf.gameObject.SetActive(true);
        signTf.DOScale(Vector3.one * 1.3f,0.3f).SetLoops(-1,LoopType.Yoyo);
    }

    public void HideEventSign()
    {
        signTf.gameObject.SetActive(false);
        signTf.DOKill();
        signTf.localScale = Vector3.one;
    }
}
