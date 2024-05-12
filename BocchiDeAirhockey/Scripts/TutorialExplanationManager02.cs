using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialExplanationManager02 : MonoBehaviour
{
    public GameObject explanationPanel;
    public GameObject[] explanationCanvas = new GameObject[2];
    public GameObject[] explanationAnimObj = new GameObject[2];

    public PlayerManager playerManager;


    private void Start()
    {
        StartCoroutine(StartExplanation());
    }

    //最初の説明UIを表示する
    IEnumerator StartExplanation()
    {
        yield return new WaitForSeconds(1.3f);
        explanationPanel.SetActive(true);
        explanationCanvas[0].SetActive(true);
        explanationAnimObj[0].SetActive(true);
    }

    //最初の説明UIを非表示、2番目の説明UIを表示する関数
    public void NextExplanation()
    {
        explanationCanvas[0].SetActive(false);
        explanationAnimObj[0].SetActive(false);
        explanationCanvas[1].SetActive(true);
        explanationAnimObj[1].SetActive(true);

    }

    //2番目のUIを非表示にする関数
    public void EndExplanation()
    {
        explanationPanel.SetActive(false);
        explanationCanvas[1].SetActive(false);
        explanationAnimObj[1].SetActive(false);
        playerManager.isPlay = true;
    }
}
