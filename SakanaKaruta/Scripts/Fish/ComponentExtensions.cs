using UnityEngine;

public static class ComponentExtensions
{
    public static void RemoveComponent<T>(this Component self) where T : Component
    {
        GameObject.Destroy(self.GetComponent<T>());
    }
}
