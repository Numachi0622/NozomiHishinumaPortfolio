using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class MouseClickView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI getFishText;
    [SerializeField] private GameObject badUiPrefab;
    [SerializeField] private GameObject hitEffectPrefab;
    private List<GameObject> badUiPool = new List<GameObject>();
    private List<GameObject> hitEffectPool = new List<GameObject>();
    private Vector2 scoreUiPos = new Vector2(8.5f, 2.5f);
    private bool isShowGetFishUI = false;
    private bool isShowBadUI = false;
    public bool IsShowGetFishUI => isShowGetFishUI;
    private IObservable<Vector3> onClick => Observable.EveryUpdate()
    .Where(_ => Input.GetMouseButtonDown(0))
    .Select(_ => Input.mousePosition);
    public IObservable<Vector3> OnClick => onClick;
    private ReactiveProperty<GameObject> badUi = new ReactiveProperty<GameObject>();
    public IObservable<GameObject> BadUI => badUi;

    public void HideFish(Fish fish, FishColliderModel fishColliderModel, ScoreModel scoreModel, QuestionModel questionModel, Action restart = null)
    {
        //fish.GetComponent<FishMove>().enabled = false;
        fish.GetComponent<FishMove>().MoveStop();
        var tf = fish.transform;
        var targetPos = new Vector2(0, -1.5f);
        var col = fish.GetComponent<Collider2D>();
        isShowGetFishUI = true;
        ShowGetFishText(fish);
        
        if (Mathf.Abs(tf.localScale.x) < 1.0f)
        {
            fish.GetComponent<FishView>().enabled = false;
            tf.DOKill();
            var target = tf.localScale.x < 0 ? new Vector3(-1, 1, 1) : Vector3.one;
            tf.localScale = target;
        }
        
        var targetScale = tf.localScale * 2f;
        var sequence = DOTween.Sequence()
            .Append(tf.DOMove(targetPos, 1f))
            .Join(tf.DOScale(targetScale, 1f)
                .OnStart(() =>
                {
                    Vector3 childTarget = tf.localScale.x < 0 ? new Vector3(-1, 1, 1) : Vector3.one;
                    tf.GetChild(0).localScale = childTarget;
                }))
            .AppendInterval(2.5f)
            .AppendCallback(() =>
            {
                fishColliderModel.FishColliders.Remove(col);
                restart?.Invoke();
            })
            .Append(tf.DOMove(scoreUiPos, 1f))
            .Join(tf.DOScale(Vector2.zero, 1f))
            .AppendCallback(() =>
            {
                Destroy(col.gameObject);
                scoreModel.AddScore(questionModel);
            })
            .AppendInterval(1f)
            .AppendCallback(() => isShowGetFishUI = false);
    }

    public void DestroyFish(Fish fish, FishColliderModel fishColliderModel)
    {
        var col = fish.GetComponent<Collider2D>();
        fishColliderModel.FishColliders.Remove(col);
        Destroy(col.gameObject);
    }

    public void ShowGetFishText(Fish fish)
    {
        SoundManager.Instance.PlaySE("GetFish");
        string fishName = fish.KanaName;
        getFishText.text = fishName + "ゲット！";
        var textTf = getFishText.transform;
        Vector2 startPos = textTf.localPosition;
        var sequence = DOTween.Sequence()
            .Append(textTf.DOLocalMove(new Vector2(0, 250f), 1f).SetEase(Ease.OutBack))
            .AppendInterval(1.5f)
            .Append(textTf.DOLocalMove(startPos, 1f).SetEase(Ease.InBack));
    }

    public void ShowBadUI(Vector2 pos)
    {
        if (isShowBadUI) return;
        isShowBadUI = true;
        SoundManager.Instance.PlaySE("Miss");
        badUi.Value = null;
        badUi.Value = GetUIFromPool();
        GameObject ui = badUi.Value;
        ui.transform.position = pos;
        float target = ui.transform.position.y + 0.75f;
        var sequence = DOTween.Sequence()
            .Append(ui.transform.DOMoveY(target, 0.3f))
            .Join(ui.GetComponent<Renderer>().material.DOFade(1f, 0.2f))
            .AppendInterval(0.5f)
            .Append(ui.GetComponent<Renderer>().material.DOFade(0f, 0.2f))
            .AppendCallback(() =>
            {
                ui.SetActive(false);
                isShowBadUI = false;
            });
    }

    private GameObject GetUIFromPool()
    {
        for(int i = 0; i < badUiPool.Count; i++)
        {
            if (!badUiPool[i].activeSelf)
            {
                badUiPool[i].SetActive(true);
                return badUiPool[i];
            }
        }
        GameObject newUi = Instantiate(badUiPrefab);
        badUiPool.Add(newUi);
        return newUi;
    }

    public void ShowHitEffect(Vector3 pos)
    {
        GameObject hitEffect = GetEffectFromPool();
        hitEffect.transform.position = pos;
        SoundManager.Instance.PlaySE("Hit");
    }

    private GameObject GetEffectFromPool()
    {
        for(int i = 0; i < hitEffectPool.Count; i++)
        {
            if (!hitEffectPool[i].activeSelf)
            {
                hitEffectPool[i].SetActive(true);
                return hitEffectPool[i];
            }
        }
        GameObject newEffect = Instantiate(hitEffectPrefab);
        hitEffectPool.Add(newEffect);
        return newEffect;
    }
}
