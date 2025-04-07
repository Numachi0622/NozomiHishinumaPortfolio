using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyListView : MonoBehaviour
{
    /// <summary>
    /// 敵の残数テキスト
    /// </summary>
    [SerializeField] private TextMeshProUGUI _enemyCountText;

    /// <summary>
    /// 敵の残数テキストを更新する
    /// </summary>
    /// <param name="count"></param>
    public void UpdateEnemyCount(int count)
    {
        _enemyCountText.text = $"のこり {count}";
    }
}
