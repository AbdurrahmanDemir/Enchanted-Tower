using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave_", menuName = "Game/Wave Data", order = 1)]
public class WaveSO : ScriptableObject
{
    public string waveName;
    public List<WaveSegment> segments = new List<WaveSegment>();
}

[Serializable]
public class WaveSegment
{
    public float segmentDuration = 1f;
    public List<WaveEnemyGroup> enemyGroups = new List<WaveEnemyGroup>();
}

[Serializable]
public class WaveEnemyGroup
{
    public GameObject enemyPrefab;
    public EnemySO enemyLevel;
    public int enemyCount = 1;
}
