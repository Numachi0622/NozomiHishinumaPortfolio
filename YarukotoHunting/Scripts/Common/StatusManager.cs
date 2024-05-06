using UnityEngine;

public class StatusManager : MonoBehaviour
{
    public enum STATE
    {
        NORMAL, // 通常状態(Idol)
        ATTACK, // 攻撃状態
        DIE,     // 死亡状態
        PARALYSIS, // マヒ状態（ボス1でのみ）
        DANCE // ダンス状態
    }

    protected float hp; // HP
    protected float baseHp = 100; // 基礎HP
    [SerializeField] protected float maxHp = 100; // HPの最大値
    protected float damage; // 受けるダメージ量
    public float Damage { get => damage; }
    protected float baseAttack = 10;
    [SerializeField] protected float attack = 10; // 自身の攻撃力
    public float Attack { get => attack; }  

    public STATE state;

    public bool isMovable => STATE.NORMAL == state; // 移動可能か
    public bool isAttackable => STATE.NORMAL == state; // 可能か

    protected Animator animator;
    [SerializeField] protected DamageText damageText;
    [SerializeField] protected HPGauge hpGauge;

    [SerializeField] protected AudioSource damageSource, paralysisSource;
    [SerializeField] protected AudioClip damageSE, paralysisSE;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // ダメージを受ける関数
    public virtual void ReceiveDamage()
    {
        if (state == STATE.DIE) return;
        hp -= GetDamage();
        hpGauge.DecreaseHP(hp);
        damageText.AppearDamageText(GetDamage());
        damageSource.PlayOneShot(damageSE);
        if(state != STATE.PARALYSIS) animator.SetTrigger("Damage");

        // HPが0になったときの処理
        if (hp <= 0) GoToDieState();
    }

    public void SetDamage(float _value)
    {
        damage = _value;
    }
    public virtual float GetDamage()
    {
        return damage;
    }

    // Normal状態へ遷移
    public virtual void GoToNormalState()
    {
        if (state == STATE.NORMAL || state == STATE.DIE || state == STATE.PARALYSIS || state == STATE.DANCE) return;
        state = STATE.NORMAL;
    }

    // Attack状態へ遷移
    public virtual void GoToAttackState()
    {
        if (!isAttackable) return;
        state = STATE.ATTACK;

        // アニメーションの再生
        animator.SetTrigger("Attack");
    }

    // Die状態へ遷移
    public virtual void GoToDieState()
    {
        if (state == STATE.DIE) return;
        state = STATE.DIE; 
    }

    // マヒ状態へ遷移
    public virtual void GoToParalysisState()
    {
        if(state == STATE.PARALYSIS) return;
        state = STATE.PARALYSIS;
        animator.ResetTrigger("Recover");
        animator.SetTrigger("Paralysis");
        paralysisSource.PlayOneShot(paralysisSE);
    }

    // ノーマル状態へ回復（強制的にノーマル状態へ遷移）
    public void RecoveryToNormal()
    {
        state = STATE.NORMAL;
        animator.SetTrigger("Recover");
    }
}