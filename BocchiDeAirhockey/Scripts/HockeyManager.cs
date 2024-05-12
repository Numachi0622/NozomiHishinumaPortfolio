using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HockeyManager : MonoBehaviour
{
    public GameObject clearDetecter;    //クリア判定をするオブジェクト
    public GameObject loseDetecter;     //負け判定をするオブジェクト
    public GameObject gameDirector;
    public GameObject player;

    public Material[] color = new Material[3];  //0:red 1:blue 2:green
    public GameObject[] effectObj = new GameObject[3];  //レーザーに触れた時のパーティクルオブジェクト
    int colorNum;   //色を識別する変数
    int red = 0;
    int blue = 1;
    int green = 2;

    public AudioClip reflectionSE1;　//壁、ブロックなどに触れた際のSE
    public AudioClip reflectionSE2; //プレイヤーに触れた際のSE
    public AudioClip razerHitSE;    //レーザーヒット時のSE
    public AudioClip switchSE;      //スイッチに触れた時のSE
        
    AudioSource audioSource;

    Rigidbody2D hockeyRb;

    void Start()
    {
        hockeyRb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        colorNum = -1;  //初期は-1に初期化しておく
    }

    void FixedUpdate()
    {
        SpeedControll();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //クリア時の処理
        if(collision.gameObject == clearDetecter)
        {
            //クリア時のUIを表示
            gameDirector.GetComponent<GameDirector>().ShowClearUI(true);

            //玉を消す
            Destroy(this.gameObject);

            //PlayerManagerにクリアしたことを通知
            player.GetComponent<PlayerManager>().isClear = true;
        }
        //ゲームオーバー時の処理
        else if(collision.gameObject == loseDetecter)
        {
            //ゲームオーバー時のUIを表示
            gameDirector.GetComponent<GameDirector>().ShowLoseUI(true);

            //玉を消す
            Destroy(this.gameObject);

            //PlayerManagerにゲームオーバーを通知
            player.GetComponent<PlayerManager>().isLose = true;
        }

        //レッドレーザーに触れた時の処理
        if(collision.gameObject.tag == "RedRazer")
        {
            if (colorNum != red)
            {
                colorNum = red;
                ChangeColor(red);
                PlayEffect(red);
            }
        }
        //ブルーレーザーに触れた時の処理
        else if(collision.gameObject.tag == "BlueRazer")
        {
            if (colorNum != blue)
            {
                colorNum = blue;
                ChangeColor(blue);
                PlayEffect(blue);
            }
        }
        //グリーンレーザーに触れた時の処理
        else if(collision.gameObject.tag == "GreenRazer")
        {
            if (colorNum != green)
            {
                colorNum = green;
                ChangeColor(green);
                PlayEffect(green);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            //プレイヤーに触れたらSEを鳴らす
            audioSource.PlayOneShot(reflectionSE2);
        }
        else
        {
            //壁などに触れたらSEを鳴らす
            audioSource.PlayOneShot(reflectionSE1);
        }

        if (collision.gameObject.tag == "RedBlock" && colorNum == red)
        {
            //レッドブロックを消す
            Destroy(collision.gameObject);
        }
        if(collision.gameObject.tag == "BlueBlock" && colorNum == blue)
        {
            //ブルーブロックを消す
            Destroy(collision.gameObject);
        }
        if(collision.gameObject.tag == "GreenBlock" && colorNum == green)
        {
            //グリーンブロックを消す
            Destroy(collision.gameObject);
        }

        if(collision.gameObject.tag == "RedSwitch" && colorNum == red)
        {
            //レッドスイッチをON/OFF
            SwitchOnOff(collision.gameObject);
        }
        if(collision.gameObject.tag == "BlueSwitch" && colorNum == blue)
        {
            //ブルースイッチをON/OFF
            SwitchOnOff(collision.gameObject);
        }
        if(collision.gameObject.tag == "GreenSwitch" && colorNum == green)
        {
            //グリーンスイッチをON/OFF
            SwitchOnOff(collision.gameObject);
        }
    }

    //玉の色を帰る関数
    void ChangeColor(int colorNum)
    {
        //引数を配列の要素番号とし、マテリアルの配列を割り当てる
        this.GetComponent<Renderer>().material = color[colorNum];
    }

    //レーザーヒット時のパーティクル再生関数
    void PlayEffect(int colorNum)
    {
        //引数を配列の要素番号とし、パーティクルの配列からParticleSystemを取得する
        ParticleSystem particleSystem = effectObj[colorNum].GetComponent<ParticleSystem>();
        particleSystem = Instantiate(particleSystem);
        particleSystem.transform.position = this.transform.position;
        particleSystem.Play(); 
        audioSource.PlayOneShot(razerHitSE);
    }

    //スイッチをON/OFFにする関数
    void SwitchOnOff(GameObject collision)
    {
        //引数のSwitchManagerを取得
        SwitchManager switchManager = collision.GetComponent<SwitchManager>();
        if (switchManager.isHit == false)
        {
            //OFF状態で触れたらONにする
            switchManager.isHit = true;
            switchManager.Switch(switchManager.isHit);
        }
        else
        {
            //ON状態で触れたらOFFにする
            switchManager.isHit = false;
            switchManager.Switch(switchManager.isHit);
        }
        //スイッチヒット時のSEwo鳴らす
        audioSource.PlayOneShot(switchSE);
    }

    //玉を初期値に戻す関数
    public void ResetPosition()
    {
        hockeyRb.velocity = Vector3.zero;
        this.gameObject.transform.position = new Vector3(0, -2.93f, 0);
    }

    //Ver.1.0.3で追加
    //速度制限関数
    void SpeedControll()
    {
        float velocityX = hockeyRb.velocity.x;
        float velocityY = hockeyRb.velocity.y;
        const float MAX_VELOCITY = 10;　//速度の最大値
        if(hockeyRb.velocity.magnitude > MAX_VELOCITY)
        {
            //最大値以上にならないように制御
            hockeyRb.velocity = hockeyRb.velocity.normalized * MAX_VELOCITY;
        }
    }
    
}
