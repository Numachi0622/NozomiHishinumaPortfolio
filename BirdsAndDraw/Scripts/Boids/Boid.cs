using Unity.VisualScripting;
using UnityEngine;

public class Boid : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _cohesionWeight = 0.05f;
    [SerializeField] private float _alignmentWeight = 0.1f;
    [SerializeField] private float _separationWeight = 0.2f;
    [SerializeField] private float _optimalCruisingDist = 20;
    [SerializeField] private float _boxSize = 50f;
    private Vector3 _position;
    private Vector3 _direction;
    private Vector3 _boidsCenter;
    private Vector3 _cohesionDir;
    private Vector3 _alignmentDir;
    private Vector3 _separationDir;
    private Boid _target;
    private Boid[] _group;
    private Transform _tf;
    
    public Vector3 Position => _position;
    public Vector3 Direction => _direction;

    public void Initialize(Vector3 pos, Boid[] group)
    {
        _position = pos;
        _direction = Random.onUnitSphere;
        _tf = transform;
        _group = group;
    }

    public void Move(float delta)
    {
        _position += _direction * _speed * delta;
        _tf.position = _position;
    }

    public void Turn()
    {
        if (_position.x > _boxSize || _position.x < -_boxSize)
        {
            _direction = new Vector3(-_direction.x, _direction.y, _direction.z);
        }
        if (_position.y > _boxSize || _position.y < -_boxSize)
        {
            _direction = new Vector3(_direction.x, -_direction.y, _direction.z);
        }
        if (_position.z > _boxSize || _position.z < -_boxSize)
        {
            _direction = new Vector3(_direction.x, _direction.y, -_direction.z);
        }
    }

    public void Cohesion(Vector3 center)
    {
        _cohesionDir = (center - _position).normalized;
    }

    public void Aligment()
    {
        if (_target == this) return;
        _alignmentDir = _target.Direction.normalized;
    }

    public void Separation()
    {
        _separationDir = Vector3.zero;
        if (_target == this) return;
        
        float dist = Vector3.Distance(_position, _target.Position);
        if (dist < _optimalCruisingDist)
        {
            _separationDir = (_position - _target.Position).normalized;
        }
    }

    public void Search()
    {
        float dist;
        float minDist = 99999;
        _target = this;
        foreach (var boid in _group)
        {
            float sp = Vector3.Dot(_direction, boid.Position - _position);
            if(sp <= 0) continue;
            dist = Vector3.Distance(_position, boid.Position);
            if (dist < minDist)
            {
                _target = boid;
                minDist = dist;
            }
        }
    }

    public void UpdateDirection()
    {
        _direction += _cohesionDir * _cohesionWeight
            + _alignmentDir * _alignmentWeight
            + _separationDir * _separationWeight;
        Vector3.Normalize(_direction);
    }
}
