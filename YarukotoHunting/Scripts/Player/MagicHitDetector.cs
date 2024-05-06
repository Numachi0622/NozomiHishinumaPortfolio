using UnityEngine;
using UnityEngine.UI;

public class MagicHitDetector : MonoBehaviour
{
    private int left, right;
    private int answer;
    public int Answer { get => answer; }
    [SerializeField] private GameObject homeworkPanel;
    [SerializeField] private Text formula;
    [SerializeField] private Calculator calculator;
    [SerializeField] private PlayerStatus playerStatus;
    [SerializeField] private BossMove bossMove;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Homework")) return;
        if (!homeworkPanel.activeSelf)
        {
            homeworkPanel.SetActive(true);
            HomeworkEvent(1);
        }
        calculator.DisplayCalculator();
        // マヒ状態に遷移
        playerStatus.GoToParalysisState();
        // 距離に関係なく敵が近づいてくる
        bossMove.IdentifyPlayer(true);
    }
    public void HomeworkEvent(int _phase)
    {
        left = Random.Range(1,101);
        right = Random.Range(1,101);
        switch(_phase){
            case 1:
                answer = left + right;
                formula.text = left + " + " + right + " = ";
                break;
            case 2:
                if (left > right)
                {
                    answer = left - right;
                    formula.text = left + " - " + right + " = ";
                }
                else
                {
                    answer = right - left;
                    formula.text = right + " - " + left + " = ";
                }
                break;
            case 3:
                right = Random.Range(1,11);
                answer = left * right;
                formula.text = left + " × " + right + " = ";
                break;
        }
    }
}
