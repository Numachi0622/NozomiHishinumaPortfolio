using System.Collections.Generic;
using UnityEngine;

public class MouseClickModel
{ 
    private List<Collider2D> hitColliders = new List<Collider2D>();
    public List<Collider2D> HitColliders => hitColliders;
}
