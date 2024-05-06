using UnityEngine;
using TMPro;

// 制限時間計測用クラス
public class TimeCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText; // 制限時間表示用Text
    [SerializeField] private GameManager gameManager; // GameManagerクラス
    private float limitTime = 10f; // 制限時間
    private void Update()
    {
        limitTime -= Time.deltaTime;
        timeText.text = limitTime.ToString("F1");
        if(limitTime <= 0)
        {
            timeText.enabled = false;
            gameManager.GoToResultState();
            this.enabled = false;
        }
    }
}
