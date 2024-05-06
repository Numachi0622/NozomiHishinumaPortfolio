using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public bool isClear,isLose;　//ゲームオーバー、クリアを判定するbool
    public bool isPlay; //プレイ可能かを判定するbool
    Vector3 mousePos, worldPos;
    Rigidbody2D rb;
    private void Start()
    {
        isClear = false;
        isLose = false;
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        rb.velocity = new Vector2(0,0);

        //クリア、ゲームオーバー状態でなかったら
        if (!isClear && !isLose)
        {
            //プレイ可能状態、マウスを左クリックしたら（スマホなら画面に触れたら）
            if (isPlay && Input.GetMouseButton(0))
            {
                //画面の触れている位置にプレイヤーを移動（ドラッグ）
                mousePos = Input.mousePosition;
                worldPos = Camera.main.ScreenToWorldPoint(new Vector2(mousePos.x, mousePos.y));
                if (worldPos.x > -2 && worldPos.x < 2)
                {
                    //場外にプレイヤーが行かないよう制御
                    rb.MovePosition(worldPos);
                }
            }
        }
    }
}
