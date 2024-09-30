using TMPro;
using UnityEngine;

[CreateAssetMenu(menuName = "DynamicUIParam")]
public class DynamicUIParam : ScriptableObject
{
    public VertexGradient[] Gradient = new VertexGradient[4];
}
