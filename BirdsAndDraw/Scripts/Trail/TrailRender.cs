using System;
using UnityEngine;

public class TrailRender : MonoBehaviour
{
    [SerializeField] private Material _material;
    [SerializeField] private float _width;
    private BoidsGPU _boidsGPU;
    private TrailGPU _trailGPU;

    private void Start()
    {
        _boidsGPU = GetComponent<BoidsGPU>();
        _trailGPU = GetComponent<TrailGPU>();
    }

    private void OnRenderObject()
    {
        _material.SetFloat("_NodeNumPerTrail", _trailGPU.NodeNumPerTrail);
        _material.SetFloat("_Width", _width);
        _material.SetBuffer("_TrailBuffer", _trailGPU.TrailBuffer);
        _material.SetBuffer("_NodeBuffer", _trailGPU.NodeBuffer);
        _material.SetPass(0);
        
        Graphics.DrawProceduralNow(MeshTopology.Points, _trailGPU.NodeNumPerTrail,instanceCount: _boidsGPU.InstanceCount);
    }
}
