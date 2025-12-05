using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;


public class HeadquartersController : MonoBehaviour, IDamageable
{
    [Header("Settings")]
    public HqSO towerSO;
    int health;


    [Header("Elements")]
    private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;
    //[SerializeField] private TextMeshProUGUI menuHealthText;
    //[SerializeField] private TextMeshProUGUI upgradePriceText;
    SpriteRenderer towerSpriteRenderer;
    private Color originalColor;
    private Vector2 originalScale;
    public Vector2 scaleReduction = new Vector3(0.9f, 0.9f, 1f);

    public static Action onGameLose;
    public static Action onTowerUpgrade;

    //public void Awake()
    //{
    //    UpgradeSelectManager.towerHealthItem += TowerHealthUpgradeItem;
    //}
    //private void OnDestroy()
    //{
    //    UpgradeSelectManager.towerHealthItem -= TowerHealthUpgradeItem;
    //}

    private void Start()
    {
        towerSpriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = towerSpriteRenderer.color;
        originalScale = transform.localScale;

        healthSlider = GetComponentInChildren<Slider>();
        healthSlider.maxValue = towerSO.GetCurrentHealth();
        health = towerSO.GetCurrentHealth();
        healthSlider.value = health;
        healthText.text = health.ToString();
        //menuHealthText.text = health.ToString() + "<color=green> +100 </color>";
        //upgradePriceText.text = towerSO.GetUpgradeCost().ToString();
    }
    public void ResetTower()
    {
        towerSpriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = towerSpriteRenderer.color;
        originalScale = transform.localScale;

        healthSlider = GetComponentInChildren<Slider>();
        healthSlider.maxValue = towerSO.GetCurrentHealth();
        health = towerSO.GetCurrentHealth();
        healthSlider.value = health;
        healthText.text = health.ToString();
        //menuHealthText.text = health.ToString() + "<color=green> +100 </color>";
        //upgradePriceText.text = towerSO.GetUpgradeCost().ToString();


    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        healthSlider.value = health;
        healthText.text = health.ToString();


        towerSpriteRenderer.DOColor(Color.gray, 0.1f).OnComplete(() =>
        {
            towerSpriteRenderer.DOColor(originalColor, 0.1f).SetDelay(0.1f);
        });
        transform.DOScale(originalScale * scaleReduction, 0.1f).OnComplete(() =>
        {
            transform.DOScale(originalScale, 0.1f);
        });


        if (health <= 0)
        {
            onGameLose?.Invoke();
        }
    }

    //public void TowerUpgrade()
    //{
    //    if (HookManager.instance.TryPurchaseToken(HookManager.instance.costs[HookManager.instance.offlineEarnings - 3]))
    //    {
    //        HookManager.instance.BuyOfflineEarnings();

    //        health += 100;
    //        healthSlider.value = health;
    //        healthText.text = health.ToString();


    //        towerSpriteRenderer.DOColor(Color.gray, 0.1f).OnComplete(() =>
    //        {
    //            towerSpriteRenderer.DOColor(originalColor, 0.1f).SetDelay(0.1f);
    //        });
    //        transform.DOScale(originalScale * scaleReduction, 0.1f).OnComplete(() =>
    //        {
    //            transform.DOScale(originalScale, 0.1f);
    //        });
    //    }

    //}

    public void TowerMenuUpgrade()
    {
        if (DataManager.instance.TryPurchaseGold(towerSO.GetUpgradeCost()))
        {
            towerSO.UpgradeDamage();
            //menuHealthText.text = health.ToString();
            onTowerUpgrade?.Invoke();
            ResetTower();
        }
    }

    public void TowerHealthUpgradeItem(int healthAmount)
    {
        health += healthAmount;
        healthSlider.value = health;
        healthText.text = health.ToString();


        towerSpriteRenderer.DOColor(Color.gray, 0.1f).OnComplete(() =>
        {
            towerSpriteRenderer.DOColor(originalColor, 0.1f).SetDelay(0.1f);
        });
        transform.DOScale(originalScale * scaleReduction, 0.1f).OnComplete(() =>
        {
            transform.DOScale(originalScale, 0.1f);
        });
    }

    public int GetCurrentHealth() { return health; }
    public TeamType GetTeam() => TeamType.Building;

}
