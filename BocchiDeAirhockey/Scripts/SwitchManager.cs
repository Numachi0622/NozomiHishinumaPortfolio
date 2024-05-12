using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchManager : MonoBehaviour
{
    public Material switchOnMat;
    public Material switchOffMat;

    GameObject switchLight;
    GameObject switchChild;

    public bool isHit;

    private void Start()
    {
        switchLight = transform.GetChild(0).gameObject;
        switchChild = transform.GetChild(1).gameObject;

    }

    //引数のtrue/falseによって中のスイッチを表示/非表示する関数
    public void Switch(bool isHit)
    {
        if (isHit == true)
        {
            //ON状態でスイッチを赤く点灯
            switchLight.GetComponent<Renderer>().material = switchOnMat;
        }
        else
        {
            //OFF状態でスイッチの点灯を停止
            switchLight.GetComponent<Renderer>().material = switchOffMat;
        }

        //白いスイッチオブジェクトを表示/非表示
        switchChild.SetActive(isHit);
    }
}
