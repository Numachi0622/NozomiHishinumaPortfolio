using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    [SerializeField] int id;
    public int Id { get => id; }

    [SerializeField] private string fishName;
    public string Name { get => fishName; }

    [SerializeField] private string explanation;
    public string Explanation { get => explanation; }
}
