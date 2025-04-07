using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Utility;

/// <summary>攻撃情報の受け渡しをするコンポーネント</summary>
public class Attacker : MonoBehaviour {
    /// <summary>攻撃判定のコライダー</summary>
    public Collider Collider => _collider;
    [SerializeField] private Collider _collider;
    
    /// <summary>接触イベント</summary>
    public Action OnTriggerEnterEvent;

    /// <summary>威力</summary>
    public int Power { get; private set; }
    
    /// <summary>
    /// 攻撃タイプ
    /// </summary>
    public HitType HitType { get; private set; }

    /// <summary>攻撃情報を設定</summary>
    /// <param name="power">威力</param>
    /// <param name="hitType">攻撃タイプ</param>
    public void SetData(int power, HitType hitType) {
        Power = power;
        HitType = hitType;
    }

    /// <summary>
    /// 攻撃情報を設定
    /// </summary>
    /// <param name="power">威力</param>
    public void SetData(int power)
    {
        Power = power;
    }

    /// <summary>
    /// 攻撃進行中フラグ
    /// </summary>
    private bool _isAttacking = false;

    /// <summary>
    /// 非同期処理で指定時間の間AttackColliderを有効にする
    /// </summary>
    /// <param name="waitTime">待機時間</param>
    /// <param name="onWaited">待機後のコールバック</param>
    public async UniTaskVoid EnableAttackCollider(float waitTime = 0f, Action onWaited = null)
    {
        if(_collider == null) return;
        if (_isAttacking) return;
        
        // 待機
        await UniTask.Delay(TimeSpan.FromSeconds(waitTime));
        
        // 攻撃進行中フラグを立てる
        _isAttacking = true;
        
        // 待機後のコールバック実行
        onWaited?.Invoke();
        
        // Colliderを有効化 -> 無効化
        _collider.enabled = true;
        await UniTask.Delay(TimeSpan.FromSeconds(GameConst.COLLIDER_ACTIVE_TIME));
        _collider.enabled = false;
        
        // 攻撃進行中を削除
        _isAttacking = false;
    }

    /// <summary>TriggerのColliderとの接触処理</summary>
    /// <param name="other">接触したコライダー</param>
    private void OnTriggerEnter(Collider other) {
        OnTriggerEnterEvent?.Invoke();
    }
}
