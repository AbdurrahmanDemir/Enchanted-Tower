using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using GameAnalyticsSDK;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;
    public UIManager uiManager;
    public LevelMapManager levelMapManager;

    [Header("Elements")]
    public WaveUIManager waveUI;
    public Transform[] creatEnemyPosition;
    public Transform enemyParent;

    [Header("Wave Data")]
    public WaveSO[] waves;

    [Header("Settings")]
    public float segmentDelay = 2f;

    private WaveSO currentWave;
    private int currentWaveIndex;
    private int currentSegmentIndex;
    private int currentEnemyGroupIndex;
    private int currentEnemyCount;
    private float timer;
    private bool isTimerOn;
    private bool onThrow;
    private int aliveEnemyCount;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void OnEnable()
    {
        UpgradeSelectManager.onPowerUpPanelOpened += OnThrowStartingCallBack;
        UpgradeSelectManager.onPowerUpPanelClosed += OnThrowEndingCallBack;
    }

    private void OnDestroy()
    {
        UpgradeSelectManager.onPowerUpPanelOpened -= OnThrowStartingCallBack;
        UpgradeSelectManager.onPowerUpPanelClosed -= OnThrowEndingCallBack;
    }

    private void Update()
    {
        if (!isTimerOn)
            return;

        ManageCurrentWave();
    }

    public void StartWaves(int index)
    {
        currentWaveIndex = index;
        currentSegmentIndex = 0;
        currentEnemyGroupIndex = 0;
        aliveEnemyCount = 0;

        if (index >= waves.Length)
        {
            Debug.LogError("Wave index " + index + " is out of range. Total waves: " + waves.Length);
            return;
        }

        currentWave = waves[currentWaveIndex];
        isTimerOn = true;
        timer = 0;
        SetupNextSegment();
        waveUI.waveSegmentText.text = "Wave " + (currentSegmentIndex + 1) + " / " + currentWave.segments.Count;

        if (!PlayerPrefs.HasKey("Tutorial"))
        {
            TutorialManager.instance.tutorialPanel3.SetActive(true);
        }
    }

    private void ManageCurrentWave()
    {
        if (currentSegmentIndex >= currentWave.segments.Count)
        {
            isTimerOn = false;
            Debug.Log("All segments in the wave completed.");
            CheckWaveCompleted();
            return;
        }

        if (onThrow)
            return;

        WaveSegment currentSegment = currentWave.segments[currentSegmentIndex];

        timer += Time.deltaTime;

        if (timer >= currentSegment.segmentDuration)
        {
            if (SpawnEnemy(currentSegment))
            {
                timer = 0;
            }
            else
            {
                currentSegmentIndex++;
                Debug.Log("Moving to next segment. Current Index: " + currentSegmentIndex);

                if (currentSegmentIndex >= currentWave.segments.Count)
                {
                    isTimerOn = false;
                    Debug.Log("All segments in the wave completed.");
                    CheckWaveCompleted();
                    return;
                }

                waveUI.waveSegmentText.text = "Wave " + (currentSegmentIndex + 1) + " / " + currentWave.segments.Count;

                isTimerOn = false;
                Invoke("StartNextSegment", segmentDelay);
            }
        }
    }

    private void CheckWaveCompleted()
    {
        bool allSegmentsFinished = currentSegmentIndex >= currentWave.segments.Count;
        bool noEnemiesAlive = aliveEnemyCount <= 0;

        Debug.Log("CheckWaveCompleted -> segmentsFinished:" + allSegmentsFinished + ", noEnemiesAlive:" + noEnemiesAlive + ", alive:" + aliveEnemyCount);

        if (allSegmentsFinished && noEnemiesAlive)
        {
            Debug.Log("All waves completed.");

            if (uiManager != null)
            {
                int playingEpisode = PlayerPrefs.GetInt("PlayingEpisode", 0);
                int playingLevel = PlayerPrefs.GetInt("PlayingLevel", 0);

                Debug.Log("Tamamlanan Episode: " + playingEpisode + ", Level: " + playingLevel);

                int newLevel = playingLevel + 2;
                levelMapManager.SetCurrentLevelForEpisode(playingEpisode, newLevel);

                int totalLevelsInEpisode = GetTotalLevelsInEpisode(playingEpisode);

                if (playingLevel >= totalLevelsInEpisode - 1)
                {
                    Debug.Log("Episode " + playingEpisode + " tamamlandi! Yeni episode aciliyor...");

                    int newEpisodeIndex = playingEpisode + 1;
                    PlayerPrefs.SetInt("LevelEpisodeIndex", newEpisodeIndex);

                    levelMapManager.SetCurrentLevelForEpisode(newEpisodeIndex, 1);

                    PlayerPrefs.Save();

                    Debug.Log("Yeni episode acildi: " + newEpisodeIndex);
                }

                // GameAnalytics: Level Complete Event
                GameAnalytics.NewProgressionEvent(
                    GAProgressionStatus.Complete,
                    "Episode" + playingEpisode,
                    "Level" + playingLevel
                );

                uiManager.GameWinPanel();
            }
            else
            {
                Debug.LogError("uiManager atanmadi!");
            }
        }
    }

    private int GetTotalLevelsInEpisode(int episodeIndex)
    {
        if (levelMapManager != null)
        {
            return levelMapManager.GetEpisodeLevelCount(episodeIndex);
        }
        return 6;
    }

    private void StartNextSegment()
    {
        isTimerOn = true;
        timer = 0;
        Debug.Log("Starting next segment. Current Index: " + currentSegmentIndex);
        SetupNextSegment();
    }

    private void SetupNextSegment()
    {
        currentEnemyGroupIndex = 0;

        if (currentSegmentIndex < currentWave.segments.Count)
        {
            WaveSegment segment = currentWave.segments[currentSegmentIndex];
            if (segment.enemyGroups.Count > 0)
            {
                currentEnemyCount = segment.enemyGroups[currentEnemyGroupIndex].enemyCount;
                Debug.Log("Setting up next segment. Enemy Count: " + currentEnemyCount);
            }
            else
            {
                Debug.LogError("No enemies defined in the current segment.");
            }
        }
    }

    private bool SpawnEnemy(WaveSegment segment)
    {
        if (currentEnemyCount <= 0)
        {
            currentEnemyGroupIndex++;
            if (currentEnemyGroupIndex < segment.enemyGroups.Count)
            {
                currentEnemyCount = segment.enemyGroups[currentEnemyGroupIndex].enemyCount;
            }
            else
            {
                return false;
            }
        }

        if (currentEnemyGroupIndex >= segment.enemyGroups.Count)
        {
            Debug.LogError("Index out of range error.");
            return false;
        }

        WaveEnemyGroup enemyGroup = segment.enemyGroups[currentEnemyGroupIndex];

        if (enemyGroup.enemyPrefab == null)
        {
            Debug.LogError("Enemy prefab is null!");
            currentEnemyCount--;
            return true;
        }

        int randomCreatPos = Random.Range(0, creatEnemyPosition.Length);
        GameObject enemyInstance = Instantiate(
            enemyGroup.enemyPrefab,
            creatEnemyPosition[randomCreatPos].position,
            Quaternion.Euler(0f, 180f, 0f),
            enemyParent
        );

        Enemy enemy = enemyInstance.GetComponent<Enemy>();
        if (enemy != null && enemyGroup.enemyLevel != null)
        {
            enemy.Initialize(enemyGroup.enemyLevel);
        }

        aliveEnemyCount++;
        currentEnemyCount--;
        return true;
    }

    public void OnEnemyDied()
    {
        aliveEnemyCount = Mathf.Max(0, aliveEnemyCount - 1);
        CheckWaveCompleted();
    }

    public void OnThrowStartingCallBack()
    {
        onThrow = true;
        Time.timeScale = 1;
        Debug.Log("Action started: " + onThrow);
    }

    public void OnThrowEndingCallBack()
    {
        onThrow = false;
        Debug.Log("Action ended: " + onThrow);
    }

    public void StopAllWaves()
    {
        isTimerOn = false;
        
        currentSegmentIndex = 0;
        currentWaveIndex = 0;
        currentEnemyGroupIndex = 0;
        currentEnemyCount = 0;
        
        aliveEnemyCount = 0;
        
        timer = 0;
        
        onThrow = false;
        
    }
}
