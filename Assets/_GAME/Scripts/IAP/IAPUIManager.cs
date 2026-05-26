using UnityEngine;
using UnityEngine.Purchasing;
using System;

public class IAPUIManager : MonoBehaviour
{
    [Header("UI Pack Elements")]
    [SerializeField] private GameObject startedPack1;
    [SerializeField] private GameObject startedPack2;
    [SerializeField] private GameObject starPack1;
    [SerializeField] private GameObject adsPack;

    [Header("Managers")]
    [SerializeField] private ChestManager chestManager;
    [SerializeField] private SeasonPass seasonPass;

    private void Start()
    {
        UpdatePackVisibility();
    }

    private void OnEnable()
    {
        UpdatePackVisibility();
        
        if (IAPCore.Instance == null)
        {
            Debug.LogError("IAPUIManager: IAPCore not found! IAP will not work.");
        }
        else
        {
            Debug.Log("IAPUIManager: IAPCore found and ready");
        }
    }

    private void UpdatePackVisibility()
    {
        if (startedPack1 != null)
        {
            if (!PlayerPrefs.HasKey("startedpack1"))
                startedPack1.SetActive(true);
            else
                startedPack1.SetActive(false);
        }

        if (startedPack2 != null)
        {
            if (!PlayerPrefs.HasKey("startedpack2"))
                startedPack2.SetActive(true);
            else
                startedPack2.SetActive(false);
        }

        if (starPack1 != null)
        {
        }

        // Ads Pack
        if (adsPack != null)
        {
            if (!PlayerPrefs.HasKey("adsPack"))
                adsPack.SetActive(true);
            else
                adsPack.SetActive(false);
        }
    }

    public PurchaseProcessingResult HandlePurchase(PurchaseEventArgs e, string[] productIds)
    {
        string productId = e.purchasedProduct.definition.id;

        // Gold paketleri
        if (string.Equals(productId, productIds[0], StringComparison.Ordinal)) // 1000 altın
        {
            DataManager.instance.AddGold(1000);
            return PurchaseProcessingResult.Complete;
        }
        else if (string.Equals(productId, productIds[1], StringComparison.Ordinal)) // 6250 altın
        {
            DataManager.instance.AddGold(6250);
            return PurchaseProcessingResult.Complete;
        }
        else if (string.Equals(productId, productIds[2], StringComparison.Ordinal)) // 15000 altın
        {
            DataManager.instance.AddGold(15000);
            return PurchaseProcessingResult.Complete;
        }
        else if (string.Equals(productId, productIds[3], StringComparison.Ordinal)) // 31250 altın
        {
            DataManager.instance.AddGold(31250);
            return PurchaseProcessingResult.Complete;
        }
        else if (string.Equals(productId, productIds[4], StringComparison.Ordinal)) // 81250 altın
        {
            DataManager.instance.AddGold(81250);
            return PurchaseProcessingResult.Complete;
        }
        else if (string.Equals(productId, productIds[5], StringComparison.Ordinal)) // 175000 altın
        {
            DataManager.instance.AddGold(175000);
            return PurchaseProcessingResult.Complete;
        }
        // Starter Pack
        else if (string.Equals(productId, productIds[6], StringComparison.Ordinal))
        {
            DataManager.instance.AddGold(1000);
            DataManager.instance.AddEnergy(50);
            
            if (chestManager != null)
                chestManager.EpicChestFree();
            
            if (startedPack1 != null)
                startedPack1.SetActive(false);
            
            PlayerPrefs.SetInt("startedpack1", 1);
            return PurchaseProcessingResult.Complete;
        }
        // Legendary Pack
        else if (string.Equals(productId, productIds[7], StringComparison.Ordinal))
        {
            DataManager.instance.AddEnergy(300);
            DataManager.instance.AddHeroToken(3000);
            
            if (chestManager != null)
                chestManager.LegendaryChestFree();
            
            return PurchaseProcessingResult.Complete;
        }
        // Remove Ads Pack
        else if (string.Equals(productId, productIds[8], StringComparison.Ordinal))
        {
            OnRemoveAdsPurchaseSuccess();
            
            if (adsPack != null)
                adsPack.SetActive(false);
            
            PlayerPrefs.SetInt("adsPack", 1);
            return PurchaseProcessingResult.Complete;
        }
        // Golden Pass
        else if (string.Equals(productId, productIds[9], StringComparison.Ordinal))
        {
            OnGoldenPassPurchaseSuccess();
            PlayerPrefs.SetInt("goldenpass", 1);
            return PurchaseProcessingResult.Complete;
        }
        else
        {
            Debug.LogWarning($"IAPUIManager: Unknown product: {productId}");
            return PurchaseProcessingResult.Pending;
        }
    }

    private void OnRemoveAdsPurchaseSuccess()
    {
        if (RemoveAdsManager.Instance != null)
        {
            RemoveAdsManager.Instance.PurchaseRemoveAds();
            PopUpController.instance?.OpenPopUp("ADS REMOVED SUCCESSFULLY!");
        }
    }

    private void OnGoldenPassPurchaseSuccess()
    {
        if (seasonPass != null)
        {
            seasonPass.PurchaseGoldenPass();
        }
    }

    public void IAPButton(string productId)
    {
        if (IAPCore.Instance != null)
        {
            IAPCore.Instance.PurchaseProduct(productId);
        }
        else
        {
            Debug.LogError("IAPUIManager: IAPCore not found!");
            PopUpController.instance?.OpenPopUp("Purchase system not available!");
        }
    }
}
