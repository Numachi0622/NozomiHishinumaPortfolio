using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using R3;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utility
{
    public class GameObserver : Singleton<GameObserver>
    {
        [SerializeField] private GameEndView _view;
        public bool IsGameEnd { get; private set; } = false;

        public bool IsSkillTime { get; private set; } = false;

        public EnemyPresenter TargetingEnemy { get; private set; }

        private List<EnemyPresenter> _enemyListCopy = new List<EnemyPresenter>();

        private void Start()
        {
            _view.RestartButton.OnClickAsObservable()
                .Subscribe(_ => SceneManager.LoadScene(0))
                .AddTo(this);
        }

        public void RegisterEnemy(List<EnemyPresenter> enemies)
        {
            _enemyListCopy = enemies;
        }

        public void GameEnd(EndType type)
        {
            IsGameEnd = true;
            _view.Play(type);
        }

        public void SkillMode(EnemyPresenter targetingEnemy)
        {
            if(IsGameEnd) return;
            IsSkillTime = true;
            TargetingEnemy = targetingEnemy;
        }

        public void HideEnemyInSkillMode()
        {
            TargetingEnemy.transform.rotation = Quaternion.Euler(0f, -180f, 0f);
            TargetingEnemy.GetComponent<Animator>()?.SetBool("IsMove", false);
            foreach (var enemy in _enemyListCopy)
            {
                if(enemy == TargetingEnemy) continue;
                enemy.Hide();
            }
        }

        public void ResetSkillMode()
        {
            IsSkillTime = false;
            
            foreach (var enemy in _enemyListCopy)
            {
                if(enemy == TargetingEnemy) continue;
                enemy.Show();
            }
            TargetingEnemy.GetComponent<Animator>()?.SetBool("IsMove", true);
            TargetingEnemy = null;
        }
    }
    
    public enum EndType
    {
        Win, Lose
    }
}
