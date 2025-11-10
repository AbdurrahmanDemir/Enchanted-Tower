using GoogleMobileAds.Api;
using UnityEngine;

public class AdManager : MonoBehaviour
{
    private static bool _isInitialized = false;

    void Awake()
    {
        if (_isInitialized) return;
        DontDestroyOnLoad(gameObject);

        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            Debug.Log("AdMob SDK baþlatýldý (tek sefer).");
        });

        _isInitialized = true;
    }
}
