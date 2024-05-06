using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using SimpleInputNamespace;

public class TutorialManager : MonoBehaviour
{
    private int playCount;
    private int enemyHitCount = 0; // 敵に攻撃した回数をカウント
    private int returnCount = 0; // 戻るボタンを押した回数をカウント
    private Vector3 defaultScale = Vector3.one; // fingerのデフォルトのスケール
    private Vector3 inversionScale = new Vector3(-1, 1, 1); // fingerの反転スケール
    [SerializeField] private Image textBackground;
    [SerializeField] private NovelTextManager novelTextManager;
    [SerializeField] private GameObject finger; // 指さしイベントで使用
    [SerializeField] private GameObject checkMark; // チェックマークを表示する際に使用
    [SerializeField] private GameObject checkEffect; // 敵に発生するオーラパーティクル
    [SerializeField] private GameObject arrow; // 移動イベント時の矢印オブジェクト
    [SerializeField] private Button[] buttonActives;
    [SerializeField] private Joystick joystick; // ジョイスティックの有効化/無効化を切り替える
    [SerializeField] private InputField inputField; // InputFieldの有効化/無効化を切り替える
    [SerializeField] private Collider forAttackCollider; // 敵の攻撃を制御するコライダー
    [SerializeField] private EnemyStatus enemyStatus; // チュートリアルの敵のEnemyStatus
    [SerializeField] private SceneTransition sceneTransition; // シーン遷移に関する処理を行うクラス

    public int eventCount { get; private set; } // イベントの発生回数をカウント

    private void Start()
    {
        eventCount = 0;
        playCount = PlayerPrefs.GetInt("PlayCount",0);

        finger.SetActive(false);
        // 全てのボタンを無効化する
        foreach (var image in buttonActives)
        {
            image.enabled = false;
        }
        // Playerの移動を不可能にする
        joystick.enabled = false;
        // 文字入力をしないようにする
        inputField.enabled = false;
    }

    // イベント番号から最新のイベントを発生させる
    public void Event(int _eventNum)
    {
        eventCount++;
        switch (_eventNum)
        {
            case 3:
                // 指のアニメーション
                FingerAinm(inversionScale, 140, new Vector3(-417, -70, 0), new Vector3(-500, -186, 0));
                novelTextManager.ResumeText();
                break;
            case 4:
                // 指のアニメーション
                FingerAinm(defaultScale, 0, new Vector3(330, 0, 0), new Vector3(630, 0, 0));
                novelTextManager.ResumeText();
                break;
            case 5:
                ArrowAnim();
                novelTextManager.ResumeText();
                break;
            case 6:
                // ジョイスティックを有効化
                PlayerMovable(true);
                TextBackgroundAnim(false);
                break;
            case 8:
                // タスク追加ボタンの有効化
                //buttonActives[0].enabled = true;
                FingerAinm(defaultScale,0,new Vector3(800,-109,0),new Vector3(800,-9,0));
                novelTextManager.ResumeText();
                break;
            case 9:
                FingerAinm(defaultScale, 40, new Vector3(230, 174, 0), new Vector3(166, 254, 0));
                novelTextManager.ResumeText();
                break;
            case 11:
                inputField.enabled = true;
                novelTextManager.ResumeText();
                break;
            case 12:
                inputField.enabled = false;
                // 保存ボタンの有効化
                //buttonActives[1].enabled = true;
                FingerAinm(defaultScale, 0, new Vector3(27, 70, 0), new Vector3(27,117, 0));
                novelTextManager.ResumeText();
                break;
            case 14:
                // 戻るボタンの有効化
                //buttonActives[2].enabled = true;
                FingerAinm(defaultScale, 0, new Vector3(625, 245, 0), new Vector3(625, 295, 0));
                novelTextManager.ResumeText();
                break;
            case 18:
                // 攻撃ボタンの有効化
                buttonActives[3].enabled = true;
                FingerAinm(inversionScale, 150, new Vector3(856, -123, 0), new Vector3(780, -215, 0));
                novelTextManager.ResumeText();
                break;
            case 19:
                PlayerMovable(true);
                TextBackgroundAnim(false);
                break;
            case 21:
                // タスク追加ボタンの有効化
                //buttonActives[0].enabled = true;
                FingerAinm(defaultScale, 0, new Vector3(800, -109, 0), new Vector3(800, -9, 0));
                novelTextManager.ResumeText();
                break;
            case 22:
                //buttonActives[4].enabled = true;
                FingerAinm(defaultScale, 0, new Vector3(-503, -86, 0), new Vector3(-503, -29, 0));
                novelTextManager.ResumeText();
                break;
            case 26:
                // 戻るボタンの有効化
                //buttonActives[2].enabled = true;
                FingerAinm(defaultScale, 0, new Vector3(625, 245, 0), new Vector3(625, 295, 0));
                novelTextManager.ResumeText();
                break;
            case 31:
                // コライダーを有効化し、敵が攻撃できる状態にする
                forAttackCollider.gameObject.SetActive(true);
                // プレイヤーの移動、攻撃を有効化
                PlayerMovable(true);
                buttonActives[3].enabled = true;
                TextBackgroundAnim(false);
                break;
            case 39:
                playCount++;
                PlayerPrefs.SetInt("PlayCount",playCount);
                PlayerPrefs.DeleteKey("HP");
                TextBackgroundAnim(false);
                sceneTransition.FadeOut();
                break;
        }
    }

