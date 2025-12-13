using GoogleMobileAds.Api;
using System;
using UnityEngine;

public class RewardedAdController : MonoBehaviour
{
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

    public void LoadRewardedAd()
    {
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        Debug.Log("Ödüllü reklam yükleniyor...");

        var adRequest = new AdRequest();

        RewardedAd.Load(_adUnitId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError("Ödüllü reklam yüklenemedi: " + error);
                    return;
                }

                Debug.Log("Ödüllü reklam baþarýyla yüklendi.");
                _rewardedAd = ad;
                RegisterEventHandlers(_rewardedAd);
            });
    }

    public void ShowRewardedAd()
    {
        // Remove Ads satýn alýndýysa direkt ödül ver
        if (RemoveAdsManager.Instance != null && RemoveAdsManager.Instance.IsAdsPurchased())
        {
            Debug.Log("Ads removed - Auto-rewarding player");
            OnRewardedAdSuccess();
            return;
        }

        // Reklam yüklü mü kontrol et
        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            Debug.Log("Showing Rewarded Ad");

            // ÖNEMLÝ: Reklamý göster
            _rewardedAd.Show((Reward reward) =>
            {
                // Kullanýcý reklamý izledi, ödül ver
                Debug.Log($"Rewarded ad watched! Reward: {reward.Amount} {reward.Type}");
                OnRewardedAdSuccess();
            });
        }
        else
        {
            Debug.LogError("Rewarded ad is not ready yet.");
            // Reklam hazýr deðilse yeniden yükle
            LoadRewardedAd();
        }
    }

    private void OnRewardedAdSuccess()
    {
        Debug.Log("Player rewarded!");
        // Buraya ödül verme kodunuzu ekleyin
        // Örneðin: DataManager.instance.AddGold(100);
    }

    private void RegisterEventHandlers(RewardedAd ad)
    {
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Ödüllü reklam tam ekran içeriði kapandý.");
            LoadRewardedAd();
        };

        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Ödüllü reklam tam ekran içeriði gösterilemedi: " + error);
            LoadRewardedAd();
        };

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