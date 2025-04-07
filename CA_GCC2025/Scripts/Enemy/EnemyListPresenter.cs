using System;
using System.Collections;
using System.Collections.Generic;
using R3;
using R3.Triggers;
using ObservableCollections;
using R3.Collections;
using UnityEngine;
using Utility;
using Random = UnityEngine.Random;

public class EnemyListPresenter : MonoBehaviour
{
    /// <summary>
    /// スライムの数
    /// </summary>
    [SerializeField] private int _slimeCount = 5;

    /// <summary>
    /// タートルシェルの数
    /// </summary>
    [SerializeField] private int _turtleShellCount = 4;

    /// <summary>
    /// ドラゴンの数
    /// </summary>
    [SerializeField] private int _dragonCount = 1;
    
    /// <summary>
    /// 敵のPrefab
    /// </summary>
    [SerializeField] private EnemyPresenter _slime, _turtleShell, _dragon;

    /// <summary>
    /// 敵に紐付けるHPViewのPrefab
    /// </summary>
    [SerializeField] private HitPointView _hpViewPrefab;

    /// <summary>
    /// HPゲージを表示する親
    /// </summary>
    [SerializeField] private Transform _hpViewParent;

    /// <summary>
    /// 敵リストのビュー
    /// </summary>
    [SerializeField] private EnemyListView _view;
    
    /// <summary>
    /// 敵リストのモデル
    /// </summary>
    private EnemyListModel _model;
    
    private void Start()
    {
        _model = new EnemyListModel(_slimeCount + _turtleShellCount + _dragonCount);
        GameObserver.Instance.RegisterEnemy(_model.Enemies);
        
        // 敵を生成
        Generate(_slime, _slimeCount);
        Generate(_turtleShell, _turtleShellCount);
        Generate(_dragon, _dragonCount);

        // 敵数が変動したときのイベント
        _model.EnemyNum
            .Subscribe(num =>
            {
                _view.UpdateEnemyCount(num);
                if(num == 0) GameObserver.Instance.GameEnd(EndType.Win);
            })
            .AddTo(this);
    }

    /// <summary>
    /// 敵を生成
    /// </summary>
    private void Generate(EnemyPresenter enemyPrefab, int count)
    {
        for (int i = 0; i < count; i++)
        {
            var enemy = Instantiate(enemyPrefab, RandomPos, Quaternion.identity, transform)
                .GetComponent<EnemyPresenter>();
            var hpView = Instantiate(_hpViewPrefab, _hpViewParent)
                .GetComponent<HitPointView>();

            enemy.Bind(hpView);

            enemy.Initialize();

            enemy.OnDead = () => _model.Decrease(enemy);

            hpView.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    hpView.transform.position =
                        RectTransformUtility.WorldToScreenPoint(Camera.main,
                            enemy.transform.position + Vector3.up * 1.4f);
                })
                .AddTo(this);

            _model.Register(enemy);
        }
        
        SoundManager.Instance.PlaySe(SoundType.EnemyGenerate);
    }

    /// <summary>
    /// ランダムな生成座標
    /// </summary>
    private Vector3 RandomPos
        => new Vector3(
            Random.Range(GameConst.MIN_MOVABLE_AREA, GameConst.MAX_MOVABLE_AREA), 
            0,
            Random.Range(GameConst.MIN_MOVABLE_AREA, GameConst.MAX_MOVABLE_AREA));
}
