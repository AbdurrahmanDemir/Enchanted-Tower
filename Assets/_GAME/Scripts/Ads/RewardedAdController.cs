using GoogleMobileAds.Api;
using System;
using UnityEngine;

public class RewardedAdController : MonoBehaviour
{
    // Bu kimlikleri kendi AdMob kimliklerinizle deðiþtirmeyi unutmayýn!
#if UNITY_ANDROID
    private string _adUnitId = "ca-app-pub-9662935799337911/6137007511";
#elif UNITY_IPHONE
    private string _adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
    private string _adUnitId = "unused";
#endif

    public RewardedAd _rewardedAd;

    private void Start()
    {
        LoadRewardedAd();
    }

    /// <summary>
    /// Ödüllü reklamý yükler.
    /// </summary>
    public void LoadRewardedAd()
    {
        // Önceki reklamý temizle.
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        Debug.Log("Ödüllü reklam yükleniyor...");

        // Reklam isteðini oluþtur.
        var adRequest = new AdRequest();

        // Reklamý yükleme isteðini gönder.
        RewardedAd.Load(_adUnitId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                // Hata kontrolü.
                if (error != null || ad == null)
                {
                    Debug.LogError("Ödüllü reklam yüklenemedi: " + error);
                    return;
                }

                Debug.Log("Ödüllü reklam baþarýyla yüklendi.");
                _rewardedAd = ad;

                // ?? ÖNERÝLEN DEÐÝÞÝKLÝK: Reklam yüklendikten hemen sonra olaylarý kaydet.
                RegisterEventHandlers(_rewardedAd);
            });
    }

    /// <summary>
    /// Yüklü reklamý gösterir.
    /// </summary>
    public void ShowRewardedAd()
    {
        const string rewardMsg = "Kullanýcý ödüllendirildi. Tür: {0}, Miktar: {1}.";

        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            _rewardedAd.Show((Reward reward) =>
            {
                // ?? BURASI ÇOK ÖNEMLÝ: KULLANICIYA ÖDÜLÜNÜ VERDÝÐÝNÝZ YER
                Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));
                // Örneðin: GameManager.Instance.AddCoins((int)reward.Amount);
            });
        }
        else
        {
            Debug.LogError("Ödüllü reklam gösterilmeye hazýr deðil veya yüklenemedi.");
        }
    }

    private void RegisterEventHandlers(RewardedAd ad)
    {
        // Reklamý kapattýktan sonra yeni reklamý otomatik yüklemek için kritik olay.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Ödüllü reklam tam ekran içeriði kapandý.");
            LoadRewardedAd(); // Reklam kapandýktan sonra hemen yeni reklam yükle.
        };

        // Hata durumunda yeni reklamý otomatik yüklemek için kritik olay.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Ödüllü reklam tam ekran içeriði gösterilemedi: " + error);
            LoadRewardedAd(); // Hata olsa bile yeni reklam yüklemeyi dene.
        };

        // Diðer olay iþleyicileri (isteðe baðlý, ama bilgilendirici)
        ad.OnAdPaid += (adValue) =>
        {
            Debug.Log(String.Format("Ödüllü reklamdan kazanýlan deðer: {0} {1}",
                adValue.Value, adValue.CurrencyCode));
        };
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Ödüllü reklam gösterimi kaydedildi.");
        };
        ad.OnAdClicked += () =>
        {
            Debug.Log("Ödüllü reklama týklandý.");
        };
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Ödüllü reklam tam ekran içeriði açýldý.");
        };
    }
}