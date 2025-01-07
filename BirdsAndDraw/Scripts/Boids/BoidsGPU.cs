using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Random = UnityEngine.Random;

public class BoidsGPU : MonoBehaviour
{
    [Serializable]
    private struct BoidData
    {
        public Vector3 Position;
        public Vector3 Velocity;
    }

    [Header("Boids Param")] 
    [SerializeField] private int _instanceCount = 10000;
    public int InstanceCount => _instanceCount;
    
    [SerializeField] private float _cohesionNeighborhoodRadius = 2f;
    [SerializeField] private float _alignmentNeighborhoodRadius = 2f;
    [SerializeField] private float _separateNeighborhoodRadius = 1f;

    [SerializeField] private float _maxSpeed = 5f;
    [SerializeField] private float _maxStreerForce = 0.5f;
    
    [SerializeField] private float _cohesionWeight = 1f;
    [SerializeField] private float _alignmentWeight = 1f;
    [SerializeField] private float _separateWeight = 1f;
    [SerializeField] private float _avoidWallWeight = 10f;

    [SerializeField] private Vector3 _wallCenter = Vector3.zero;
    public Vector3 WallCenter => _wallCenter;
    
    [SerializeField] private Vector3 _wallSize = new Vector3(32f, 32f, 32f);
    public Vector3 WallSize => _wallSize;
    
    [Header("Resource")]
    [SerializeField] private ComputeShader _computeShader;
    private ComputeBuffer _boidForceBuffer;
    private ComputeBuffer _boidDataBuffer;
    public ComputeBuffer BoidDataBuffer => _boidDataBuffer;

    private List<BoidData> _boidDataArray;
    private List<Vector3> _boidForceArray;

    public bool IsAddBoid { get; set; } = false;
    public Vector3 AddPosition { get; set; } = Vector3.zero;
    public int AddValue { get; set; } = 0;
    
    /// <summary>
    /// 1グループに含まれるスレッド数
    /// </summary>
    public const int ThreadNum = 256;

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        if (IsAddBoid)
        {
            IsAddBoid = false;
            AddBoid(AddValue, AddPosition);
        }
        Simulation();

        // var array = new BoidData[_instanceCount];
        // _boidDataBuffer.GetData(array);
        // var pos = array[0].Position;
        // testGo.transform.position = pos;
    }

    private void Initialize()
    {
        // バッファを初期化
        _boidDataBuffer = new ComputeBuffer(_instanceCount, Marshal.SizeOf(typeof(BoidData)));
        _boidForceBuffer = new ComputeBuffer(_instanceCount, Marshal.SizeOf(typeof(Vector3)));
        
        // Boidデータ, Forceバッファを初期化
        _boidForceArray = new List<Vector3>();
        _boidDataArray = new List<BoidData>();
        for (int i = 0; i < _instanceCount; i++)
        {
            _boidForceArray.Add(Vector3.zero);
            _boidDataArray.Add(new BoidData()
            {
                Position = Random.insideUnitSphere * 10f,
                Velocity = Random.insideUnitSphere * 0.1f
            });
        }
        _boidForceBuffer.SetData(_boidForceArray.ToArray());
        _boidDataBuffer.SetData(_boidDataArray.ToArray());
    }

    private void Simulation()
    {
        var cs = _computeShader;
        
        // インスタンス数から合計グループ数を取得
        var threadGroupNum = Mathf.CeilToInt((float)_instanceCount / ThreadNum);
        
        var forceKernel = cs.FindKernel("Force");
        cs.SetInt("_InstanceCount", _instanceCount);
        cs.SetFloat("_CohesionNeighborhoodRadius", _cohesionNeighborhoodRadius);
        cs.SetFloat("_AlignmentNeighborhoodRadius", _alignmentNeighborhoodRadius);
        cs.SetFloat("_SeparateNeighborhoodRadius", _separateNeighborhoodRadius);
        cs.SetFloat("_MaxSpeed", _maxSpeed);
        cs.SetFloat("_MaxSteerForce", _maxStreerForce);
        cs.SetFloat("_CohesionWeight", _cohesionWeight);
        cs.SetFloat("_AlignmentWeight", _alignmentWeight);
        cs.SetFloat("_SeparateWeight", _separateWeight);
        cs.SetFloat("_AvoidWallWeight", _avoidWallWeight);
        cs.SetVector("_WallCenter", _wallCenter);
        cs.SetVector("_WallSize", _wallSize);
        cs.SetBuffer(forceKernel, "_BoidDataBufferRead", _boidDataBuffer);
        cs.SetBuffer(forceKernel, "_BoidForceBufferWrite", _boidForceBuffer);
        
        // Forceカーネルを実行
        cs.Dispatch(forceKernel, threadGroupNum, 1, 1);
        
        var integrateKernel = cs.FindKernel("Integrate");
        cs.SetFloat("_DeltaTime", Time.deltaTime);
        cs.SetBuffer(integrateKernel, "_BoidDataBufferWrite", _boidDataBuffer);
        cs.SetBuffer(integrateKernel, "_BoidForceBufferRead", _boidForceBuffer);
        
        // Integrateカーネルを実行
        cs.Dispatch(integrateKernel, threadGroupNum, 1, 1);
    }

    public void UpdateInput(ComputeBuffer inputBuffer)
    {
        var cs = _computeShader;
        // インスタンス数から合計グループ数を取得
        var threadGroupNum = Mathf.CeilToInt((float)_instanceCount / ThreadNum);
        
        var updateInputKernel = cs.FindKernel("UpdateInput");
        cs.SetBuffer(updateInputKernel, "_BoidDataBufferRead", _boidDataBuffer);
        cs.SetBuffer(updateInputKernel, "_InputBuffer", inputBuffer);
        cs.SetFloat("_Time", Time.time);
        cs.Dispatch(updateInputKernel, threadGroupNum, 1, 1);
    }

    public void AddBoid(int addValue, Vector3 center)
    {
        _instanceCount += addValue;
        
        var prevBoidDataArray = new BoidData[_boidForceArray.Count];
        _boidDataBuffer.GetData(prevBoidDataArray);
        _boidDataArray = prevBoidDataArray.ToList();
        
        var prevBoidForceArray = new Vector3[_boidForceArray.Count];
        _boidForceBuffer.GetData(prevBoidForceArray);
        _boidForceArray = prevBoidForceArray.ToList();
        
        for (int i = 0; i < addValue; i++)
        {
            _boidForceArray.Add(Vector3.zero);
            _boidDataArray.Add(new BoidData()
            {
                Position = center,
                Velocity = Random.insideUnitSphere * 0.1f
            });
        }
        
        _boidDataBuffer = new ComputeBuffer(_instanceCount, Marshal.SizeOf(typeof(BoidData)));
        _boidForceBuffer = new ComputeBuffer(_instanceCount, Marshal.SizeOf(typeof(Vector3)));
        
        _boidForceBuffer.SetData(_boidForceArray.ToArray());
        _boidDataBuffer.SetData(_boidDataArray.ToArray());
    }

    private void OnDestroy()
    {
        _boidForceBuffer?.Release();
        _boidDataBuffer?.Release();
    }
}
