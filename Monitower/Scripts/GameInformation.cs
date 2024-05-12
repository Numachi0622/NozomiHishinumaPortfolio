using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameInformation : MonoBehaviour
{
    //スキルのレベル変数
    public int coinUpLevel; //コイン獲得量スキルのレベル
    public int goldEnemyProbabilityLevel; //ゴールド敵確率アップスキルのレベル
    public int bossBattleTimeLevel; //ボス戦時間増量スキルのレベル
    public int rocketMagnificationLevel; //レーザーダメ倍率のレベル
    public int powerUpLevel; //パワー上昇のレベル
    public int powerUpTimeLevel; //n秒強化のレベル
    public int rocketNumLevel;//レーザーの回数
    public int weakPointNumLevel; //弱点個数
    public int weakPointMagnificationLevel;//弱点倍率のレベル

    public int numberOfPlays; //総周回数
    public int progress; //進行度
    public int havingTotalCoin; //持ってるコインの合計

    private void Start()
    {
        havingTotalCoin = Refresh("TOTAL_COIN");
        progress = Refresh("PROGRESS");
        numberOfPlays = Refresh("NUMBER_OF_PLAYS");
    }

    //レベル更新時に呼び出す関数
    public int Refresh(string key)
    {
        int value;
        if (key == "NUMBER_OF_PLAYS" || key == "PROGRESS" || key == "TOTAL_COIN")
        {
            value = PlayerPrefs.GetInt(key);
        }
        else
        {
            value = PlayerPrefs.GetInt(key, 1);
        }
        return value;
    }
}
