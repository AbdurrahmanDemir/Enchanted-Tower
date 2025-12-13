using GoogleMobileAds.Api;
using System;
using UnityEngine;

public class InterstitialAdController : MonoBehaviour
{
#if UNITY_ANDROID
    private string _adUnitId = "ca-app-pub-9662935799337911/7044687080";
#elif UNITY_IPHONE
    private string _adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
    private string _adUnitId = "unused";
#endif

    private InterstitialAd _interstitialAd;

    private void Start()
    {
        LoadInterstitialAd();
    }

    public void LoadInterstitialAd()
    {
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        Debug.Log("Geçiþ reklamý yükleniyor...");

        var adRequest = new AdRequest();

        InterstitialAd.Load(_adUnitId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError("Geçiþ reklamý yüklenemedi: " + error);
                    return;
                }

                Debug.Log("Geçiþ reklamý baþarýyla yüklendi.");
                _interstitialAd = ad;
                RegisterEventHandlers(_interstitialAd);
            });
    }

    public void ShowInterstitialAd()
    {
        // Remove Ads satýn alýndýysa reklamý atlat
        if (RemoveAdsManager.Instance != null && RemoveAdsManager.Instance.IsAdsPurchased())
        {
            Debug.Log("Ads removed - Interstitial ad skipped");
            return;
        }

        // Reklam yüklü mü kontrol et
        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            Debug.Log("Showing Interstitial Ad");

            // ÖNEMLÝ: Reklamý göster
            _interstitialAd.Show();
        }
        else
        {
            Debug.LogError("Interstitial ad is not ready yet.");
            // Reklam hazýr deðilse yeniden yükle
            LoadInterstitialAd();
        }
    }

    private void RegisterEventHandlers(InterstitialAd interstitialAd)
    {
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Geçiþ reklamý tam ekran içeriði kapandý.");
            LoadInterstitialAd();
        };

        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Geçiþ reklamý tam ekran içeriði gösterilemedi: " + error);
            LoadInterstitialAd();
        };

        interstitialAd.OnAdPaid += (adValue) =>
        {
            Debug.Log(String.Format("Geçiþ reklamýndan kazanýlan deðer: {0} {1}",
                adValue.Value, adValue.CurrencyCode));
        };

        interstitialAd.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Geçiþ reklamý gösterimi kaydedildi.");
        };

        interstitialAd.OnAdClicked += () =>
        {
            Debug.Log("Geçiþ reklama týklandý.");
        };

        interstitialAd.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Geçiþ reklamý tam ekran içeriði açýldý.");
        };
    }
}