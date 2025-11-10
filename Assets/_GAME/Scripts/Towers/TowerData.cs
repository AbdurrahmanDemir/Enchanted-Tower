using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Base", menuName = "EnemyBase/Tower")]

public class TowerData : ScriptableObject
{
    [Header("Tower Stats")]
    public int health;
    public float range;
    public float attackCooldown;
    public int damage;
    public TargetPriorityType targetingType;
}
public enum TargetPriorityType
{
    Closest,
    Random,
    HighestHealth
}
