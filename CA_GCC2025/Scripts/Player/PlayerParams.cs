using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements.Experimental;
using Utility;

[CreateAssetMenu(menuName = "PlayerParams")]
public class PlayerParams : ScriptableObject
{
    public BasePower BasePower;
    public float MaxSpeed;
    public int MaxHp;
    public int MaxComboCount;
    public float AttackCoolTime;
    public float SearchRange;
    public int MaxSearchCount;
    public float AttackRotateTime;
    public float MaxDashMoveDist;
    public float DashAttackTime;
    public float DashAttackOffset;
    public float IncreaseSpValue;
    public float CriticalRate;
    public CriticalMagnification CriticalMagnification;
}
