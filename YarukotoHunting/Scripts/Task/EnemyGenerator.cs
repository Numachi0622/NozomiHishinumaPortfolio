using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyGenerator : MonoBehaviour
{
    private List<GameObject> enemyPool = new List<GameObject>();

    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Collider[] exclusionColliders;

    // 敵を生成
    public void GenerateEnemy(string _name,string _key,bool _isChecked)
    {
        Vector3 firstPos;
        do
        {
            firstPos = new Vector3(Random.Range(-70f, 80f), 0, Random.Range(-20f, 55f));
        } while (!Geanaratable(firstPos));
        //指定座標から一番近いnavmeshの座標を探す
        NavMeshHit hit;
        GameObject _enemy = _enemy = GetEnemyFromPool();
        if (NavMesh.SamplePosition(firstPos, out hit, 40, NavMesh.AllAreas))
        {
            _enemy.GetComponent<EnemyMove>().SetStartPosition(hit.position);
        }

        if (!_enemy.activeSelf) _enemy.SetActive(true);
        _enemy.transform.parent = this.gameObject.transform;

        EnemyStatus enemyStatus = _enemy.GetComponent<EnemyStatus>();
        enemyStatus.key = _key;
        enemyStatus.gameObject.name = _name;
        enemyStatus.DisplayTaskName(_name);
        enemyStatus.IsChecked(_isChecked);
    }

    // 敵オブジェクトをプールから取得
    private GameObject GetEnemyFromPool()
    {
        for(int i = 0; i < enemyPool.Count; i++)
        {
            if (!enemyPool[i].gameObject.activeSelf) return enemyPool[i];
        }
        GameObject newEnemy = Instantiate(enemyPrefab);
        enemyPool.Add(newEnemy);
        return newEnemy;
    }

    // 敵を生成な座標か可能か
    private bool Geanaratable(Vector3 _pos)
    {
        foreach(Collider e in exclusionColliders)
        {
            if (e.bounds.Contains(_pos)) return false;
        }
        return true;
    }
}
