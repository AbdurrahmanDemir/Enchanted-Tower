using UnityEngine;

public class RemoveAdsManager : MonoBehaviour
{
    public static RemoveAdsManager Instance { get; private set; }

    private const string REMOVE_ADS_KEY = "RemoveAdsPurchased";

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Debug.Log($"RemoveAdsManager initialized. Ads purchased: {IsAdsPurchased()}");
    }

    public bool IsAdsPurchased()
    {
        bool isPurchased = PlayerPrefs.GetInt(REMOVE_ADS_KEY, 0) == 1;
        Debug.Log($"Checking ads purchase status: {isPurchased}");
        return isPurchased;
    }

    public void PurchaseRemoveAds()
    {
        PlayerPrefs.SetInt(REMOVE_ADS_KEY, 1);
        PlayerPrefs.Save();
        Debug.Log("Remove Ads purchased and saved successfully!");

        Debug.Log($"Verification - Ads purchased: {IsAdsPurchased()}");
    }

    // Test için - Inspector'dan çaðýrýlabilir
    [ContextMenu("Purchase Remove Ads (Test)")]
    public void TestPurchase()
    {
        PurchaseRemoveAds();
        PopUpController.instance?.OpenPopUp("REMOVE ADS PURCHASED (TEST)");
    }

    [ContextMenu("Reset Remove Ads (Test)")]
    public void ResetRemoveAds()
    {
        PlayerPrefs.DeleteKey(REMOVE_ADS_KEY);
        PlayerPrefs.Save();
        Debug.Log("Remove Ads reset!");
        PopUpController.instance?.OpenPopUp("REMOVE ADS RESET (TEST)");
    }

    [ContextMenu("Check Purchase Status")]
    public void CheckStatus()
    {
        Debug.Log($"=== Remove Ads Status ===");
        Debug.Log($"Purchased: {IsAdsPurchased()}");
        Debug.Log($"PlayerPrefs Value: {PlayerPrefs.GetInt(REMOVE_ADS_KEY, 0)}");
    }
}