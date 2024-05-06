using UnityEngine;
using DG.Tweening;

public class PlayerStatus : StatusManager
{
    private bool IsDashAttackable => searchTarget.distance > dashAttackPossibleBoder; // ダッシュアタック可能条件
    private float dashAttackPossibleBoder = 2f; // ダッシュアタック可能な距離

    [SerializeField] private Collider attackCollider;
    [SerializeField] private Collider damageCollider;
    [SerializeField] private Collider playerCollider;
    [SerializeField] private Transform cameraTf;
    [SerializeField] private SearchTarget searchTarget;
    [SerializeField] private PlayerController controller;
    [SerializeField] private ExperiencePointSystem expSystem;
    [SerializeField] private UIAnimation UIAnim;
    [SerializeField] private bool isBoss;

    [SerializeField] private GameObject buffParticle;
    [SerializeField] private GameObject recoverParticle;

    [SerializeField] private AudioSource jumpSource,itemSource;
    [SerializeField] private AudioClip[] jumpSE = new AudioClip[2]
        ,itemSE = new AudioClip[2];
    private bool isPowerUp;
    private float powerUpTime = 10;

    private void Start()
    {
        StatusSettingFromRank(expSystem.Rank);
        if (isBoss)
            hp = maxHp;
        else
            hp = PlayerPrefs.GetFloat("HP",maxHp);
        hpGauge.SetHP(hp,maxHp);
    }
    // プレイヤーのステータスをランクに応じて設定する
    public void StatusSettingFromRank(int _rank, bool _isRecover = false)
    {
        maxHp = baseHp + 10 * (_rank - 1);
        hp = maxHp;
        attack = baseAttack + (_rank - 1);
        if (!_isRecover) return;
        hpGauge.RecoverHP(maxHp);
        PlayerPrefs.SetFloat("HP", maxHp);
    }

    public override void ReceiveDamage()
    {
        if (state == STATE.DIE) return;
        hp -= GetDamage();
        PlayerPrefs.SetFloat("HP",hp);
        hpGauge.DecreaseHP(hp);
        damageText.AppearDamageText(GetDamage());
        damageSource.PlayOneShot(damageSE);
        if (state != STATE.PARALYSIS) animator.SetTrigger("Damage");

        // HPが0になったときの処理
        if (hp <= 0) GoToDieState();
    }

    public override void GoToAttackState()
    {
        if (!isAttackable) return;
        if (searchTarget.inArea)
        {
            // 近くにいる敵の方向を向く
            Vector3 direction = searchTarget.targetPos - transform.position;
            direction.y = 0;
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.DORotateQuaternion(targetRot,0.1f).OnComplete(() => controller.IsDashAttacking());
            if (IsDashAttackable)
            {
                // 一定距離内であればダッシュアタックする
                Vector3 destination = transform.position + direction.normalized;
                transform.DOMove(destination, 0.1f).SetEase(Ease.Linear);
            }
        }
        base.GoToAttackState();
    }
    public override void GoToDieState()
    {
        base.GoToDieState();
        damageCollider.enabled = false;
        playerCollider.enabled = false;
        animator.SetTrigger("Die");
        UIAnim.GameOverAnim();
    }
    public void AttackColliderRefresh()
    {
        if (base.state == STATE.DIE) return;
        if(!attackCollider.enabled) attackCollider.enabled = true;
    }

    // 敵を倒したとき、ランクアップ時のアニメーション
    public void Dance(int _danceNum = 1,bool _isLoop = false)
    {
        damageCollider.enabled = false;
        // 引数に応じてダンスのアニメーショントリガーを変更
        string trigger;
        switch (_danceNum)
        {
            case 1:
                trigger = "Dance"; break;
            case 2:
                trigger = "RankUp"; break;
            default:
                trigger = "Dance"; break;
        }
        // カメラの方を向く
        Vector3 direction = new Vector3(cameraTf.position.x, transform.position.y, cameraTf.position.z) - transform.position;
        Quaternion targetRot = Quaternion.LookRotation(direction);
        transform.DORotateQuaternion(targetRot,0.5f).OnComplete(() =>
        {
            state = STATE.DANCE;
            animator.SetTrigger(trigger);
            if (_isLoop) return;
            DOVirtual.DelayedCall(3f, () =>
            {
                state = STATE.NORMAL;
                animator.ResetTrigger(trigger);
                animator.SetTrigger("Recover");
                damageCollider.enabled = true;
            });
        });
        PlayerPrefs.SetFloat("HP",maxHp);
    }

    public void PlayJumpAnim()
    {
        if (state != STATE.NORMAL) return;
        animator.SetTrigger("Jump");
    }

    // 広告表示後の設定処理
    public void ContinueSetting()
    {
        PlayerPrefs.SetFloat("HP",maxHp);
    }

    public void PowerUp()
    {
        if (isPowerUp) return;
        isPowerUp = true;
        itemSource.PlayOneShot(itemSE[0]);
        float previousAt = attack;
        attack *= 3f;
        buffParticle.SetActive(true);
        DOVirtual.DelayedCall(powerUpTime,() =>
        {
            attack = previousAt;
            buffParticle.SetActive(false);
            isPowerUp = false;
        });
    }

    public void Recover()
    {
        hp = maxHp;
        itemSource.PlayOneShot(itemSE[1]);
        hpGauge.RecoverHP(maxHp);
        recoverParticle.SetActive(true);
    }

    public void PlayJumpSE(int _clipNum)
    {
        jumpSource.PlayOneShot(jumpSE[_clipNum]);
    }
}
