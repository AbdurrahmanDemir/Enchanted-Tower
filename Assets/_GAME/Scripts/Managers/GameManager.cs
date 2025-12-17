using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private UpgradeSelectManager upgradeSelectManager;
    [SerializeField] private RewardedAdController rewarded;
    [Header("Settings")]
    [SerializeField] private Slider powerUpSlider;
    [SerializeField] private int[] powerUpLevel;
    int powerUpIndex=0;

    public static int enemyCount=0;

    private float savedTimeScale = 1f;
    public float SavedTimeScale => savedTimeScale;

    private void Awake()
    {
        Enemy.onDead += PowerUpSliderUpdate;
        Tower.onDead += PowerUpSliderUpdate;
    }
    private void OnDestroy()
    {
        Enemy.onDead -= PowerUpSliderUpdate;
        Tower.onDead -= PowerUpSliderUpdate;

    }
    private void Start()
    {
        powerUpSlider.value = 0;
        powerUpSlider.maxValue = powerUpLevel[powerUpIndex];
    }
    

    public void GameSpeedController()
    {
        if (Time.timeScale == 1)
        {
            if (AdManager.Instance != null && AdManager.Instance.ShouldShowAds())
            {
                rewarded.ShowRewardedAd();
            }

            Time.timeScale = 2;
            savedTimeScale = 2f;
        }
        else
        {
            Time.timeScale = 1;
            savedTimeScale = 1f; 
        }

        Debug.Log($"Game speed changed to: {Time.timeScale}x (Saved: {savedTimeScale}x)");
    }

    public void PowerUpSliderUpdate(Vector2 createPosition)
    {
        powerUpSlider.value++;

        if(powerUpSlider.value >= powerUpSlider.maxValue)
        {
            powerUpIndex++;
            powerUpSlider.maxValue = powerUpLevel[powerUpIndex];
            SaveCurrentGameSpeed();
            upgradeSelectManager.PowerUpPanelOpen();
            powerUpSlider.value = 0;
        }
    }
    public void PowerUpReset()
    {
        powerUpIndex = 0;
        powerUpSlider.value = 0;
        powerUpSlider.maxValue = powerUpLevel[powerUpIndex];
    }
    public void SaveCurrentGameSpeed()
    {
        savedTimeScale = Time.timeScale;
    }

    public void RestoreSavedGameSpeed()
    {
        Time.timeScale = savedTimeScale;
    }
}
