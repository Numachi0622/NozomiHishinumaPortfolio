using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Interface;
using UnityEngine;
using R3;
using R3.Triggers;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine.Playables;
using Utility;

public class PlayerPresenter : MonoBehaviour
{
    /// <summary>
    /// プレイヤーのパラメータデータ
    /// </summary>
    [SerializeField] private PlayerParams _playerParams;
    
    /// <summary>
    /// 仮想ジョイスティック入力を検知
    /// </summary>
    [SerializeField] private InputVirtualJoyStick _inputVirtualJoyStick;

    /// <summary>
    /// 画面タップを検知
    /// </summary>
    [SerializeField] private InputTapScreen _inputTapScreen;
    
    /// <summary>
    /// プレイヤーのAttacker
    /// </summary>
    [SerializeField] private Attacker _attacker;

    /// <summary>
    /// プレイヤーに追従するカメラを制御するクラス
    /// </summary>
    [SerializeField] private FollowingCamera _followingCamera;

    /// <summary>
    /// プレイヤーのHPビュー
    /// </summary>
    [SerializeField] private HitPointView _hpView;

    /// <summary>
    /// プレイヤーのSPビュー
    /// </summary>
    [SerializeField] private SkillPointView _spView;

    /// <summary>
    /// スキル演出のビュー
    /// </summary>
    [SerializeField] private PlayerSkillView _skillView;

    /// <summary>
    /// スキル発動システムのPresenter
    /// </summary>
    [SerializeField] private SkillActivationPresenter _skillActPresenter;
    
    /// <summary>
    /// 攻撃処理クラス
    /// </summary>
    private PlayerAttack _playerAttack;

    /// <summary>
    /// スキル攻撃クラス
    /// </summary>
    private PlayerSkillAttack _playerSkillAttack;
    
    /// <summary>
    /// 移動処理クラス
    /// </summary>
    private PlayerMovement _playerMovement;
    
    /// <summary>
    /// 被ダメ処理クラス
    /// </summary>
    private IDamagable _playerDamage;
    
    /// <summary>
    /// プレイヤーのHPモデル
    /// </summary>
    private HitPointModel _hpModel;

    /// <summary>
    /// プレイヤーのSPモデル
    /// </summary>
    private SkillPointModel _spModel;
    
    /// <summary>
    /// プレイヤーのStateモデル
    /// </summary>
    private PlayerStateModel _stateModel;
    
    /// <summary>
    /// 周辺の敵を検知するクラス
    /// </summary>
    private SeearchTarget _searchTarget;
    
    /// <summary>
    /// 最も近い攻撃対象のTransform
    /// </summary>
    private Transform _nearestTarget;

    /// <summary>
    /// 被ダメ用Collider
    /// </summary>
    private Collider _damageCollider;
    
    private Animator _animator;
    
    // 一旦テストで
    // ToDo 後でクラス分離する
    [SerializeField] private GameObject _playerSkillEvent;
    
    /// <summary>
    /// 移動可能フラグ
    /// </summary>
    private bool IsMovable => _inputVirtualJoyStick.IsInput && _stateModel.CurrentState == PlayerState.State.Idle;
    
    /// <summary>
    /// 死亡フラグ
    /// </summary>
    private bool IsDead => _stateModel.CurrentState == PlayerState.State.Dead;

    /// <summary>
    /// 攻撃可能フラグ
    /// </summary>
    private bool IsAttackable
        => _stateModel.CurrentState == PlayerState.State.Idle ||
           _stateModel.CurrentState == PlayerState.State.Attack ||
           _stateModel.CurrentState == PlayerState.State.ComboAttack;

    private bool IsSkillAttackable => _stateModel.CurrentState == PlayerState.State.Idle;

