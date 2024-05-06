using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialExplanationManager03 : MonoBehaviour
{
    public GameObject explanationPanel;
    public GameObject explanationCanvas;
    public GameObject explanationAnimObj;

    public PlayerManager playerManager;
    private void Start()
    {
        StartCoroutine(StartExplanation());
    }

    //説明UIを表示
    IEnumerator StartExplanation()
    {
        yield return new WaitForSeconds(1.3f);
        explanationPanel.SetActive(true);
        explanationCanvas.SetActive(true);
        explanationAnimObj.SetActive(true);
    }

    //説明UIを非表示にする関数
    public void EndExplanation()
    {
        explanationPanel.SetActive(false);
        explanationCanvas.SetActive(false);
        explanationAnimObj.SetActive(false);
        playerManager.isPlay = true;
    }
}
