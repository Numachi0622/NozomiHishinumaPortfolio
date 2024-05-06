using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class DamageText : MonoBehaviour
{
    [SerializeField] private GameObject damageTextPrefab; // 表示するダメージテキストのプレファブ
    private List<GameObject> textPool = new List<GameObject>();
    [SerializeField] private GameObject vCam; // ダメージテキストはこのカメラの方向に向かせるようにする

    private void Awake()
    {
        if (vCam == null)
            vCam = Camera.main.gameObject;
    }

    // ダメージテキストを表示
    public void AppearDamageText(float _damage)
    {
        GameObject damageText = GetTextFromPool();
        if(!damageText.activeSelf) damageText.SetActive(true);
        damageText.transform.localPosition = SettingPosition();
        damageText.transform.LookAt(vCam.transform);
        damageText.GetComponent<TextMeshProUGUI>().text = _damage.ToString();
        CanvasGroup canvasGroup = damageText.GetComponent<CanvasGroup>();   

        damageText.transform.DOLocalMoveY(damageText.transform.localPosition.y + 0.2f,1);
        canvasGroup.DOFade(0,1).SetEase(Ease.InCubic).OnComplete(() =>
        {
            damageText.SetActive(false);
            canvasGroup.alpha = 1;
            textPool.Add(damageText);
        });
    }

    // テキストの初期値を設定する
    private Vector3 SettingPosition()
    {
        Vector3 firstPos = Vector3.zero;
        firstPos.x = Random.Range(-0.25f,0.25f);
        return firstPos;
    }

    // テキストの生成をオブジェクトプール化
    private GameObject GetTextFromPool()
    {
        for (int i = 0; i < textPool.Count; i++)
        {
            if (!textPool[i].activeSelf) return textPool[i];
        }
        GameObject newText = Instantiate(damageTextPrefab, Vector3.zero, Quaternion.identity, this.transform);
        return newText;
    }
}
