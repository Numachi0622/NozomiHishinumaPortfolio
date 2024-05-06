using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class LastBossStatus : StatusManager
{
    [SerializeField] private LastBossEventManager eventManager;
    [SerializeField] private PlayerStatus playerStatus;
    [SerializeField] private UIAnimation UIAnim;
    [SerializeField] private SceneTransition sceneTransition;
    [SerializeField] private Collider moveCollider;
    [SerializeField] private Explosion explosion;
    [SerializeField] private GameObject bodyMesh;
    [SerializeField] private GameObject awakeningParticle;
    [SerializeField] private GameObject explosionWaitingParticle;
    [SerializeField] private GameObject dieParticle;
    [SerializeField] private GameObject[] vasnishingObjects;
    [SerializeField] private FinishTaskSound finishTaskSound;
    [SerializeField] private ProgressManager progressManager;
    [SerializeField] private AudioSource chargeSource;
    [SerializeField] private AudioClip chargeSE,dieSE;
    private BossMove bossMove;
    private WaitForSeconds attackInterval;
    private WaitForSeconds fireBallLotteryInterval = new WaitForSeconds(3);
    private WaitForSeconds fireBallTime = new WaitForSeconds(3);
    private WaitForSeconds explosionWaitingTime = new WaitForSeconds(5);
    private float attackIntervalTime = 5;
    private float addIntervalTime = 3;
    private bool isPhysicalAttackable;
    private bool isMagicAttacking;
    public bool isAwakening { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        bossMove = GetComponent<BossMove>();
    }

    private void Start()
    {
        hp = maxHp;
        hpGauge.SetHP(hp,maxHp);
        attackInterval = new WaitForSeconds(attackIntervalTime);
    }

    public void SetAttackJudge(bool _isAble)
    {
        isPhysicalAttackable = _isAble;
    }

    public override void GoToAttackState()
    {
        if (state == STATE.ATTACK) return;
        if (!isAttackable) return;
        StartCoroutine(AttackLoop());
    }
    IEnumerator AttackLoop()
    {
        while (isPhysicalAttackable)
        {
            if (state == STATE.DIE) break;
            if (isAwakening)
            {
                if (Random.Range(0, 5) == 0)
                {
                    state = STATE.ATTACK;
                    chargeSource.PlayOneShot(chargeSE);
                    explosionWaitingParticle.SetActive(true);
                    animator.ResetTrigger("Reset");
                    animator.SetTrigger("Explosion");
                    yield return explosionWaitingTime;
                    chargeSource.Stop();
                    explosionWaitingParticle.SetActive(false);
                    state = STATE.NORMAL;
                    animator.SetTrigger("Reset");
                    explosion.ExplosionEvent();
                }
                else
                {
                    state = STATE.ATTACK;
                    animator.SetTrigger("Punch");
                    yield return attackInterval;
                }
            }
            else
            {
                state = STATE.ATTACK;
                animator.SetTrigger("Punch");
                yield return attackInterval;
            }
        }
        while (!isPhysicalAttackable)
        {
            if (state == STATE.DIE) break;
            yield return fireBallLotteryInterval;
            if (Random.Range(0, 3) == 0)
            {
                state = STATE.ATTACK;
                isMagicAttacking = true;
                animator.SetTrigger("FireBall");
                bossMove.MoveStop();
                yield return fireBallTime;
                animator.SetTrigger("Reset");
                bossMove.ResumeMove();
                bossMove.SetSpeed();
                isMagicAttacking = false;
            }
        }
    }

    public override void ReceiveDamage()
    {
        if (state == STATE.DIE) return;
        hp -= GetDamage();
        hpGauge.DecreaseHP(hp);
        damageText.AppearDamageText(GetDamage());
        damageSource.PlayOneShot(damageSE);
        if (!isAwakening) animator.SetTrigger("Damage");
        if (hp <= maxHp / 2)
            AwakeningMode();

        // HPが0になったときの処理
        if (hp <= 0) GoToDieState();
    }

    private void AwakeningMode()
    {
        if (isAwakening) return;
        eventManager.InAwakeningEvent();
    }

    public void StatusUp()
    {
        isAwakening = true;
        awakeningParticle.SetActive(true);
        maxHp *= 2f;
        attack *= 1.5f;
        hp = maxHp;
        hpGauge.RecoverHP(maxHp);
        gameObject.GetComponent<NavMeshAgent>().speed = 6f;
        attackInterval = new WaitForSeconds(attackIntervalTime + addIntervalTime);
    }

    public override void GoToDieState()
    {
        base.GoToDieState();
        animator.SetTrigger("Die");
        moveCollider.enabled = false;
        bossMove.enabled = false;
        progressManager.UpdateProgress();
    }

    public void DieEffect()
    {
        damageSource.PlayOneShot(dieSE);
        dieParticle.SetActive(true);
        bodyMesh.SetActive(false);
        awakeningParticle.SetActive(false);
        foreach (GameObject o in vasnishingObjects)
            o.SetActive(false);
        StartCoroutine(EffectDelayCoroutine());
    }
    IEnumerator EffectDelayCoroutine()
    {
        yield return new WaitForSeconds(1.5f);
        finishTaskSound.Play(true);
        playerStatus.Dance(1, true);
        UIAnim.CompleteMessageAnim(gameObject.name, true);
        yield return new WaitForSeconds(3f);
        sceneTransition.FadeOut(5f);
    }
}
