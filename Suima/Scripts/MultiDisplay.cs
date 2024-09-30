using UnityEngine;

public class MultiDisplay : MonoBehaviour
{
    private void Awake()
    {
        for (int i = 1; i < 3; i++)
        {
            Display.displays[i].Activate();
        }
    }
}