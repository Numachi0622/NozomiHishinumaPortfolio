using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialExplanationManager01 : MonoBehaviour
{
    public GameObject explanationPanel;
    public GameObject explanationCanvas;

    public PlayerManager playerManager;


    private void Start()
    {
        StartCoroutine(StartExplanation());
    }

    //説明UIを表示する
    IEnumerator StartExplanation()
    {
        yield return new WaitForSeconds(1.3f);
        explanationPanel.SetActive(true);
        explanationCanvas.SetActive(true);
    }

    //説明UIを非表示する関数
    public void EndExplanation()
    {
        explanationPanel.SetActive(false);
        explanationCanvas.SetActive(false);
        playerManager.isPlay = true;
    }
}
