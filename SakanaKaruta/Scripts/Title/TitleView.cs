using UnityEngine;
using UniRx;
using DG.Tweening;

public class TitleView : MonoBehaviour
{
    [SerializeField] private CanvasGroup titleCg;
    [SerializeField] private Transform startTextTf;
    private void Start()
    {
        StartTextAnim();
        StateManager.Instance.State.Subscribe(state =>
        {
            if (state != State.Game) return;
            FadeOutTitleUIAnim();
        });
    }

    private void StartTextAnim()
    {
        startTextTf.DOLocalMoveY(-300f, 1f)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void FadeOutTitleUIAnim()
    {
        titleCg.DOFade(0f, 1f).OnComplete(() =>
        {
            titleCg.gameObject.SetActive(false);
        });
    }
}
