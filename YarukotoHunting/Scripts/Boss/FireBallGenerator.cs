using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FireBallGenerator : MonoBehaviour
{
    [SerializeField] private GameObject fireBallPrefab;
    [SerializeField] private LastBossStatus status;
    private List<GameObject> fireBallPool = new List<GameObject>();
    private Transform tf;
    private int fireBallCount = 20;
    private float angle;

    private void Awake()
    {
        tf = transform;
    }

    private void Start()
    {
        angle = 360f / fireBallCount;
        for(int i = 0; i < tf.childCount; i++)
            fireBallPool.Add(tf.GetChild(i).gameObject);
    }

    private GameObject GetFireBallFromPool()
    {
        for(int i = 0;i < fireBallPool.Count; i++)
        {
            if (!fireBallPool[i].activeSelf)
            {
                fireBallPool[i].SetActive(true);
                fireBallPool[i].transform.position = tf.position;
                return fireBallPool[i];
            }
        }
        GameObject newFireBall = Instantiate(fireBallPrefab,tf.position,Quaternion.identity,tf);
        fireBallPool.Add(newFireBall);
        return newFireBall;
    }

    public void GenerateFireBall()
    {
        for (int i = 0; i < fireBallCount; i++)
        {
            GameObject fireBall = GetFireBallFromPool();
            Vector3 dest = fireBall.transform.position + Quaternion.Euler(0, angle * i, 0) * Vector3.forward * 50;
            fireBall.transform.DOMove(dest,2.5f).OnComplete(() => fireBall.SetActive(false));
        }
    }

    public void GenerateAwakeningFireBall(Transform _targetTf)
    {
        GameObject fireBall = GetFireBallFromPool();
        Vector3 dest = tf.position + (_targetTf.position - tf.position) * 1.5f;
        dest.y = tf.position.y;
        fireBall.transform.DOMove(dest, 1f).OnComplete(() =>
        {
            AwakeningFireBallLoop(fireBall.transform,_targetTf);
        });
    }

    public void AwakeningFireBallLoop(Transform _myTf,Transform _targetTf)
    {
        Vector3 dest = _myTf.position + (_targetTf.position - _myTf.position) * 2f;
        dest.y = _myTf.position.y;
        _myTf.DOMove(dest,0.2f)
            .SetLoops(5,LoopType.Yoyo)
            .OnComplete(() => _myTf.gameObject.SetActive(false));
    }
}
