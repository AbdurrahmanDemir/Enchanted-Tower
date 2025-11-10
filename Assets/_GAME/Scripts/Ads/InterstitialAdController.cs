using GoogleMobileAds.Api;
using System;
using UnityEngine;

public class InterstitialAdController : MonoBehaviour
{
    // Bu kimlikleri kendi AdMob kimliklerinizle deðiþtirmeyi unutmayýn!
#if UNITY_ANDROID
    private string _adUnitId = "ca-app-pub-9662935799337911/7044687080";
#elif UNITY_IPHONE
    private string _adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
    private string _adUnitId = "unused";
#endif

    private InterstitialAd _interstitialAd;

    /// <summary>
    /// Geçiþ reklamýný yükler.
    /// </summary>
    public void LoadInterstitialAd()
    {
        // Önceki reklamý temizle.
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        Debug.Log("Geçiþ reklamý yükleniyor...");

        // Reklam isteðini oluþtur.
        var adRequest = new AdRequest();

        // Reklamý yükleme isteðini gönder.
        InterstitialAd.Load(_adUnitId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                // Hata kontrolü.
                if (error != null || ad == null)
                {
                    Debug.LogError("Geçiþ reklamý yüklenemedi: " + error);
                    return;
                }

                Debug.Log("Geçiþ reklamý baþarýyla yüklendi.");
                _interstitialAd = ad;

                // ?? ÖNERÝLEN DEÐÝÞÝKLÝK: Reklam yüklendikten hemen sonra olaylarý kaydet.
                RegisterEventHandlers(_interstitialAd);
            });
    }

    /// <summary>
    /// Yüklü reklamý gösterir.
    /// </summary>
    public void ShowInterstitialAd()
    {
        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            Debug.Log("Geçiþ reklamý gösteriliyor.");
            _interstitialAd.Show();
        }
        else
        {
            Debug.LogError("Geçiþ reklamý gösterilmeye hazýr deðil veya yüklenemedi.");
        }
    }

    private void RegisterEventHandlers(InterstitialAd interstitialAd)
    {
        // Reklamý kapattýktan sonra yeni reklamý otomatik yüklemek için kritik olay.
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Geçiþ reklamý tam ekran içeriði kapandý.");
            LoadInterstitialAd(); // Reklam kapandýktan sonra hemen yeni reklam yükle.
        };

        // Hata durumunda yeni reklamý otomatik yüklemek için kritik olay.
        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Geçiþ reklamý tam ekran içeriði gösterilemedi: " + error);
            LoadInterstitialAd(); // Hata olsa bile yeni reklam yüklemeyi dene.
        };

        // Diðer olay iþleyicileri (isteðe baðlý)
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