    private void Awake()
    {
        // 初期化
        _animator = GetComponent<Animator>();
        _damageCollider = GetComponent<Collider>();
        _playerAttack = GetComponent<PlayerAttack>();
        _playerSkillAttack = GetComponent<PlayerSkillAttack>();
        
        _hpModel = new HitPointModel(_playerParams.MaxHp);
        _spModel = new SkillPointModel(0f);
        _playerMovement = new PlayerMovement(_playerParams, _animator, transform);
        _playerDamage = new PlayerDamage(_hpModel);
        _stateModel = new PlayerStateModel(PlayerState.State.Idle);
        _searchTarget = new SeearchTarget(_playerParams);
        
        _playerAttack.Initialize(_playerParams, _animator, _attacker, _stateModel, _spModel);
        _playerSkillAttack.Initialize(_playerParams, _animator, _stateModel);
        _followingCamera.Initialize(transform);
        _hpView.Initialize(_playerParams.MaxHp);
        
        // ジョイスティックの入力開始/終了イベントを登録
        _inputVirtualJoyStick.OnStartEvent = () => _animator.SetBool("IsMove", true);
        _inputVirtualJoyStick.OnCancelEvent = () =>
        {
            _animator.SetBool("IsMove", false);
            SoundManager.Instance.StopSeContinue();
        };
        
        // 画面タップ入力イベントを登録
        _inputTapScreen.OnPerformEvent = () =>
        {
            if(!IsAttackable) return;
            
            // 最も近いターゲットを取得
            if (!_playerAttack.IsComboAttacking)
            {
                _nearestTarget = _searchTarget.GetNearestTarget(transform.position);
            }
            
            if (_nearestTarget != null)
            {
                // ターゲットの方向に回転
                _playerMovement.AttackRotate(_nearestTarget);

                // ターゲットの方向にダッシュアタック
                _playerMovement.DashAttack(_nearestTarget);
            }

            // 攻撃処理
            _playerAttack.Attack();
        };

        // スキル発動イベント
        _skillActPresenter.OnSkillActivate = () =>
        {
            if(!IsSkillAttackable) return;

            _damageCollider.enabled = false;
            _spModel.Decrease(GameConst.SKILL_DECREASE_VALUE);
            
            // 最も近いターゲットを取得
            _nearestTarget = _searchTarget.GetNearestTarget(transform.position);
            if (_nearestTarget != null)
            {
                // ターゲットの方向に回転
                _playerMovement.AttackRotate(_nearestTarget);
            }
                
            _playerSkillAttack.Attack();
                
            _skillView.StartSkill();
            _followingCamera.Zoom();
            
            SoundManager.Instance.PlaySe(SoundType.SkillVoice);
        };

        _skillActPresenter.OnSpecialSkillActivate = () =>
        {
            _nearestTarget = _searchTarget.GetNearestTarget(transform.position);
            if(_nearestTarget == null) return;
            
            _spModel.Decrease(GameConst.SP_SKILL_DECREASE_VALUE);
            
            _stateModel.SetState(PlayerState.State.SKillAttack);
            GameObserver.Instance.SkillMode(_nearestTarget.GetComponent<EnemyPresenter>());
            _skillView.StartSpecialSkill(() =>
            {
                _followingCamera.gameObject.SetActive(false);
                var startPos = Vector3.zero;
                var targetPos = _nearestTarget.position;
                targetPos.z = Mathf.Max(targetPos.z, GameConst.MIN_MOVABLE_AREA + 14f);
                _nearestTarget.position = targetPos;
                
                _playerSkillEvent.transform.position = targetPos - new Vector3(0, 0, 12f);
                _playerSkillEvent.SetActive(true);
                transform.SetParent(_playerSkillEvent.transform);
                transform.localPosition = startPos; 
            });
            
            SoundManager.Instance.PlaySe(SoundType.SpecialSkillVoice);
        };
        
        _skillActPresenter.Initialize();


        // スペシャルスキル終了イベントを登録
        _playerSkillAttack.OnCompleteSkill = () => _damageCollider.enabled = true;
        
        _playerSkillEvent.GetComponent<SpecialSkillEffect>().OnCompleteEffect = () =>
        {
            _skillView.EndSpecialSkill(() =>
            {
                _stateModel.SetState(PlayerState.State.Idle);
                GameObserver.Instance.ResetSkillMode();
                _followingCamera.gameObject.SetActive(true);
                
                transform.SetParent(null);
                _playerSkillEvent.SetActive(false);
            });
        };
        
        // HP減少イベントを登録
        _hpModel.Hp
            .Skip(1) // 初期化時の購読を無視する
            .Subscribe(hp =>
            {
                // HPゲージ更新
                _hpView.UpdateHp(hp);
                
                // ダメージアニメーション再生
                _animator.SetTrigger("Damage");
                
                // カメラの振動
                _followingCamera.Shake();
                
                if (hp <= 0)
                {
                    // 死亡Stateへ遷移
                    _stateModel.SetState(PlayerState.State.Dead);
                    
                    // 死亡アニメーション再生
                    _animator.SetTrigger("Dead");
                    
                    // ゲームを終了する
                    GameObserver.Instance.GameEnd(EndType.Lose);
                    
                    SoundManager.Instance.PlaySe(SoundType.PlayerDeadVoice);
                    return;
                }
                
                SoundManager.Instance.PlaySe(SoundType.PlayerDamageVoice);
            })
            .AddTo(this);

        // SPゲージを更新
        _spModel.Sp
            .Skip(1)
            .Subscribe(_spView.UpdateSpGauge)
            .AddTo(this);

        // SPアイコンを更新
        _spModel.SpLevel
            .Skip(1)
            .Subscribe(spLevel =>
            {
                _spView.UpdateSpIcon(spLevel);
                SoundManager.Instance.PlaySe(SoundType.SkillLevelUp);
            })
            .AddTo(this);
        
        // 1コンボ終了ごとのイベントを登録
        //_playerAttack.OnAttackEndPerCombo = ResetTarget;
        
#if UNITY_EDITOR
        //Debug
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.D))
            .Subscribe(_ => _spModel.Decrease(0.3f))
            .AddTo(this);
        
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.I))
            .Subscribe(_ => _spModel.Increase(1f))
            .AddTo(this);

        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.S))
            .Subscribe(_ =>
            {
                _nearestTarget = _searchTarget.GetNearestTarget(transform.position);
                if(_nearestTarget == null) return;
                
                _stateModel.SetState(PlayerState.State.SKillAttack);
                GameObserver.Instance.SkillMode(_nearestTarget.GetComponent<EnemyPresenter>());
                _skillView.StartSpecialSkill(() =>
                {
                    _followingCamera.gameObject.SetActive(false);
                    var startPos = Vector3.zero;
                    _playerSkillEvent.transform.position = _nearestTarget.position - new Vector3(0, 0, 12f);
                    _playerSkillEvent.SetActive(true);
                    transform.SetParent(_playerSkillEvent.transform);
                    transform.localPosition = startPos; 
                });
            })
            .AddTo(this);
#endif
    }

    /// <summary>
    /// 攻撃対象をリセット
    /// AnimationのStateを切り替える度に呼ばれる
    /// </summary>
    public void ResetTarget()
    {
        _nearestTarget = null;
    }

    private void Update()
    {
        if (GameObserver.Instance.IsGameEnd) return;
        if (IsDead) return;
        
        if (IsMovable)
        {
            // ジョイスティックから移動方向を取得
            var direction = new Vector3(_inputVirtualJoyStick.Vector.x, 0f, _inputVirtualJoyStick.Vector.y);
            _playerMovement.Move(direction);
            return;
        }
        
        PlayerEffectManager.Instance.StopMoveEffect();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Attacker>(out var attacker))
        {
            _playerDamage.Damage(attacker.Power);
            
            var hitPos = _damageCollider.ClosestPointOnBounds(other.transform.position);
            DamageTextView.Instance.Play(HitType.FromEnemy, attacker.Power, hitPos);
            HitEffectManager.Instance.Play(HitType.FromEnemy, hitPos);
        }
    }
}
