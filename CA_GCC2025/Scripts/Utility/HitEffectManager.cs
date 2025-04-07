using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    public enum HitType
    {
        FromPlayer,
        FromPlayerSkill,
        FromPlayerCritical,
        FromPlayerSkillCritical,
        FromEnemy,
    }
    public class HitEffectManager : Singleton<HitEffectManager>
    {
        /// <summary>
        /// プレイヤーが攻撃を受けたときのヒットエフェクト
        /// </summary>
        [SerializeField] private ParticleSystem _playerHitEffect;
    
        /// <summary>
        /// 敵が攻撃を受けたときのヒットエフェクト
        /// </summary>
        [SerializeField] private ParticleSystem _enemyHitEffect;

        /// <summary>
        /// クリティカル時のヒットエフェクト
        /// </summary>
        [SerializeField] private ParticleSystem _criticalHitEffect;

        /// <summary>
        /// プレイヤーと敵のヒットエフェクトを同時に管理するDictionary
        /// </summary>
        private Dictionary<HitType, List<ParticleSystem>> _hitEffectPool;

        protected override void Awake()
        {
            _hitEffectPool = new Dictionary<HitType, List<ParticleSystem>>()
            {
                { HitType.FromEnemy , new List<ParticleSystem>()},
                { HitType.FromPlayer , new List<ParticleSystem>()},
                { HitType.FromPlayerCritical, new List<ParticleSystem>()}
            };
            
            base.Awake();
        }

        /// <summary>
        /// オブジェクトプールから取得したヒットエフェクトを再生する
        /// </summary>
        /// <param name="type">ヒットエフェクトのタイプ</param>
        /// <param name="pos">再生する座標</param>
        public void Play(HitType type, Vector3 pos)
        {
            var hitEffect = GetEffectFromPool(type);
            hitEffect.transform.position = pos;
            hitEffect.Play();
        }

        /// <summary>
        /// オブジェクトプールからヒットエフェクトを取得
        /// </summary>
        /// <param name="hitType"></param>
        /// <returns>ヒットエフェクト</returns>
        private ParticleSystem GetEffectFromPool(HitType hitType)
        {
            var type = hitType;
            if (type == HitType.FromPlayerSkillCritical) type = HitType.FromPlayerCritical;
            else if (type == HitType.FromPlayerSkill) type = HitType.FromPlayer;           
            
            for (int i = 0; i < _hitEffectPool[type].Count; i++)
            {
                if (!_hitEffectPool[type][i].gameObject.activeSelf)
                {
                    _hitEffectPool[type][i].gameObject.SetActive(true);
                    return _hitEffectPool[type][i];
                }
            }

            var effectPrefab = GetEffectType(type);
            var effect = Instantiate(effectPrefab, transform).GetComponent<ParticleSystem>();
            _hitEffectPool[type].Add(effect);
            return effect;
        }

        private ParticleSystem GetEffectType(HitType type)
        {
            switch (type)
            {
                case HitType.FromEnemy :
                    return _playerHitEffect;
                case HitType.FromPlayer :
                    return _enemyHitEffect;
                case HitType.FromPlayerCritical :
                    return _criticalHitEffect;
                case HitType.FromPlayerSkillCritical :
                    return _criticalHitEffect;
                default: 
                    return _enemyHitEffect;
            }
        }
    }
}