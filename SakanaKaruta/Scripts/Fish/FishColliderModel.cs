using System.Collections.Generic;
using UnityEngine;

public class FishColliderModel : MonoBehaviour
{
    private List<Collider2D> fishColliders = new List<Collider2D>();
    public List<Collider2D> FishColliders => fishColliders;
}
