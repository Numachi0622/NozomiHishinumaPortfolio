using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus;

public class HandDetection : MonoBehaviour
{ 
    //GameManagerのunableSkillで攻撃可能か、スキル発動可能かを判別
    [SerializeField] GameManager gameManager;

    [SerializeField] SkillEffectManager skillEffectManager;
    [SerializeField] GameInformation gameInformation;

    //ｎ秒強化状態化を判定
    public bool strengthenMode;

    //手のTransform
    [SerializeField] Transform rightHandTf,leftHandTf;

    //手の座標
    Vector3 rightHandPos,leftHandPos;

    //左右の手の移動距離
    public float distanceRight,distanceLeft;

    //前フレームの座標(距離計算の際に使用)
    private Vector3 previousPosRight, previousPosLeft;

    //ロケット発動の座標(左手用)
    const float ROCKET_LAUNCH_POINT = 0.35f;

    //左手がロケット発動条件を満たしているか
    private bool leftHandKey;

    [SerializeField] Collider rocketLaunchCollider;

    private void Start()
    {
        strengthenMode = false;
        ResetDistance();
        previousPosRight = rightHandTf.localPosition;
        previousPosLeft = leftHandTf.localPosition;
    }

    private void Update()
    {
        //HandPosに両手の座標を代入
        rightHandPos = rightHandTf.localPosition;
        leftHandPos = leftHandTf.localPosition;

        distanceRight = DistanceCalculationRight(distanceRight);
        distanceLeft = DistanceCaluculationLeft(distanceLeft);

        //左手の場合のロケット発動条件
        if(leftHandPos.y > ROCKET_LAUNCH_POINT) leftHandKey = true;
        else leftHandKey = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "LeftHand" && gameManager.usableSkill)
        {
            if (gameInformation.powerUpTimeLevel < 2)
            {
                return;
            }

            //n秒強化状態に入る
            strengthenMode = true;
            //炎の演出
            skillEffectManager.StartStrengthenMode();
        }

        if (other.gameObject.CompareTag("RocketLaunchPoint") && leftHandKey && gameManager.usableSkill)
        {
            //ロケット発動
            skillEffectManager.RocketLaunch();

            //Colliderを無効化
            rocketLaunchCollider.enabled = false;
        }
    }

    //右手の移動距離を計算
    private float DistanceCalculationRight(float distance)
    {
        //現在の座標を一旦保存
        Vector3 currentPos = rightHandPos;

        //現在の座標と保存しておいた座標の差分の距離を計算
        float currentDistance = Vector3.Distance(currentPos,previousPosRight);

        //総移動距離に加算
        distance += currentDistance;

        //現在の座標を保存
        previousPosRight = currentPos;

        return distance;
    }

    //左手の移動距離を計算
    private float DistanceCaluculationLeft(float distance)
    {
        //現在の座標を一旦保存
        Vector3 currentPos = leftHandPos;

        //現在の座標と保存しておいた座標の差分の距離を計算
        float currentDistance = Vector3.Distance(currentPos, previousPosLeft);

        //総移動距離に加算
        distance += currentDistance;

        //現在の座標を保存
        previousPosLeft = currentPos;

        return distance;
    }

    //距離を0に初期化する関数
    public void ResetDistance()
    {
        distanceLeft = 0;
        distanceRight = 0;
    }
}
