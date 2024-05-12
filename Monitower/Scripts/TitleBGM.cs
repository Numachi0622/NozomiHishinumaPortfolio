using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleBGM : MonoBehaviour
{
    [SerializeField] GameManager gameManager;

    public void PlayBGM()
    {
        gameManager.TitleBGMPlay();
    }
}
