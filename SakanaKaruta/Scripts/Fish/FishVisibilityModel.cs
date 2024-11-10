using System;
using System.Collections.Generic;
using UnityEngine;

public class FishVisibilityModel : MonoBehaviour
{
    private List<FishVisibility> fishVisibilityList = new List<FishVisibility>();
    public List<FishVisibility> FishVisibilityList => fishVisibilityList;
    public Action OnCompleteSetList = null;

    public void OnCompleteSetVisibilityList()
    {
        OnCompleteSetList?.Invoke();
    }
}
