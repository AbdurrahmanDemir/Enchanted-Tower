using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Base", menuName = "EnemyBase/Tower")]

public class TowerData : ScriptableObject
{
    [Header("Tower Stats")]
    public string towerName;

    public int health;
    public float range;
    public float attackCooldown;
    public int damage;
    public TargetPriorityType targetingType;


    public int GetCurrentHealth()
    {
        return PlayerPrefs.GetInt($"{towerName}_Health", health);
    }
    public int GetCurrentDamage()
    {
        return PlayerPrefs.GetInt($"{towerName}_Damage", damage);
    }

}
public enum TargetPriorityType
{
    Closest,
    Random,
    HighestHealth
}
