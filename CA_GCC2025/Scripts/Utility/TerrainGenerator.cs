using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using Utility;
using Random = UnityEngine.Random;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private GameObject _grassPrefab;
    [SerializeField] private GameObject[] _flowerPrefabs;
    [SerializeField] private GameObject[] _obstaclePrefabs;
    [SerializeField] private int _grassNum = 100;
    [SerializeField] private int _flowerNum = 100;
    [SerializeField] private int _obstacleNum = 30;

    private void Start()
    {
        for (int i = 0; i < _grassNum; i++)
        {
            var pos = new Vector3(Random.Range(GameConst.MIN_MOVABLE_AREA, GameConst.MAX_MOVABLE_AREA), 0f, Random.Range(GameConst.MIN_MOVABLE_AREA, GameConst.MAX_MOVABLE_AREA));
            var grass = Instantiate(_grassPrefab, pos, Quaternion.identity, transform);
        }

        for (int i = 0; i < _flowerNum; i++)
        {
            var pos = new Vector3(Random.Range(GameConst.MIN_MOVABLE_AREA, GameConst.MAX_MOVABLE_AREA), 0f, Random.Range(GameConst.MIN_MOVABLE_AREA, GameConst.MAX_MOVABLE_AREA));
            var flower = Instantiate(_flowerPrefabs[Random.Range(0, _flowerPrefabs.Length)], pos, Quaternion.identity, transform);
        }
        
        for (int i = 0; i < _obstacleNum; i++)
        {
            var pos = new Vector3(Random.Range(GameConst.MIN_MOVABLE_AREA, GameConst.MAX_MOVABLE_AREA), 0f, Random.Range(GameConst.MIN_MOVABLE_AREA, GameConst.MAX_MOVABLE_AREA));
            var flower = Instantiate(_obstaclePrefabs[Random.Range(0, _flowerPrefabs.Length)], pos, Quaternion.identity, transform);
        }
    }
}
