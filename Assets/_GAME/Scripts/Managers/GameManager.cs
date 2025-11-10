using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private UpgradeSelectManager upgradeSelectManager;

    [Header("Settings")]
    [SerializeField] private Slider powerUpSlider;
    [SerializeField] private int[] powerUpLevel;
    int powerUpIndex=0;

    public static int enemyCount=0;

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
            Time.timeScale = 2;
        else
            Time.timeScale = 1;
    }

    public void PowerUpSliderUpdate(Vector2 createPosition)
    {
        powerUpSlider.value++;

        if(powerUpSlider.value >= powerUpSlider.maxValue)
        {
            powerUpIndex++;
            powerUpSlider.maxValue = powerUpLevel[powerUpIndex];
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

}
