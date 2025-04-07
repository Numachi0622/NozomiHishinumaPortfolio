using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SeearchTarget
{
    /// <summary>
    /// プレイヤーのパラメータデータ
    /// </summary>
    private PlayerParams _playerParams;
    
    /// <summary>
    /// 探索時の周囲の敵のColliderを格納する
    /// </summary>
    private Collider[] _targetColliders;

    private LayerMask _targetLayer;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="playerParams"></param>
    public SeearchTarget(PlayerParams playerParams)
    {
        _playerParams = playerParams;
        _targetColliders = new Collider[playerParams.MaxSearchCount];
        _targetLayer = LayerMask.GetMask("Enemy");
    }
    
    /// <summary>
    /// 最も近い敵を取得
    /// </summary>
    /// <param name="pos">現在位置</param>
    /// <returns>最も近い敵のTransform</returns>
    public Transform GetNearestTarget(Vector3 pos)
    {
        // 探索範囲内の敵を取得
        var hitCount = Physics.OverlapSphereNonAlloc(pos, _playerParams.SearchRange, _targetColliders, _targetLayer);
        if (hitCount == 0) return null;

        // 一番近い敵を取得
        var target = _targetColliders
            .Where(collider => collider != null)
            .OrderBy(collider => Vector3.Distance(pos, collider.transform.position))
            .FirstOrDefault()?
            .transform;

        return target;
    }
}
