using UnityEngine;

public class BoidsRender : MonoBehaviour
{
    [SerializeField] private Vector3 _size;
    [SerializeField] private Mesh _mesh;
    [SerializeField] private Material _material;
    [SerializeField] private AnimationClip _animationClip;
    private BoidsGPU _boidsGPU;
    
    /// <summary>
    /// GPUインスタンシングのための引数
    /// インスタンスあたりのインデックス数, インスタンス数, 開始インデックス位置, ベース頂点位置, インスタンスの開始位置
    /// </summary>
    private uint[] _args = new uint[5] { 0, 0, 0, 0, 0 };
    
    /// <summary>
    /// GPUインスタンシングのための引数バッファ
    /// </summary>
    private ComputeBuffer _argsBuffer;
    
    private void Start()
    {
        // 引数バッファを初期化
        _argsBuffer = new ComputeBuffer(1, _args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        
        _boidsGPU = GetComponent<BoidsGPU>();
    }

    private void Update()
    {
        uint numIndices = _mesh.GetIndexCount(0);
        _args[0] = numIndices;
        _args[1] = (uint)_boidsGPU.InstanceCount;
        _argsBuffer.SetData(_args);
        
        _material.SetBuffer("_BoidDataBuffer", _boidsGPU.BoidDataBuffer);
        _material.SetVector("_Size", _size);
        _material.SetFloat("_AnimLength", _animationClip.length);
        _material.SetFloat("_DeltaTime", Time.deltaTime);
        
        var bounds = new Bounds(_boidsGPU.WallCenter, _boidsGPU.WallSize);
        
        // GPUインスタンシング
        Graphics.DrawMeshInstancedIndirect(_mesh, 0, _material, bounds, _argsBuffer);
    }

    private void OnDestroy()
    {
        _argsBuffer?.Release();
    }
}