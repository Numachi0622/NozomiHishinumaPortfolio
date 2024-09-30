using UnityEngine;

[CreateAssetMenu(menuName = "SerialData")]
public class SerialData : ScriptableObject
{
    public string Port;
    public int Bps;
}
