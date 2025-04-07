using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;

public class EnemyEffectManager : MonoBehaviour
{
    /// <summary>
    /// 敵本体ではなく子要素のTransform
    /// </summary>
    [SerializeField] private Transform _bodyTransform;

    /// <summary>
    /// 敵本体のMeshRenderer
    /// </summary>
    [SerializeField] private SkinnedMeshRenderer _bodyMeshRenderer;

    /// <summary>
    /// 死亡時のエフェクト
    /// </summary>
    [SerializeField] private ParticleSystem _deadEffect;
    
    /// <summary>
    /// 敵本体のマテリアル
    /// </summary>
    private Material _bodyMaterial;
    
    /// <summary>
    /// 色点滅アニメーションのSequence
    /// </summary>
    private Sequence _blinkSequence;

    /// <summary>
    /// 振動アニメーションのSequence
    /// </summary>
    private Sequence _shakeSequence;

    /// <summary>
    /// 振動方向のオフセット
    /// </summary>
    private Vector3 _shakeOffset;

    /// <summary>
    /// 敵のパラメータデータ
    /// </summary>
    private EnemyParams _enemyParams;

    private Animator _animator;

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize(EnemyParams enemyParams, Animator animator)
    {
        _enemyParams = enemyParams;
        _animator = animator;
        _bodyMaterial = _bodyMeshRenderer.material;
    }

    /// <summary>
    /// 色点滅の演出を再生
    /// </summary>
    /// <param name="color">点滅させる色</param>
    public void BlinkColor(Color color)
    {
        _blinkSequence?.Kill();
        
        _blinkSequence = DOTween.Sequence()
            .SetLink(gameObject)
            .Append(DOTween.To(() => Color.black, SetColor, color, 0.1f))
            .Append(DOTween.To(() => color, SetColor, Color.black, 0.15f));
    }
    
    /// <summary>フェードによる色点滅の演出を再生</summary>
    /// <param name="color">点滅させる色</param>
    /// <param name="loop">ループ回数</param>
    public void FadeColor(Color color, int loop) {
        // 前回の_blinkColorSeqがまだ再生中だった場合を考慮して、演出の強制終了メソッドを呼び出し
        _blinkSequence?.Kill();

        _blinkSequence = DOTween.Sequence()
            .SetLink(gameObject);

        for (int i = 0; i < loop; i++) {
            _blinkSequence
                .Append(DOTween.To(() => Color.black, SetColor, color, 0.15f))
                .Append(DOTween.To(() => color, SetColor, Color.black, 0.15f));
        }
    }

    /// <summary>
    /// 点滅演出を停止
    /// </summary>
    public void ResetBlink()
    {
        _blinkSequence?.Kill();
    }

    /// <summary>
    /// 色をシェーダーに渡す
    /// </summary>
    /// <param name="color"></param>
    private void SetColor(Color color)
    {
        _bodyMaterial.SetColor("_Color", color);
    }

    /// <summary>
    /// 振動アニメーションを再生
    /// </summary>
    public void ShakeBody(float strength = 0.25f, int vibrato = 30)
    {
        _shakeSequence?.Kill();

        _shakeSequence = DOTween.Sequence()
            .SetLink(gameObject)
            .Append(DOTween.Shake(() => Vector3.zero, offset => _shakeOffset = offset, 0.5f, strength, vibrato))
            .OnUpdate(() => _bodyTransform.localPosition += _shakeOffset)
            .SetUpdate(UpdateType.Late);
    }
    
    /// <summary>
    /// 振動アニメーションを再生（ループ）
    /// </summary>
    public void ShakeBody(int loop)
    {
        _shakeSequence?.Kill();

        _shakeSequence = DOTween.Sequence()
            .SetLink(gameObject)
            .Append(DOTween.Shake(() => Vector3.zero, offset => _shakeOffset = offset, 0.5f, 0.15f, 20, fadeOut: false))
            .OnUpdate(() => _bodyTransform.localPosition += _shakeOffset)
            .SetUpdate(UpdateType.Late)
            .SetLoops(loop);
    }

    /// <summary>
    /// 振動アニメーションを停止
    /// </summary>
    public void ResetShake()
    {
        _shakeSequence?.Kill();
    }

    /// <summary>
    /// 死亡時のエフェクトを再生
    /// </summary>
    public async UniTaskVoid PlayDeadEffect()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_enemyParams.HitStopDuration));
        
        _animator.speed = 0f;
        _blinkSequence?.Kill();
        SetColor(_enemyParams.DamagedBodyColor);
        
        await UniTask.Delay(TimeSpan.FromSeconds(_enemyParams.HitStopDuration));

        _animator.speed = 1f;
        _blinkSequence?.Kill();
        SetColor(_enemyParams.DeadBodyColor);
        _deadEffect.Play();
        _bodyMeshRenderer.shadowCastingMode = ShadowCastingMode.Off;
        
        // ディゾルブ表現
        DOTween.Sequence()
            .SetLink(gameObject)
            .Append(DOTween.To(() => 0f, threshold => _bodyMaterial.SetFloat("_Threshold", threshold), 1f, 1.5f))
            .OnComplete(() => Destroy(gameObject, 1f));
    }
    
    public async UniTaskVoid HitStop(Animator animator, Animator targetAnimator, Action onComplete = null)
    {
        if(animator == null) return;

        await UniTask.Delay(TimeSpan.FromSeconds(0.05f));
        animator.speed = 0f;
        if(targetAnimator != null) targetAnimator.speed = 0f;
        await UniTask.Delay(TimeSpan.FromSeconds(_enemyParams.HitStopDuration));
        animator.speed = 1f;
        if(targetAnimator != null) targetAnimator.speed = 1f;
        
        onComplete?.Invoke();
    }

    public async UniTaskVoid SkillHitStop(PlayableDirector director)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(0.05f));
        director?.playableGraph.GetRootPlayable(0).SetSpeed(0f);
        await UniTask.Delay(TimeSpan.FromSeconds(_enemyParams.SkillHitStopDuration));
        director?.playableGraph.GetRootPlayable(0).SetSpeed(1f);
    }
}
