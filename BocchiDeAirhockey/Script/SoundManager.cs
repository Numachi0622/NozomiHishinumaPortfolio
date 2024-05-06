using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    void Start()
    {
        
        if(SceneManager.GetActiveScene().name == "Title" || SceneManager.GetActiveScene().name == "Select")
        {
            //タイトルとセレクトシーンではBGM再生オブジェクトを常備させる（シーン間で破壊されない）
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            //それ以外はBGM再生のオブジェクトは破壊される
            Destroy(this.gameObject);
        }
    }
}
