using UnityEngine;
using TMPro;

// �������Ԍv���p�N���X
public class TimeCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText; // �������ԕ\���pText
    [SerializeField] private GameManager gameManager; // GameManager�N���X
    private float limitTime = 10f; // ��������
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
