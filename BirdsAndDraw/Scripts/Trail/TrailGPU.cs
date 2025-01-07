using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using Random = UnityEngine.Random;

public class TrailGPU : MonoBehaviour
{
    [Serializable]
    private struct Trail
    {
        public int CurrentNodeIdx;
        public float ElapsedTime;
        public float LineDensity;
        public float NoiseDensity;
    };
    
    [Serializable]
    private struct InputData
    {
        public Vector3 Position;
    };

    [Serializable]
    private struct Node
    {
        public float UpdatedTime;
        public Vector3 Position;
        public float Life;
    };

    [Header("Trail Param")]
    [SerializeField] private int _nodeNumPerTrail;
    public int NodeNumPerTrail => _nodeNumPerTrail;
    [SerializeField] private float _nodeLifeTime;
    private int _instanceCount;
    
    [Header("Resource")] 
    [SerializeField] private ComputeShader _computeShader;
    private BoidsGPU _boidsGPU;
    private ComputeBuffer _trailBuffer;
    public ComputeBuffer TrailBuffer => _trailBuffer;
    private ComputeBuffer _nodeBuffer;
    public ComputeBuffer NodeBuffer => _nodeBuffer;
    private ComputeBuffer _inputBuffer;
    
    private List<Trail> _trailArray;
    private List<InputData> _inputArray;
    private List<Node> _nodeArray;
    
    public const int ThreadNum = 256;

    public bool IsAddTrail { get; set; } = false;
    public int AddValue { get; set; } = 0;
    
    private void Start()
    {
        Initialize();
        // testGo = new GameObject[_nodeNumPerTrail];
        // for (int i = 0; i < _nodeNumPerTrail; i++)
        // {
        //     testGo[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //     testGo[i].transform.localScale = Vector3.one * 0.1f;
        // }
    }

    private void Update()
    {
        if (IsAddTrail)
        {
            IsAddTrail = false;
            AddTrail(AddValue);
        }
        UpdateInput();
    }

    private void LateUpdate()
    {
        CalcInput();
        return;
         var array0 = new Trail[_instanceCount];
         _trailBuffer.GetData(array0);
         
         var array1 = new Node[_instanceCount * _nodeNumPerTrail];
         _nodeBuffer.GetData(array1);
        
         var array2 = new InputData[_instanceCount];
         _inputBuffer.GetData(array2);
         
         var elapsed = array0[0].ElapsedTime;
         var updatedTime = array1[0].UpdatedTime;
         var life = array1[0].Life;
         var value = 1.0f - Mathf.Clamp((elapsed - updatedTime) / life, 0.0f, 1.0f);
    }

    private void Initialize()
    {
        // FPS設定
        Application.targetFrameRate = 60;
        
        // パラメータ初期化
        var fps = 1f / Time.deltaTime;
        //_nodeNumPerTrail = Mathf.CeilToInt((_nodeLifeTime + 1.0f) * fps);
        _nodeNumPerTrail = Mathf.CeilToInt((_nodeLifeTime + 1.0f) * 60f);
        _boidsGPU = GetComponent<BoidsGPU>();
        _instanceCount = _boidsGPU.InstanceCount;
        var totalNodeNum = _instanceCount * _nodeNumPerTrail;
        
        // バッファの初期化
        _trailBuffer = new ComputeBuffer(_instanceCount, Marshal.SizeOf(typeof(Trail)));
        _inputBuffer = new ComputeBuffer(_instanceCount, Marshal.SizeOf(typeof(InputData)));
        _nodeBuffer = new ComputeBuffer(totalNodeNum, Marshal.SizeOf(typeof(Node)));

        _trailArray = new List<Trail>();
        _inputArray = new List<InputData>();
        _nodeArray = new List<Node>();
        for (int i = 0; i < _instanceCount; i++)
        {
            _trailArray.Add(new Trail()
            {
                CurrentNodeIdx = -1,
                ElapsedTime = 0,
                LineDensity = Random.Range(5f, 20f),
                NoiseDensity = Random.Range(0.5f, 1f)
            });
            _inputArray.Add(new InputData());
        }
        for (int i = 0; i < totalNodeNum; i++) _nodeArray.Add(new Node() { UpdatedTime = -1f });
        _trailBuffer.SetData(_trailArray);
        _inputBuffer.SetData(_inputArray);
        _nodeBuffer.SetData(_nodeArray);
    }

    private void CalcInput()
    {
        var cs = _computeShader;
        var inputKernel = _computeShader.FindKernel("CalcInput");
        var threadGroupNum = Mathf.CeilToInt((float)_instanceCount / ThreadNum);
        
        cs.SetInt("_InstanceCount", _instanceCount);
        cs.SetFloat("_NodeNumPerTrail", _nodeNumPerTrail);
        cs.SetFloat("_UpdatedTime", Time.time);
        cs.SetFloat("_NodeLifeTime", _nodeLifeTime);
        cs.SetBuffer(inputKernel, "_TrailBuffer", _trailBuffer);
        cs.SetBuffer(inputKernel, "_NodeBuffer", _nodeBuffer);
        cs.SetBuffer(inputKernel, "_InputBuffer", _inputBuffer);
        
        cs.Dispatch(inputKernel, threadGroupNum, 1, 1);
    }

    private void UpdateInput()
    {
        _boidsGPU.UpdateInput(_inputBuffer);
    }

    public void AddTrail(int addValue)
    {
        _instanceCount += addValue;
        var totalNodeNum = _instanceCount * _nodeNumPerTrail;
        
        var prevTrailArray = new Trail[_trailArray.Count];
        _trailBuffer.GetData(prevTrailArray);
        _trailArray = prevTrailArray.ToList();
        
        var prevInputArray = new InputData[_inputArray.Count];
        _inputBuffer.GetData(prevInputArray);
        _inputArray = prevInputArray.ToList();
        
        var prevNodeArray = new Node[_nodeArray.Count];
        _nodeBuffer.GetData(prevNodeArray);
        _nodeArray = prevNodeArray.ToList();
        
        for (int i = 0; i < addValue; i++)
        {
            _trailArray.Add(new Trail()
            {
                CurrentNodeIdx = -1,
                ElapsedTime = 0,
                LineDensity = Random.Range(5f, 20f),
                NoiseDensity = Random.Range(0.5f, 1f)
            });
            _inputArray.Add(new InputData());
        }
        for(int i = 0; i < addValue * _nodeNumPerTrail; i++) _nodeArray.Add(new Node() { UpdatedTime = -1f });

        _trailBuffer = new ComputeBuffer(_instanceCount, Marshal.SizeOf(typeof(Trail)));
        _inputBuffer = new ComputeBuffer(_instanceCount, Marshal.SizeOf(typeof(InputData)));
        _nodeBuffer = new ComputeBuffer(totalNodeNum, Marshal.SizeOf(typeof(Node)));

        _trailBuffer.SetData(_trailArray);
        _inputBuffer.SetData(_inputArray);
        _nodeBuffer.SetData(_nodeArray);
    }

    private void OnDestroy()
    {
        _trailBuffer?.Release();
        _inputBuffer?.Release();
        _nodeBuffer?.Release();
    }
}
