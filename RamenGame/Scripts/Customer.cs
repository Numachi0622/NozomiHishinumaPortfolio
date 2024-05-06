using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

// 客の処理をまとめたクラス
public class Customer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI myText; // 自身のセリフを表示するtext
    [SerializeField] private GameObject chopsticks; // 客が持つ箸オブジェクト
    [SerializeField] private AudioClip ramenSE; // ラーメンをすする効果音
    private UIDisplayer uiDisplayer; // UIDisplayerクラス
    private ScoreManager scoreManager; // スコアを算出するクラス
    private Animator animator; // 自身のAnimatorコンポーネント
    private Vector3 sittingPos = new Vector3(-3.2f, 0.2f, 1.26f); // 座るとき自身の座標
    private Quaternion sittingRot = Quaternion.Euler(0, -180, 0); // 座るときの自身のQuaternion

    [SerializeField] private string orderLine; // 注文するときの客のセリフ
    [SerializeField] private string[] feedback = new string[6]; // 評価のセリフ
    private string[] scoreValue = { "S", "A", "B", "C", "D", "E" }; // スコア
    private Dictionary<string, string> resultLine = new Dictionary<string, string>(); // 評価のセリフをスコアと対応付ける

    private void Awake()
    {
        // コンポーネントを取得
        animator = GetComponent<Animator>();
        uiDisplayer = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIDisplayer>();
        scoreManager = GameObject.FindGameObjectWithTag("Canvas").GetComponent<ScoreManager>();

        // 自身のTextコンポーネントを渡す
        uiDisplayer.SetCustomerText(myText);
    }
    private void Start()
    {
        // Dictionaryに代入
        for(int i = 0; i < feedback.Length; i++)
        {
            resultLine[scoreValue[i]] = feedback[i];
        }
        WalkToSittingPosition();
    }

    // 椅子に座るまでの歩行アニメーションを実行するメソッド
    public void WalkToSittingPosition()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(transform.DOMoveX(sittingPos.x, 4f).SetEase(Ease.Linear))
            .Append(transform.DORotateQuaternion(sittingRot, 0.5f))
            .Append(transform.DOMove(sittingPos, 1f))
            .OnComplete(() =>
            {
                animator.SetTrigger("Sit");
                uiDisplayer.DisplayCustomerUI(orderLine,false);
            });
    }

    // ラーメンを食べるアニメーションメソッド
    public void EatAnimation()
    {
        chopsticks.SetActive(true);
        chopsticks.transform.DOLocalMove(Vector3.up * 0.1f, 0.5f)
            .SetEase(Ease.Linear)
            .SetLoops(4, LoopType.Yoyo)
            .SetDelay(2f)
            .OnComplete(() =>
            {
                chopsticks.SetActive(false);
                uiDisplayer.DisplayCustomerUI(resultLine[scoreManager.Score()],true);
            });
        SoundManager.instance.PlaySE(ramenSE);
    }
}
