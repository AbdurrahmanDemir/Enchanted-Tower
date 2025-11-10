using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyBaseData", menuName = "Enemy Base/EnemyBaseData")]

public class GridEnemyBaseData : ScriptableObject
{
    [Header("Level Settings")]
    public string baseName;
    [TextArea] public string cardDescription;
    public string episodeDate;
    public LevelType levelType;

    //[Header("Base Settings")]
    //public int gridSize;       
    //[System.Serializable]
    //public class GridSlot
    //{
    //    public int x;
    //    public int y;
    //    public BaseEntityType entityType;
    //    public BaseEntitySO entityData;
    //}

    //public GridSlot[] entitiesInGrid;
}
public enum BaseEntityType
{
    Empty,
    Enemy,
    Tower,
    Boss
}
public enum LevelType
{
    EnemyBase,
    Bandits,
    ResourceRegion
}

