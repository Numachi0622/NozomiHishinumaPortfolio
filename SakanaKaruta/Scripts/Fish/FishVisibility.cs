using System;
using UnityEngine;

public class FishVisibility : MonoBehaviour
{
    private FishVisibilityModel model;
    public Action OnDisableAction;

    private void OnDisable()
    {
        OnDisableAction?.Invoke();
    }

    public void SetVisibilityModel(FishVisibilityModel model)
    {
        this.model = model;
    }
}
