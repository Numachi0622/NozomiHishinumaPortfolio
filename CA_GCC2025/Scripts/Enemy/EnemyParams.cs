using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
using Utility;

[CreateAssetMenu(menuName = "EnemyParams")]
public class EnemyParams : ScriptableObject
{
    public BasePower BasePower;
    public float MaxSpeed;
    public int MaxHp;
    public float AttackRange;
    public float AttackReadyTime;
    public float AttackCoolTime;
    public float HitStopDuration;
    public float SkillHitStopDuration;
    public Color DamagedBodyColor;
    public Color DeadBodyColor;
}