    // 指のアニメーション
    public void FingerAinm(Vector3 _scale, float _rot, Vector3 _firstPos, Vector3 _endPos)
    {
        finger.SetActive(true);
        finger.transform.transform.localScale = _scale;
        finger.transform.localRotation = Quaternion.Euler(0,0,_rot);
        finger.transform.localPosition = _firstPos;
        finger.transform.DOLocalMove(_endPos, 0.5f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }

    // テキストの背景アニメーション
    public void TextBackgroundAnim(bool _isStart)
    {
        // true => 表示するとき　false => 非表示にするとき
        if (_isStart) textBackground.gameObject.transform.DOScale(1, 0.5f).SetEase(Ease.OutBack);
        else textBackground.gameObject.transform.DOScale(0, 0.5f).SetEase(Ease.InBack).
                OnComplete(() => novelTextManager.DisplayButtonTapSign(false));
    }

    // 指を非表示にする
    public void FingerHide()
    {
        if(finger.activeSelf) finger.SetActive(false);
        finger.transform.DOKill();
    }

    // プレイヤーの移動を制限
    public void PlayerMovable(bool _able)
    {
        if (_able) joystick.enabled = true;
        else joystick.enabled = false;
    }

    // ボタンを押した後にそのボタンを無効化する
    public void ButtonEnabled(Button _button)
    {
        _button.enabled = false;
    }

    public void ButtonEnabled(int _index)
    {
        buttonActives[_index].enabled = true; 
    }

    // Playerのアニメーションを再開
    public void PlayerAnimationStart(Animator _animator)
    {
        _animator.enabled = true;
    }

    // 敵に攻撃したときに発生するイベント
    public void AttackEvent()
    {
        if (enemyHitCount > 0) return;
        // 最初の一回のみ発生させる
        enemyHitCount++;
        // コライダーを無効化し、敵が攻撃できない状態にする
        forAttackCollider.gameObject.SetActive(false);
        // プレイヤーの移動、攻撃を無効化
        PlayerMovable(false);
        buttonActives[3].enabled = false;
        TextBackgroundAnim(true);
        novelTextManager.Resume();
    }

    // チェックボックスを押したときに発生するイベント
    public void CheckBoxEvent(Button _button)
    { 
        checkMark.SetActive(true);
        novelTextManager.OnClickNextButton(_button);
    }

    // 戻るボタンを押したときに発生するイベント
    public void ReturnButtonEvent(Button _button)
    {
        if (returnCount > 0)
        {
            checkEffect.SetActive(true);
            enemyStatus.isChecked = true;
        }
        returnCount++;
        novelTextManager.OnClickNextButton(_button);
    }

    // 敵を倒したときのイベント
    public void KnockDownEvent()
    {
        TextBackgroundAnim(true);
        novelTextManager.Resume();
        PlayerMovable(false);
    }

    // 移動イベント時の矢印アニメーション
    private void ArrowAnim()
    {
        if (!arrow.activeSelf) arrow.SetActive(true);
        arrow.transform.DOLocalMoveY(0.5f,0.5f).SetEase(Ease.InQuart).SetLoops(-1,LoopType.Yoyo);
    }
    // イベント終了時、矢印を非表示にする
    public void ArrowHide()
    {
        arrow.transform.parent.gameObject.SetActive(false);
    }

    public void TextEndAnim(GameObject _obj)
    {
        _obj.transform.DOLocalMoveY(-155,0.5f).SetEase(Ease.Linear).SetLoops(-1,LoopType.Yoyo);
    }

    public void ButtonAnim(Transform _tf)
    {
        _tf.DOScale(Vector3.one * 1.3f,0.2f).SetLoops(-1,LoopType.Yoyo);
    }
}
