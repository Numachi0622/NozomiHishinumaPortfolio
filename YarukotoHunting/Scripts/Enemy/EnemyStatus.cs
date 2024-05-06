using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStatus : StatusManager
{
    private WaitForSeconds attackDelayTime = new WaitForSeconds(2.5f); // 攻撃時のクールタイム
    private WaitForSeconds normalDelayTime = new WaitForSeconds(0.5f);
    private WaitForSeconds dieDelayTime = new WaitForSeconds(3);
    private ExperiencePointSystem expSystem;
    private PlayerStatus playerStatus;
    private SearchTarget searchTarget;
    private UIAnimation UIAnim;
    private FinishTaskSound taskFinishSound;
    private Vector3 expDefaulPos = new Vector3(0, 0.5f, 0);
    private Vector3 expUpPos = new Vector3(0,3,-1f);
    [SerializeField] private Image checkMark; // 倒したときに表示するチェックマーク
    [SerializeField] private bool isTutorial;

    public bool isChecked; // Taskが完了しているか
    public string key; // どの敵かを判別するキー

    [SerializeField] private Collider forAttackCollider;
    [SerializeField] private Collider forMoveCollider;
    [SerializeField] private Collider attackCollider;
    [SerializeField] private Collider damageCollider;
    [SerializeField] private Text taskNameText;
    [SerializeField] private GameObject checkedPerticle;
    [SerializeField] private GameObject expParticle;
    [SerializeField] private TutorialManager tutorialManager;

    protected override void Awake()
    {
        base.Awake();
        playerStatus = GameObject.FindGameObjectWithTag("PlayerStatus").GetComponent<PlayerStatus>();
        searchTarget = GameObject.FindWithTag("Searcher").GetComponent<SearchTarget>();
        expSystem = GameObject.FindGameObjectWithTag("ExpSystem").GetComponent<ExperiencePointSystem>();
        UIAnim = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIAnimation>();
        if (damageSource == null)
            damageSource = GameObject.FindGameObjectWithTag("EnemyDamageSE").GetComponent<AudioSource>();
        taskFinishSound = GameObject.FindGameObjectWithTag("BGM").GetComponent<FinishTaskSound>();
    }

    private void OnEnable()
    {
        hp = maxHp;
        hpGauge.SetHP(hp,maxHp);
        // DIE => NORMALへは遷移できないがここでは無理やり遷移させる
        state = STATE.NORMAL;
        // オブジェクトプールで再び生成された際のCollider回復処理
        attackCollider.gameObject.SetActive(true);
        forAttackCollider.enabled = true;
        forMoveCollider.enabled = true;
        damageCollider.enabled = true;
        expParticle.transform.localPosition = expDefaulPos;
        expParticle.SetActive(false);
        checkMark.fillAmount = 0;
    }

    // Colliderを一定の間隔でアクティブにすることで攻撃のクールタイムを生み出す
    public void AttackDelay()
    {
        StartCoroutine(AttackDelayCoroutine());
    }

    IEnumerator AttackDelayCoroutine()
    {
        forAttackCollider.enabled = false;
        yield return attackDelayTime;
        if (base.state != STATE.DIE) forAttackCollider.enabled = true;
    }

    IEnumerator NormalStateDelayCoroutine()
    {
        yield return normalDelayTime;
        base.GoToNormalState();
    }

    public override void GoToAttackState()
    {
        base.GoToAttackState();
        // 0.5秒後に強制的にノーマル状態へ遷移
        StartCoroutine(NormalStateDelayCoroutine());
    }

    public override void GoToDieState()
    {
        base.GoToDieState();
        AllColliderOff();
        searchTarget.ClearTarget(transform);
        // チェックマークの演出
        checkedPerticle.SetActive(false);
        checkMark.DOFillAmount(0.35f, 1).OnComplete(() =>
        {
            checkMark.DOFillAmount(1, 0.7f).OnComplete(() =>
            {
                animator.SetTrigger("Die");
                expSystem.ExpEffect(expParticle);
            });
        });
        taskFinishSound.Play();
        // 経験値を上昇させる
        expParticle.SetActive(true);
        expParticle.transform.DOLocalMove(expUpPos, 0.7f).OnComplete(() =>
        {
            playerStatus.Dance();
            UIAnim.CompleteMessageAnim(gameObject.name);
        });
        StartCoroutine(DieDelayCoroutine());

        if (isTutorial)
        {
            // チュートリアルでのみ実行

            tutorialManager.KnockDownEvent();
            // 経験値取得の演出が全て終わるまで経験値ゲージを更新しない
            // チュートリアル時は経験値固定(1.5倍)
            DOVirtual.DelayedCall(3.7f, () => { expSystem.GetExp(expSystem.baseExp * 1.5f); });
        }
        else
        {
            // 本番ゲームでのみ実行
            for (int i = 0; i < SaveSystem.Instance.TaskList.taskList.Count; i++)
            {
                if (SaveSystem.Instance.TaskList.taskList[i].key == key)
                {
                    float elpsedTime = expSystem.ElapsedTimeCalculation(SaveSystem.Instance.TaskList.taskList[i].registeredTime);
                    // 経験値取得の演出が全て終わるまで経験値ゲージを更新しない
                    DOVirtual.DelayedCall(3.7f, () => { expSystem.GetExp(elpsedTime); });
                    // 倒した敵のタスクリストを削除
                    SaveSystem.Instance.TaskList.taskList.RemoveAt(i);
                    SaveSystem.Instance.Save();
                    break;
                }
            }
        }
    }
    IEnumerator DieDelayCoroutine()
    {
        yield return dieDelayTime;
        gameObject.SetActive(false);
    }

    public override float GetDamage()
    {
        // チェックが入ってるときは2倍の倍率、入ってないときは1の固定ダメージを返す
        if (isChecked) return damage * 2;
        else return 1;
    }

    // チェックされている判定のパーティクルを再生
    public void IsChecked(bool _isChecked)
    {
        isChecked = _isChecked;
        checkedPerticle.SetActive(isChecked);
    }

    // Dieアニメーション再生時に呼ばれる
    public void AllColliderOff()
    {
        forAttackCollider.enabled = false;
        forMoveCollider.enabled = false;
        attackCollider.gameObject.SetActive(false);
        damageCollider.enabled = false;
    }

    // HPゲージの上にタスクの名前を表示
    public void DisplayTaskName(string _name)
    {
        taskNameText.text = _name;
    }
}
