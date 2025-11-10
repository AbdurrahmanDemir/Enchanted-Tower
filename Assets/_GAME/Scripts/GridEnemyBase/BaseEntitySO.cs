using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Base", menuName = "EnemyBase/Entities")]
public class BaseEntitySO :ScriptableObject
{
    public string entityName;
    public GameObject prefab;
}
