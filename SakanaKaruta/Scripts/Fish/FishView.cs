using UnityEngine;
using DG.Tweening;

public class FishView : MonoBehaviour
{
    [SerializeField] private Sprite[] fishSprites;
    [SerializeField] private Sprite[] hiraganaSprites;
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer childSpriteRenderer;
    private Transform tf;
    private float animStartTime;
    private bool isFlipping = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        tf = transform;
        if (tf.childCount == 0) return;
        childSpriteRenderer = tf.GetChild(0).GetComponent<SpriteRenderer>();
    }

    public void Init(FishData fishData)
    {
        int fishId = fishData.GetId;
        spriteRenderer.sprite = fishSprites[fishId];
        spriteRenderer.material.SetFloat("_Shift", Random.Range(0.0f, 1.0f));
        if (childSpriteRenderer == null) return;
        childSpriteRenderer.sprite = hiraganaSprites[fishId];
    }

    public void Flip(FishDirection dir)
    {
        if(isFlipping) return;
        float targetScale = dir == FishDirection.Right ? -1.0f : 1.0f;
        tf.DOScaleX(targetScale, 0.5f)
            .OnStart(() =>
            {
                isFlipping = true;
                if (childSpriteRenderer != null)
                {
                    tf.GetChild(0).localScale = new Vector3(targetScale, 1f, 1f);
                };
            })
            .OnComplete(() => isFlipping = false);
    }

    public void SetLayer()
    {
        spriteRenderer.sortingOrder++;
        childSpriteRenderer.sortingOrder++;
    }

    public void ResetLayer()
    {
        spriteRenderer.sortingOrder--;
        childSpriteRenderer.sortingOrder--;
    }
}
