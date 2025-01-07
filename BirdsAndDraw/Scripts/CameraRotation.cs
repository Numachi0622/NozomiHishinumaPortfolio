using System;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    [SerializeField] private float _rotSpeed = 1f;
    private Transform _tf;
    private float _rotY = 0;

    private void Start()
    {
        _tf = transform;
    }

    private void Update()
    {
        _rotY += _rotSpeed * Time.deltaTime;
        _tf.eulerAngles = new Vector3(0, _rotY, 0);
    }
}
