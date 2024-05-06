using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

// UI���Ǘ�����N���X
public class UIDisplayer : MonoBehaviour
{
    [SerializeField] private CanvasGroup[] canvasGroups; // �^�C�g��UI��CanvasGroup�R���|�[�l���g
    [SerializeField] private GameManager gameManager; // GameManager�N���X
    [SerializeField] private TimeCounter timeCounter; // �������Ԍv���N���X
    [SerializeField] private TextMeshProUGUI countDownText; // 3,2,1�̃J�E���g�_�E��Text
    [SerializeField] private TextMeshProUGUI stopText; // �������ԏI�����ɕ\������Text
    [SerializeField] private TextMeshProUGUI scoreText; // �X�R�A��\������Text
    [SerializeField] private TextMeshProUGUI cutCountText; // �؂����񐔂�\������text
    [SerializeField] private Button retryButton; // ���g���C��Button�R���|�[�l���g
    [SerializeField] private Cut cut; // �؂����񐔂��Z�o����N���X
    [SerializeField] private ScoreManager scoreManager; // �X�R�A���Z�o����N���X
    [SerializeField] private LineRenderer lineRenderer; // �R���g���[���[��lineRenderer�R���|�[�l���g�i���[�U�[�j

    [SerializeField] private AudioClip textSE,countDownSE,startSE,finishSE,scoreSE; // UI�Ɋ֘A����SE

    private TextMeshProUGUI customerText; // �q�̃Z���t��\������TextMeshProUGUI�R���|�[�l���g
    private WaitForSeconds interval = new WaitForSeconds(0.1f); // 0.05�b���ɕ�����\������
    private WaitForSeconds second = new WaitForSeconds(1f); // �J�E���g�_�E���p
    public string score { get; private set; } // �X�R�A�Z�o��Ɋi�[

    // Customer�N���X�̃C���X�^���X������������CustomerText���Z�b�g�����
    public void SetCustomerText(TextMeshProUGUI _text)
    {
        customerText = _text;
    }

    // �^�C�g��UI���\���ɂ��郁�\�b�h
    public void CloseTitleUI(Button _button)
    {
        foreach (var c in canvasGroups)
        {
            if (!c.enabled) c.enabled = true;
            c.DOFade(0, 1.5f);
        }
        lineRenderer.enabled = false;
        _button.enabled = false;
    }

    // ����������\������R���[�`��
    private IEnumerator DisplaySentence(string _sentence)
    {
        foreach (char c in _sentence.ToCharArray())
        {
            // ����������\��
            customerText.text += c;
            SoundManager.instance.PlaySE(textSE);
            yield return interval;
        }
    }

    // �q�̃Z���t��w�i�ƈꏏ�ɕ\�����郁�\�b�h
    public void DisplayCustomerUI(string _sentence, bool _isResult)
    {
        if (customerText.text != null) customerText.text = null;
        customerText.transform.parent.DOScaleX(1,1)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                StartCoroutine(DisplaySentence(_sentence));
                customerText.transform.parent.DOScaleX(0, 1)
                .SetDelay(3f)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    if (_isResult)
                    {
                        DisplayScoreText();
                        return;
                    }
                    gameManager.SetStartable();
                });
            });
    }

    // �J�E���g�_�E���̃R���[�`���Ăяo�����\�b�h
    public void DisplayCountDown()
    {
        if (!gameManager.gameStartable) return;
        StartCoroutine(CountDown());
    }

    // �؂�n�߂�O�̃J�E���g�_�E���̃R���[�`��
    private IEnumerator CountDown()
    {
        for (int i = 3; i >= 0; i--)
        {
            yield return second;
            if (i == 0)
            {
                countDownText.enabled = false;
                timeCounter.enabled = true;
                gameManager.GoToGameState();
                SoundManager.instance.PlaySE(startSE);
                SoundManager.instance.ResumeBGM();
            }
            else
            {
                countDownText.text = i.ToString();
                SoundManager.instance.PlaySE(countDownSE);
            }
        }
    }

    // �������Ԃ��������̃e�L�X�g��\�����郁�\�b�h
    public void DisplayStopText(GameManager _gameManager)
    {
        Vector3 defaultScale = stopText.transform.localScale;
        stopText.transform.localScale = defaultScale * 1.2f;
        var sequence = DOTween.Sequence();
        sequence.Append(stopText.transform.DOScale(defaultScale, 1f).SetEase(Ease.InCirc))
            .Join(stopText.GetComponent<CanvasGroup>()?.DOFade(1, 1f))
            .OnComplete(() =>
            {
                stopText.GetComponent<CanvasGroup>()?.DOFade(0, 3f).SetDelay(3f);
                _gameManager.RamenEvent();
                score = scoreManager.Score();
                SoundManager.instance.PlaySE(finishSE);
            });
    }

    // �X�R�A��\�����郁�\�b�h
    private void DisplayScoreText()
    {
        scoreText.text = score + "�����N";
        Vector3 defaultScale = scoreText.transform.localScale;
        scoreText.transform.localScale = defaultScale * 1.5f;
        var sequence = DOTween.Sequence();
        sequence.Append(scoreText.transform.DOScale(defaultScale, 2f).SetEase(Ease.InCirc).SetDelay(1f))
            .Join(scoreText.GetComponent<CanvasGroup>()?.DOFade(1, 2f))
            .OnComplete(() => 
            { 
                SoundManager.instance.PlaySE(scoreSE);
                DisplayRetryButton();
                DisplayCutCount();
            });
    }

    // ���g���C�{�^����\�����郁�\�b�h
    private void DisplayRetryButton()
    {
        if(!retryButton.gameObject.activeSelf) retryButton.gameObject.SetActive(true);
        lineRenderer.enabled = true;
        retryButton.GetComponent<CanvasGroup>()?.DOFade(1,1.5f)
            .OnComplete(() => retryButton.GetComponent<CanvasGroup>().enabled = false);
    }

    // �؂����񐔂�\�����郁�\�b�h
    private void DisplayCutCount()
    {
        if(!cutCountText.gameObject.activeSelf) cutCountText.gameObject.SetActive(true);
        cutCountText.text = cut.count.ToString() + "��؂�܂���";
    }
}
