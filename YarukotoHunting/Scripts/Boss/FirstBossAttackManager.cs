using System.Collections;
using UnityEngine;
using DG.Tweening;

public class FirstBossAttackManager : AttackManager
{
    private Vector3 defaultPos = new Vector3(0,6,0);
    private WaitForSeconds interval = new WaitForSeconds(0.25f);
    [SerializeField] private GameObject homeworks;
    [SerializeField] private AudioClip magicSE;

    public void HomeworkEffect()
    {
        StartCoroutine(AppearDelay());
    }
    IEnumerator AppearDelay()
    {
        audioSource.PlayOneShot(magicSE);
        for (int i = 0; i < homeworks.transform.childCount; i++)
        {
            GameObject _homework = homeworks.transform.GetChild(i).gameObject;
            _homework.SetActive(true);
            _homework.transform.position = new Vector3((int)Random.Range(-4f,4f),0,(int)Random.Range(0f,17f));
            _homework.transform.GetChild(0).DOLocalMoveY(-6,4).OnComplete(() =>
            {
                _homework.transform.GetChild(0).localPosition = defaultPos;
            });
            yield return interval;
        }
    }
}
