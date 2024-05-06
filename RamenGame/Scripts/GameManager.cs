using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

// ゲームの状態を管理するenum
public enum State
{
    Title,      // タイトル
    Animation,  // 客の来店時のアニメーションを再生
    Game,       // 実際に麺を切る
    Result      // リザルトを表示
}

// ゲームの状態を管理するクラス
public class GameManager : MonoBehaviour
{
    [SerializeField] private UIDisplayer uiDisplayer; // UI表示用のUIDisplayerクラス
    [SerializeField] private Collider knifeColider; // ナイフのCollider
    [SerializeField] private Transform ramenTf; // 移動させるラーメンのTransform
    [SerializeField] private GameObject cutTarget; // 切るターゲットオブジェクト（麺）
    [SerializeField] private State state; // 現在の状態を格納
    [SerializeField] private UnityEvent customerEvent; // 客のメソッドをイベントとして呼び出す
    public bool gameStartable { get; private set; } // ゲームを始められるかどうか

    private void Start()
    {
        // 最初はタイトル状態にしておく
        state = State.Title;
    }

    // Animation状態に変化させるメソッド
    public void GoToAnimationState()
    {
        state = State.Animation;
    }

    // Game状態に変化させるメソッド
    public void GoToGameState()
    {
        state = State.Game;
        knifeColider.enabled = true;
    }

    // Result状態に変化させるメソッド
    public void GoToResultState()
    {
        state = State.Result;
        knifeColider.gameObject.SetActive(false);
        cutTarget.SetActive(false);
        uiDisplayer.DisplayStopText(this);
    }
    
    public void SetStartable()
    {
        gameStartable = true;
    }

    // ラーメンを提供するアニメーションメソッド
    public void RamenEvent()
    {
        ramenTf.gameObject.SetActive(true);
        Vector3 deskPos = new Vector3(-3.2f,1,0.5f);
        var sequence = DOTween.Sequence();
        sequence.Append(ramenTf.DOMoveX(-3.2f, 2f).SetDelay(2f))
            .Append(ramenTf.DOMove(new Vector3(-3.2f, 1.5f, -0.5f), 1f).SetEase(Ease.Linear).SetDelay(1f))
            .Append(ramenTf.DOMove(deskPos, 1f).SetEase(Ease.Linear))
            .OnComplete(() => customerEvent?.Invoke());
    }
}
