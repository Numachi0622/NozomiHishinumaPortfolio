using System.Collections;
using UnityEngine;

public class BossStatus : StatusManager
{
    private WaitForSeconds normalDelayTime = new WaitForSeconds(0.5f);
    private WaitForSeconds magicAttackTime = new WaitForSeconds(5);
    private WaitForSeconds attackInterval;
    private WaitForSeconds timeToRecover = new WaitForSeconds(19f);
    private BossMove bossMove;
    public bool isPhysicalAttackable; // 物理攻撃ができるかどうか
    private bool isMagicAttacking = false;

    [SerializeField] private float attackIntervalTime;
    [SerializeField] private bool isPhysicalAttackOnly;
    [SerializeField] private Collider forMoveCollider;
    [SerializeField] private UIAnimation UIAnim;
    [SerializeField] private PlayerStatus playerStatus;
    [SerializeField] private SceneTransition sceneTransition;
    [SerializeField] private BossEffect bossEffect;
    [SerializeField] private CreditScore creditScore;
    [SerializeField] private Explosion explosion;
    [SerializeField] private CreditGenerator creditGenerator;
    [SerializeField] private SkinnedMeshRenderer bodyMesh;
    [SerializeField] private GameObject dieParticle;
    [SerializeField] private GameObject bossUI;
    [SerializeField] private AudioClip dieSE,otherDamageSE;
    [SerializeField] private FinishTaskSound finishTaskSound;
    [SerializeField] private ProgressManager progressManager;

    protected override void Awake()
    {
        base.Awake();
        bossMove = GetComponent<BossMove>();
    }

    private void Start()
    {
        attackInterval = new WaitForSeconds(attackIntervalTime);
        isPhysicalAttackable = false;
        hp = maxHp;
        hpGauge.SetHP(hp,maxHp);
    }
    public override void GoToAttackState()
    {
        if (state != STATE.NORMAL) return;
        state = STATE.ATTACK;
        bossMove.SpeedToZero();
        StartCoroutine(AttackLoop());

    }
    IEnumerator AttackLoop()
    {
        while (isPhysicalAttackable)
        {
            if (state == STATE.DIE) break;
            state = STATE.ATTACK;
            animator.SetTrigger("PhysicalAttack");
            yield return attackInterval;
        }
        while (!isPhysicalAttackable)
        {
            if (state == STATE.DIE || isPhysicalAttackOnly) break;
            yield return attackInterval;
            if (Random.Range(0, 3) == 0)
            {
                if (isMagicAttacking) break;
                isMagicAttacking = true;
                animator.SetTrigger("MagicAttack");
                bossMove.MoveStop();
                StartCoroutine(MagicAttackWaitCoroutine());
            }
        }
    }
    
    IEnumerator MagicAttackWaitCoroutine()
    {
        yield return magicAttackTime;
        GoToNormalState();
        animator.SetTrigger("MagicAttackFinish");
        bossMove.ResumeMove();
        bossMove.SetSpeed();
        isMagicAttacking = false;
    }
    // バトルが始まって最初に行う魔法攻撃
    public void FirstMagicAttack()
    {
        if (isPhysicalAttackOnly) return;
        state = STATE.ATTACK;
        animator.SetTrigger("MagicAttack");
        StartCoroutine(MagicAttackWaitCoroutine());
    }

    public override void GoToDieState()
    {
        base.GoToDieState();
        animator.SetTrigger("Die");
        forMoveCollider.enabled = false;
        bossMove.enabled = false;
        progressManager.UpdateProgress();
        StartCoroutine(EffectDelayCoroutine());
    }
    IEnumerator EffectDelayCoroutine()
    {
        finishTaskSound.Play(true);
        yield return new WaitForSeconds(1f);
        playerStatus.Dance(1, true);
        UIAnim.CompleteMessageAnim(gameObject.name,true);
        bodyMesh.enabled = false;
        bossUI.SetActive(false);
        dieParticle.SetActive(true);
        damageSource.PlayOneShot(dieSE);
        yield return new WaitForSeconds(3f);
        sceneTransition.FadeOut(5f);
    }

    public override void GoToParalysisState()
    {
        base.GoToParalysisState();
        bossEffect.ParalysisCamerawork();
        bossMove.SpeedToZero();
        StartCoroutine(RecoverDelayCoroutine());
    }
    IEnumerator RecoverDelayCoroutine()
    {
        yield return timeToRecover;
        bossMove.SetSpeed();
        RecoveryToNormal();
    }

    public override void ReceiveDamage()
    {
        if(explosion == null)
            base.ReceiveDamage();
        else
        {
            if (state == STATE.DIE) return;
            hp -= GetDamage();
            hpGauge.DecreaseHP(hp);
            damageText.AppearDamageText(GetDamage());
            if (state != STATE.PARALYSIS) animator.SetTrigger("Damage");
            damageSource.PlayOneShot(damageSE);

            // HPが0になったときの処理
            if (hp <= 0)
            {
                if (!creditScore.IsDefeatable)
                {
                    explosion.ExplosionEvent();
                    return;
                }
                GoToDieState();
                creditGenerator.gameObject.SetActive(false);
            }
        }
    }
}
