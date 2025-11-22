using UnityEngine;
using System;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using DG.Tweening;

public class UpgradeSelectManager : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private UpgradeSelectSO[] upgradeData;
    [SerializeField] private GameObject powerUpPanel;
    [Header("Buttons")]
    [SerializeField] private GameObject buttonPrefabs;
    [SerializeField] private Transform buttonTransform;

    [Header("Action")]
    public static Action onPowerUpPanelOpened;
    public static Action onPowerUpPanelClosed;
    public static Action<int> addGold;
    public static Action<int> addCapacity;
    public static Action hookStranghtItem;
    public static Action<int> addUpgradeToken;
    public static Action<int> heroDamageItem;
    public static Action<int> heroHealthItem;

    public void GetUpgrade()
    {
        buttonTransform.Clear();

        for (int i = 0; i < 3; i++)
        {
            GameObject buttonInstance = Instantiate(buttonPrefabs, buttonTransform);
            int randomTypes = Random.Range(0, upgradeData.Length);

            buttonInstance.GetComponent<UpgradeSelectButton>().Config(upgradeData[randomTypes].upgradeBg,upgradeData[randomTypes].upgradeIcon, upgradeData[randomTypes].upgradeName, upgradeData[randomTypes].upgradeDescription);

            switch (upgradeData[randomTypes].upgradeType)
            {
                case UpgradeType.AddGold:
                    buttonInstance.GetComponent<UpgradeSelectButton>().GetButton().onClick
                .AddListener(() => AddGold(upgradeData[randomTypes].amount));

                    break;
                case UpgradeType.AddCapacity:
                    buttonInstance.GetComponent<UpgradeSelectButton>().GetButton().onClick
                .AddListener(() => AddCapacity(upgradeData[randomTypes].amount));

                    break;
                case UpgradeType.AddUpgradeToken:
                    buttonInstance.GetComponent<UpgradeSelectButton>().GetButton().onClick
                .AddListener(() => HookStrangthItem());

                    break;
                case UpgradeType.DamageUpgrade:
                    buttonInstance.GetComponent<UpgradeSelectButton>().GetButton().onClick
                .AddListener(() => HeroDamageItem(upgradeData[randomTypes].amount));
                    break;
                case UpgradeType.HealthUpgrade:
                    buttonInstance.GetComponent<UpgradeSelectButton>().GetButton().onClick
                .AddListener(() => HeroHealthItem(upgradeData[randomTypes].amount));
                    break;

                default:
                    break;
            }
        }
    }

    public void AddGold(int amount)
    {
        addGold?.Invoke(amount);
        DataManager.instance.AddGold(amount);
        PowerUpPanelOpen();

    }
    public void AddCapacity(int amount)
    {
        addCapacity?.Invoke(amount);
        PowerUpPanelOpen();

    }
    public void HookStrangthItem()
    {
        hookStranghtItem?.Invoke();
        PowerUpPanelOpen();

    }
    public void AddUpgradeToken(int amount)
    {
        addUpgradeToken?.Invoke(amount);
        DataManager.instance.AddHeroToken(amount);
        PowerUpPanelOpen();
    }
    public void HeroDamageItem(int amount)
    {
        heroDamageItem?.Invoke(amount);
        PowerUpPanelOpen();
    }
    public void HeroHealthItem(int amount)
    {
        heroHealthItem?.Invoke(amount);
        PowerUpPanelOpen();
    }

    public void PowerUpPanelOpen()
    {
        if (powerUpPanel.activeSelf)
        {
            powerUpPanel.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() => powerUpPanel.SetActive(false));
            onPowerUpPanelClosed?.Invoke();
        }
        else
        {
            powerUpPanel.SetActive(true);
            powerUpPanel.transform.localScale = Vector3.zero;
            powerUpPanel.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
            GetUpgrade();
            onPowerUpPanelOpened?.Invoke();
        }
    }
}
