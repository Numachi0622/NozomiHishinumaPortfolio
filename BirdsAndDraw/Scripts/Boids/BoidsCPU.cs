using System;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class BoidsCPU : MonoBehaviour
{
    [SerializeField] private int _instanceCount = 10;
    [SerializeField] private GameObject _boidPrefab;
    [SerializeField] private float _boxSize = 50f;
    private Boid[] boids;

    private void Start()
    {
        boids = new Boid[_instanceCount];
        for (int i = 0; i < _instanceCount; i++)
        {
            boids[i] = Instantiate(_boidPrefab).GetComponent<Boid>();
            boids[i].Initialize(Random.insideUnitSphere * _boxSize, boids);
        }
    }

    private void Update()
    {
        var center = CenterPosition();
        foreach (var boid in boids)
        {
            boid.Move(Time.deltaTime);
            boid.Turn();
            boid.Cohesion(center);
            boid.Search();
            boid.Aligment();
            boid.Separation();
            boid.UpdateDirection();
        }
    }

    private Vector3 CenterPosition()
    {
        Vector3 pos = Vector3.zero;
        foreach(var boid in boids)
        {
            pos += boid.Position;
        }
        pos /= boids.Length;
        return pos;
    }
}
