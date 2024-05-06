using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Calculator : MonoBehaviour
{
    private int numberOfCorrect = 0;
    private Vector3 defaultPos = new Vector3(600,-770,0);
    private Vector3 displayPos = new Vector3(600,-270,0);
    [SerializeField] private Text answerText;
    [SerializeField] private MagicHitDetector magicHitDetector;
    [SerializeField] private PlayerStatus playerStatus;
    [SerializeField] private BossMove bossMove;
    [SerializeField] private GameObject correct, incorrect;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip correctSE, incorrectSE;

    public void DisplayCalculator(bool _isDisplay = true)
    {
        if (_isDisplay)
            transform.DOLocalMove(displayPos,1f);
        else
            transform.DOLocalMove(defaultPos,1f);
    }

    // ボタンを押して値を入力
    public void Input(int num)
    {
        if (num == 0 && answerText.text == "") return;
        answerText.text += num.ToString();
    }

    // 提出ボタンを押したときの処理
    public void Submit()
    {
        if(magicHitDetector.Answer.ToString() == answerText.text)
        {
            answerText.text = "";
            numberOfCorrect++;
            magicHitDetector.HomeworkEvent(numberOfCorrect + 1);
            DisplayJudge(correct);
            audioSource.PlayOneShot(correctSE);
            if (numberOfCorrect >= 3)
            {
                DisplayCalculator(false);
                answerText.transform.parent.gameObject.SetActive(false);
                playerStatus.RecoveryToNormal();
                bossMove.IdentifyPlayer(false);
                numberOfCorrect = 0;
            }
            return;
        }
        answerText.text = "";
        DisplayJudge(incorrect);
        audioSource.PlayOneShot(incorrectSE);
    }

    // 数字を一文字消す
    public void Erase()
    {
        if (answerText.text.Length == 0) return;
        answerText.text = answerText.text.Remove(answerText.text.Length - 1);
    }

    private void DisplayJudge(GameObject _obj)
    {
        _obj.SetActive(true);
        DOVirtual.DelayedCall(0.5f, () => _obj.SetActive(false));
    }
}
