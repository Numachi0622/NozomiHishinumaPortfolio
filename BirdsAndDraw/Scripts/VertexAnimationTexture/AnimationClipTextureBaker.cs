using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif
public class AnimationClipTextureBaker : MonoBehaviour
{
    [SerializeField] private ComputeShader _computeShader;
    [SerializeField] private Shader _animShader;
    [SerializeField] private AnimationClip[] _clips;
    
    public struct VertData
    {
        public Vector3 Position;
        public Vector3 Normal;
    }
    
    /// <summary>
    /// GameObjectにアタッチされたときによばれる
    /// </summary>
    private void Reset()
    {
        var animation = GetComponent<Animation>();
        var animator = GetComponent<Animator>();

        if (animation != null)
        {
            _clips = new AnimationClip[animation.GetClipCount()];
            for (int i = 0; i < _clips.Length; i++)
            {
                _clips[i] = animation.clip;
            }
        }
        else
        {
            _clips = animator.runtimeAnimatorController.animationClips;
        }
    }

    /// <summary>
    /// テクスチャベイク
    /// </summary>
    [ContextMenu("Bake Texture")]
    public void Bake()
    {
        var skin = GetComponentInChildren<SkinnedMeshRenderer>();
        var vCount = skin.sharedMesh.vertexCount;
        var texWidth = Mathf.NextPowerOfTwo(vCount);
        var mesh = new Mesh();

        foreach (var clip in _clips)
        {
            var frames = Mathf.NextPowerOfTwo((int)(clip.length / 0.05f));
            var dt = clip.length / frames;
            var infoList = new List<VertData>();

            var pRt = new RenderTexture(texWidth, frames, 0, RenderTextureFormat.ARGBHalf);
            pRt.name = string.Format("{0}.{1}.posTex", name, clip.name);
            var nRt = new RenderTexture(texWidth, frames, 0, RenderTextureFormat.ARGBHalf);
            nRt.name = string.Format("{0}.{1}.normTex", name, clip.name);
            foreach (var rt in new[] { pRt, nRt })
            {
                rt.enableRandomWrite = true;
                rt.Create();
                RenderTexture.active = rt;
                GL.Clear(true, true, Color.clear);
            }

            for (var i = 0; i < frames; i++)
            {
                clip.SampleAnimation(gameObject, dt * i);
                skin.BakeMesh(mesh);

                infoList.AddRange(Enumerable.Range(0, vCount)
                    .Select(idx => new VertData()
                    {
                        Position = mesh.vertices[idx],
                        Normal = mesh.normals[idx]
                    })
                );
            }
            var buffer = new ComputeBuffer(infoList.Count, System.Runtime.InteropServices.Marshal.SizeOf(typeof(VertData)));
            buffer.SetData(infoList.ToArray());

            var kernel = _computeShader.FindKernel("CSMain");
            uint x, y, z;
            _computeShader.GetKernelThreadGroupSizes(kernel, out x, out y, out z);

            _computeShader.SetInt("_VertCount", vCount);
            _computeShader.SetBuffer(kernel, "_MeshDataBuffer", buffer);
            _computeShader.SetTexture(kernel, "_OutPosition", pRt);
            _computeShader.SetTexture(kernel, "_OutNormal", nRt);
            _computeShader.Dispatch(kernel, vCount / (int)x + 1, frames / (int)y + 1, 1);

            buffer.Release();

#if UNITY_EDITOR
            var folderName = "BakingSkinnedAnimationToTexture/BakedAnimationTex";
            var folderPath = Path.Combine("Assets", folderName);
            //if (!AssetDatabase.IsValidFolder(folderPath))
                //AssetDatabase.CreateFolder("Assets", folderName);

            var subFolder = name;
            var subFolderPath = Path.Combine(folderPath, subFolder);
            //if (!AssetDatabase.IsValidFolder(subFolderPath))
                //AssetDatabase.CreateFolder(folderPath, subFolder);

            var posTex = RenderTextureToTexture2D.Convert(pRt);
            var normTex = RenderTextureToTexture2D.Convert(nRt);
            Graphics.CopyTexture(pRt, posTex);
            Graphics.CopyTexture(nRt, normTex);

            var mat = new Material(_animShader);
            mat.SetTexture("_MainTex", skin.sharedMaterial.mainTexture);
            mat.SetTexture("_PosTex", posTex);
            mat.SetTexture("_NmlTex", normTex);
            mat.SetFloat("_Length", clip.length);
            if (clip.wrapMode == WrapMode.Loop)
            {
                mat.SetFloat("_Loop", 1f);
                mat.EnableKeyword("ANIM_LOOP");
            }

            var go = new GameObject(name + "." + clip.name);
            go.AddComponent<MeshRenderer>().sharedMaterial = mat;
            go.AddComponent<MeshFilter>().sharedMesh = skin.sharedMesh;

            const string path = "Assets/Texture";
            AssetDatabase.CreateAsset(posTex, Path.Combine(path, pRt.name + ".asset"));
            AssetDatabase.CreateAsset(normTex, Path.Combine(path, nRt.name + ".asset"));
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }
    }
}
