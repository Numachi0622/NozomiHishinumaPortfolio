using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using Interface;
using R3;
using UnityEngine;
using UnityEngine.Playables;
using Utility;

public class EnemyPresenter : MonoBehaviour
{
    /// <summary>
    /// 敵のパラメータデータ
    /// </summary>
    [SerializeField] private EnemyParams _enemyParams;
    
    /// <summary>
    /// 敵のAttacker
    /// </summary>
    [SerializeField] private Attacker _attacker;

    /// <summary>
    /// 攻撃処理クラス
    /// </summary>
    private EnemyAttack _enemyAttack;
    
    /// <summary>
    /// 移動処理クラス
    /// </summary>
    private EnemyMovement _enemyMovement;
    
    /// <summary>
    /// 被ダメ処理クラス
    /// </summary>
    private IDamagable _enemyDamage;

    /// <summary>
    /// 敵の演出管理クラス
    /// </summary>
    private EnemyEffectManager _enemyEffectManager;
    
    /// <summary>
    /// 敵のHPModel
    /// </summary>
    private HitPointModel _hpModel;

    /// <summary>
    /// 敵のHPView
    /// </summary>
    private HitPointView _hpView;

    /// <summary>
    /// 敵のStateModel
    /// </summary>
    private EnemyStateModel _stateModel;

    /// <summary>
    /// 被ダメ用Collider
    /// </summary>
    private Collider _damageCollider;
    
    /// <summary>
    /// 最後に受けた攻撃タイプを格納
    /// </summary>
    private HitType _lastHitType;
    
    private Animator _animator;

    /// <summary>
    /// 死亡したときの処理を格納
    /// </summary>
    public Action OnDead;

    /// <summary>
    /// 死亡状態フラグ
    /// </summary>
    private bool IsDead => _stateModel.CurrentState == EnemyState.State.Dead;

    /// <summary>
    /// 移動可能フラグ
    /// </summary>
    private bool IsMovable => _stateModel.CurrentState == EnemyState.State.Idle;
    
    public void Initialize()
    {
        // 初期化
        _animator = GetComponent<Animator>();
        _damageCollider = GetComponent<Collider>();
        _enemyAttack = GetComponent<EnemyAttack>();
        _enemyEffectManager = GetComponent<EnemyEffectManager>();
        
        _hpModel = new HitPointModel(_enemyParams.MaxHp);
        _enemyMovement = new EnemyMovement(_enemyParams, transform);
        _enemyDamage = new EnemyDamage(_hpModel);
        _stateModel = new EnemyStateModel(EnemyState.State.Idle);
        
        _enemyAttack.Initialize(_enemyParams, _animator, _attacker, _stateModel);
        _enemyEffectManager.Initialize(_enemyParams, _animator);
        
        // ターゲットの近くで敵が停止したときのイベントを登録
        _enemyMovement.OnStopNearTarget = () =>
        {
            if(IsDead) return;
            _enemyAttack.AttackReady().Forget();
        };
        
        // 攻撃準備に入ったときのイベントを登録
        _enemyAttack.OnAttackReady = () =>
        {
            if(IsDead) return;
            _enemyEffectManager.FadeColor(Color.gray, 3);
            _enemyEffectManager.ShakeBody(2);
        };
        
        // HP減少イベントを登録
        _hpModel.Hp
            .Skip(1)
            .Subscribe(hp =>
            {
                // HPゲージ更新
                _hpView?.UpdateHp(hp);

                if (hp <= 0 && _stateModel.CurrentState != EnemyState.State.Dead)
                {
                    Dead().Forget();
                }
            })
            .AddTo(this);
        
        // IdleStateに入ったときのイベントを登録
        _stateModel.State
            .Where(state => state == EnemyState.State.Idle)
            .Subscribe(_ =>
            {
                _animator.SetBool("IsMove", true);
            })
            .AddTo(this);
    }

    /// <summary>
    /// 非同期処理で死亡Stateへ遷移
    /// スキル演出中に死亡してしまうのを防ぐため
    /// </summary>
    /// <returns></returns>
    private async UniTaskVoid Dead()
    {
        // 死亡Stateへ遷移
        _stateModel.SetState(EnemyState.State.Dead);
        
        await UniTask.WaitUntil(() => !GameObserver.Instance.IsSkillTime);
        
        // Colliderを無効化
        _damageCollider.enabled = false;
                    
        // HPゲージとの紐付けを削除
        Destroy(_hpView?.gameObject, 0.5f);
                    
        // 死亡アニメーション再生
        _animator.SetTrigger("Dead");
        _enemyEffectManager.PlayDeadEffect().Forget();
        
        SoundManager.Instance.PlaySe(SoundType.EnemyDead);
        
        OnDead?.Invoke();
    }

    public void Bind(HitPointView hpView)
    {
        _hpView = hpView;
        _hpView.Initialize(_enemyParams.MaxHp);
    }

    private void Update()
    {
        if (IsDead) return;

        if (IsMovable)
        {
            _enemyMovement.Move(Vector3.zero);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Attacker>(out var attacker))
        {
            _lastHitType = attacker.HitType;
            _enemyDamage.Damage(attacker.Power);

            var hitPos = _damageCollider.ClosestPointOnBounds(other.transform.position);
            DamageTextView.Instance.Play(_lastHitType, attacker.Power, hitPos);
            HitEffectManager.Instance.Play(_lastHitType, hitPos);
            
            // 再生中の演出を停止
            if(_stateModel.CurrentState == EnemyState.State.AttackReady)
                _animator.ResetTrigger("AttackReady");
                
            if(_stateModel.CurrentState == EnemyState.State.Attack)
                _animator.ResetTrigger("Attack");
                
            // ダメージアニメーション再生
            _animator.SetTrigger("Damage");

            // ダメージ演出を再生
            _enemyEffectManager.BlinkColor(_enemyParams.DamagedBodyColor);
            
            // クリティカルでヒットストップを起こす
            if (_lastHitType == HitType.FromPlayerCritical)
            {
                // 通常より振動量を増やす
                _enemyEffectManager.ShakeBody(0.4f, 50);
                
                var targetAnimator = attacker.transform.root.GetComponent<Animator>();
                _enemyEffectManager.HitStop(_animator, targetAnimator, () =>
                {
                    var isDead = _hpModel.Hp.CurrentValue <= 0;
                    _enemyMovement.KnockBack(isDead);
                }).Forget();
                return;
            }
            
            if (_lastHitType == HitType.FromPlayerSkillCritical)
            {
                // 通常より振動量を増やす
                _enemyEffectManager.ShakeBody(0.4f, 50);

                var playableDirector = attacker.transform.root.GetComponent<PlayableDirector>();
                _enemyEffectManager.SkillHitStop(playableDirector).Forget();
                return;
            }
            
            // 残りのダメージ演出を再生
            _enemyEffectManager.ShakeBody();
            if (_lastHitType != HitType.FromPlayerSkill)
            {
                var isDead = _hpModel.Hp.CurrentValue <= 0;
                _enemyMovement.KnockBack(isDead);  
            } 
        }
    }

    /// <summary>
    /// スペシャルスキル時にターゲットにならなかった場合は非表示にする
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
        _hpView.gameObject.SetActive(false);
    }

    /// <summary>
    /// スキル終了時に再表示
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
        _hpView.gameObject.SetActive(true);
    }
}
