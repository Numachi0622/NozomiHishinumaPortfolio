using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;

/// <summary>ゲーム終了時の演出</summary>
public class GameEndView : MonoBehaviour {

    /// <summary>
    /// 画面全体に半透明の色を付けるImage
    /// </summary>
    [SerializeField] private Image _layer;

    /// <summary>
    /// ゲームクリア/オーバーを表示するテキスト
    /// </summary>
    [SerializeField] private TextMeshProUGUI _text;

    /// <summary>
    /// リスタートボタン
    /// </summary>
    [SerializeField] private Button _restartButton;
    public Button RestartButton => _restartButton;

    /// <summary>
    /// 負け演出の背景カラー
    /// </summary>
    [SerializeField] private Color _loseBgColor;

    /// <summary>
    /// テキストのカラー
    /// </summary>
    [SerializeField] private Color[] _textColor;

    /// <summary>
    /// ゲーム終了演出のSequence;
    /// </summary>
    private Sequence _gameEndSequence;

    public void Play(EndType type)
    {
        _layer.gameObject.SetActive(true);
        
        if (type == EndType.Win)
        {
            PlayWinEffect().Forget();
            return;
        }
        
        LoseEffect().Forget();
    }
    
    /// <summary>
    /// 勝利演出を再生
    /// </summary>
    private async UniTaskVoid PlayWinEffect()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(1f));
        
        _text.text = "ゲームクリア！";
        _text.transform.localScale = Vector3.zero;
        _text.gameObject.SetActive(true);
        
        _gameEndSequence?.Kill();

        _gameEndSequence = DOTween.Sequence()
            .Append(_text.transform.DOScale(Vector3.one, 1f).SetEase(Ease.OutBack))
            .AppendCallback(() => _restartButton.gameObject.SetActive(true));
    }

    /// <summary>
    /// 負け演出を再生
    /// </summary>
    private async UniTaskVoid LoseEffect()
    {
        _text.text = "ゲームオーバー";
        
        await UniTask.Delay(TimeSpan.FromSeconds(2f));
        
        _text.gameObject.SetActive(true);
        _text.color = _textColor[1];
        _layer.color = _loseBgColor;
        _restartButton.gameObject.SetActive(true);
    }
}
