using GoogleMobileAds.Api;
using UnityEngine;

public class AdManager : MonoBehaviour
{
    private static bool _isInitialized = false;
    public static AdManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (!_isInitialized)
        {
            MobileAds.Initialize((InitializationStatus initStatus) =>
            {
                Debug.Log("AdMob SDK baþlatýldý.");
                _isInitialized = true;
            });
        }
    }

    public bool ShouldShowAds()
    {
        if (RemoveAdsManager.Instance == null)
            return true;

        return !RemoveAdsManager.Instance.IsAdsPurchased();
    }
}