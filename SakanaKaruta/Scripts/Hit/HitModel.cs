using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class HitModel : MonoBehaviour
{
    private ReactiveProperty<Vector3> hitPos = new ReactiveProperty<Vector3>();
    public IObservable<Vector3> HitPos => hitPos;
    private List<Collider2D> hitColliders = new List<Collider2D>();
    public List<Collider2D> HitColliders => hitColliders;
    private bool isSetPosPossible = true;

    public void SetPos(Vector3 pos)
    {
        if (!isSetPosPossible)
        {
            return;
        }
        hitPos.Value = pos;
    }

    public void SetPosPossible(bool isPossible)
    {
        isSetPosPossible = isPossible;
    }
}
