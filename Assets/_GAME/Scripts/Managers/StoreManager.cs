using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoreManager : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private RewardedAdController rewarded;
    [SerializeField] private ChestManager chestManager;

    [Header("Silver Chest Settings")]
     private int silverAdsWatched=0;
    [SerializeField] private TextMeshProUGUI silverAdsWatchedText;

    public void WoodenChestStore()
    {
        rewarded.ShowRewardedAd();
        chestManager.WoodenChestFree();
        UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.SetActive(false);

    }
    public void SilverChestStore()
    {

        if (rewarded == null)
        {
            Debug.LogError("RewardedAdController atanmadý!");
            return;
        }

        rewarded.ShowRewardedAd();
        silverAdsWatched++;
        silverAdsWatchedText.text = silverAdsWatched + "/" + 2;

        if (silverAdsWatched >= 2)
        {
            chestManager.SilverChestFree();
            silverAdsWatched = 0;
            silverAdsWatchedText.text = silverAdsWatched + "/" + 2;
            UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.SetActive(false);
        }

    }

    public void HeroUpgradeToken300()
    {
        if (DataManager.instance.TryPurchaseGold(1500))
        {
            DataManager.instance.AddHeroToken(300);
            PopUpController.instance.OpenPopUp("SUCCESSFULLY PURCHASED.");
        }
    }
    public void HeroUpgradeToken1500()
    {
        if (DataManager.instance.TryPurchaseGold(5000))
        {
            DataManager.instance.AddHeroToken(1500);
            PopUpController.instance.OpenPopUp("SUCCESSFULLY PURCHASED.");

        }
    }
    public void HeroUpgradeToken10000()
    {
        if (DataManager.instance.TryPurchaseGold(25000))
        {
            DataManager.instance.AddHeroToken(10000);
            PopUpController.instance.OpenPopUp("SUCCESSFULLY PURCHASED.");

        }
    }
    public void Gem50()
    {
        if (DataManager.instance.TryPurchaseGold(1000))
        {
            DataManager.instance.AddEnergy(50);
            PopUpController.instance.OpenPopUp("SUCCESSFULLY PURCHASED.");

        }
    }

    public void Gem550()
    {
        if (DataManager.instance.TryPurchaseGold(10000))
        {
            DataManager.instance.AddEnergy(550);
            PopUpController.instance.OpenPopUp("SUCCESSFULLY PURCHASED.");

        }
    }
    public void Gem2000()
    {
        if (DataManager.instance.TryPurchaseGold(30000))
        {
            DataManager.instance.AddEnergy(2000);
            PopUpController.instance.OpenPopUp("SUCCESSFULLY PURCHASED.");

        }
    }
}
