using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace Sound
{
    [CreateAssetMenu(menuName = "SoundData")]
    public class SoundData : ScriptableObject
    {
        /// <summary>
        /// PlayerのSE
        /// </summary>
        [SerializeField] private AudioClip[] _attackVoice;
        [SerializeField] private AudioClip[] _slash;
        [SerializeField] private AudioClip[] _slashHit;
        [SerializeField] private AudioClip[] _lastComboSlashHit;
        [SerializeField] private AudioClip[] _criticalHit;
        [SerializeField] private AudioClip[] _skillAttack;
        [SerializeField] private AudioClip[] _explosionHit;
        [SerializeField] private AudioClip[] _skillVoice;
        [SerializeField] private AudioClip[] _specialSkillVoice;
        [SerializeField] private AudioClip[] _playerDamageVoice;
        [SerializeField] private AudioClip[] _playerDeadVoice;
        [SerializeField] private AudioClip[] _walk;
        [SerializeField] private AudioClip[] _run;

        /// <summary>
        /// 敵のSE
        /// </summary>
        [SerializeField] private AudioClip[] _enemyGenerate;
        [SerializeField] private AudioClip[] _enemyAttackReady;
        [SerializeField] private AudioClip[] _enemyAttack;
        [SerializeField] private AudioClip[] _enemyDead;
        [SerializeField] private AudioClip[] _enemyAttackHit;

        /// <summary>
        /// UI関係のSE
        /// </summary>
        [SerializeField] private AudioClip[] _skillLevelUp;
        [SerializeField] private AudioClip[] _skillReady;
        [SerializeField] private AudioClip[] _skillReadySelect;
        [SerializeField] private AudioClip[] _skillActivate;

        private Dictionary<SoundType, AudioClip[]> _sounds;

        public AudioClip Sounds(SoundType type) => _sounds[type][Random.Range(0, _sounds[type].Length)];

        private void OnEnable()
        {
            _sounds = new Dictionary<SoundType, AudioClip[]>()
            {
                { SoundType.AttackVoice, _attackVoice },
                { SoundType.Slash , _slash},
                { SoundType.SlashHit , _slashHit},
                { SoundType.LastComboSlashHit , _lastComboSlashHit},
                { SoundType.CriticalHit , _criticalHit},
                { SoundType.SkillAttack, _skillAttack},
                { SoundType.ExplosionHit , _explosionHit},
                { SoundType.SkillVoice , _skillVoice},
                { SoundType.SpecialSkillVoice , _specialSkillVoice},
                { SoundType.PlayerDamageVoice, _playerDamageVoice},
                { SoundType.PlayerDeadVoice, _playerDeadVoice },
                
                { SoundType.Walk, _walk},
                { SoundType.Run , _run},
                
                { SoundType.EnemyGenerate, _enemyGenerate},
                { SoundType.EnemyAttackReady, _enemyAttackReady},
                { SoundType.EnemyAttack, _enemyAttack},
                { SoundType.EnemyDead, _enemyDead},
                { SoundType.EnemyAttackHit, _enemyAttackHit},
                
                { SoundType.SkillLevelUp, _skillLevelUp},
                { SoundType.SkillReady, _skillReady},
                { SoundType.SkillReadySelect, _skillReadySelect},
                { SoundType.SKillActivate, _skillActivate},
            };
        }
    }
}

public enum SoundType
{
    AttackVoice, Slash, SlashHit, LastComboSlashHit, CriticalHit, SkillAttack, ExplosionHit, SkillVoice, SpecialSkillVoice,PlayerDamageVoice, PlayerDeadVoice,
    Walk, Run,
    EnemyGenerate, EnemyAttackReady, EnemyAttack, EnemyDead, EnemyAttackHit,
    SkillLevelUp, SkillReady, SkillReadySelect, SKillActivate,
}