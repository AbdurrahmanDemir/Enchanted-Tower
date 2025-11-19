using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

[System.Serializable]
public class RewardData
{
    public RewardType rewardType;
    public int minAmount;
    public int maxAmount;
    [Range(0f, 100f)] public float dropChance = 100f;
}

[System.Serializable]
public class ChestConfig
{
    public string chestName;
    public int price;
    public List<RewardData> possibleRewards;
}

public enum RewardType
{
    Gold,
    Energy,
    HeroUpgradeToken,
    RandomHeroCard
}

public class ChestManager : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private GameObject rewardPopUp;
    [SerializeField] private GameObject rewardPopUpShop;
    [SerializeField] private GameObject rewardContainerPrefab;
    [SerializeField] private Transform rewardContainersParent;
    [SerializeField] private Transform rewardContainersParentShop;

    [Header("Reward Icons")]
    [SerializeField] private Sprite rewardGoldIcon;
    [SerializeField] private Sprite rewardEnergyIcon;
    [SerializeField] private Sprite rewardTokenIcon;
    [SerializeField] private Sprite rewardHeroCardIcon;

    [Header("Chest Configurations")]
    [SerializeField]
    private ChestConfig woodenChestConfig = new ChestConfig
    {
        chestName = "Wooden Chest",
        price = 100,
        possibleRewards = new List<RewardData>
        {
            new RewardData { rewardType = RewardType.Gold, minAmount = 50, maxAmount = 150, dropChance = 70f },
            new RewardData { rewardType = RewardType.Energy, minAmount = 1, maxAmount = 3, dropChance = 60f },
            new RewardData { rewardType = RewardType.HeroUpgradeToken, minAmount = 5, maxAmount = 15, dropChance = 40f },
            new RewardData { rewardType = RewardType.RandomHeroCard, minAmount = 1, maxAmount = 1, dropChance = 10f }
        }
    };

    [SerializeField]
    private ChestConfig silverChestConfig = new ChestConfig
    {
        chestName = "Silver Chest",
        price = 250,
        possibleRewards = new List<RewardData>
        {
            new RewardData { rewardType = RewardType.Gold, minAmount = 150, maxAmount = 300, dropChance = 80f },
            new RewardData { rewardType = RewardType.Energy, minAmount = 3, maxAmount = 7, dropChance = 70f },
            new RewardData { rewardType = RewardType.HeroUpgradeToken, minAmount = 15, maxAmount = 35, dropChance = 60f },
            new RewardData { rewardType = RewardType.RandomHeroCard, minAmount = 1, maxAmount = 1, dropChance = 30f }
        }
    };

    [SerializeField]
    private ChestConfig legendaryChestConfig = new ChestConfig
    {
        chestName = "Legendary Chest",
        price = 500,
        possibleRewards = new List<RewardData>
        {
            new RewardData { rewardType = RewardType.Gold, minAmount = 400, maxAmount = 800, dropChance = 90f },
            new RewardData { rewardType = RewardType.Energy, minAmount = 7, maxAmount = 15, dropChance = 85f },
            new RewardData { rewardType = RewardType.HeroUpgradeToken, minAmount = 40, maxAmount = 80, dropChance = 80f },
            new RewardData { rewardType = RewardType.RandomHeroCard, minAmount = 1, maxAmount = 2, dropChance = 60f }
        }
    };

    [Header("Hero Cards Pool")]
    [SerializeField] private MenuHeroCardSO[] availableHeroCards;

    public void WoodenChest() => OpenChest(woodenChestConfig, false, rewardContainersParent, rewardPopUp);
    public void WoodenChestBuy() => OpenChest(woodenChestConfig, true, rewardContainersParent, rewardPopUp);
    public void SilverChest() => OpenChest(silverChestConfig, false, rewardContainersParent, rewardPopUp);
    public void SilverChestBuy() => OpenChest(silverChestConfig, true, rewardContainersParent, rewardPopUp);
    public void LegendaryBox() => OpenChest(legendaryChestConfig, true, rewardContainersParentShop, rewardPopUpShop);

    private void OpenChest(ChestConfig config, bool requiresPurchase, Transform containerParent, GameObject popUp)
    {
        if (requiresPurchase && !DataManager.instance.TryPurchaseGold(config.price))
        {
            return;
        }

        containerParent.Clear();

        List<(RewardType type, int amount)> earnedRewards = GenerateRewards(config);

        foreach (var reward in earnedRewards)
        {
            GiveReward(reward.type, reward.amount);
            CreateRewardUI(reward.type, reward.amount, containerParent);
        }

        TogglePanel(popUp);

        GameObject button = EventSystem.current.currentSelectedGameObject;
        if (button != null)
        {
            button.SetActive(false);
        }
    }

    private List<(RewardType type, int amount)> GenerateRewards(ChestConfig config)
    {
        List<(RewardType type, int amount)> rewards = new List<(RewardType type, int amount)>();

        foreach (var rewardData in config.possibleRewards)
        {
            float randomChance = Random.Range(0f, 100f);
            if (randomChance <= rewardData.dropChance)
            {
                int amount = Random.Range(rewardData.minAmount, rewardData.maxAmount + 1);

                if (rewardData.rewardType == RewardType.RandomHeroCard)
                {
                    for (int i = 0; i < amount; i++)
                    {
                        rewards.Add((rewardData.rewardType, 1));
                    }
                }
                else
                {
                    rewards.Add((rewardData.rewardType, amount));
                }
            }
        }

        if (rewards.Count == 0)
        {
            var fallbackReward = config.possibleRewards[0];
            int amount = Random.Range(fallbackReward.minAmount, fallbackReward.maxAmount + 1);
            rewards.Add((fallbackReward.rewardType, amount));
        }

        return rewards;
    }

    private void GiveReward(RewardType type, int amount)
    {
        switch (type)
        {
            case RewardType.Gold:
                DataManager.instance.AddGold(amount);
                break;

            case RewardType.Energy:
                DataManager.instance.AddEnergy(amount);
                break;

            case RewardType.HeroUpgradeToken:
                DataManager.instance.AddHeroToken(amount);
                break;

            case RewardType.RandomHeroCard:
                if (availableHeroCards != null && availableHeroCards.Length > 0)
                {
                    MenuHeroCardSO randomHero = availableHeroCards[Random.Range(0, availableHeroCards.Length)];
                    // Hero card sisteminize göre kart ekleme iþlemi
                    // Örnek: HeroCardManager.Instance.AddCard(randomHero, 1);
                    Debug.Log($"Hero Card kazanýldý: {randomHero.cardName}");
                }
                break;
        }
    }

    private void CreateRewardUI(RewardType type, int amount, Transform parent)
    {
        GameObject containerInstance = Instantiate(rewardContainerPrefab, parent);
        Image targetImage = containerInstance.transform.GetChild(0).GetComponent<Image>();
        TextMeshProUGUI amountText = containerInstance.GetComponentInChildren<TextMeshProUGUI>();

        Sprite icon = GetRewardIcon(type);
        targetImage.sprite = icon;

        if (type == RewardType.RandomHeroCard && availableHeroCards.Length > 0)
        {
            MenuHeroCardSO randomHero = availableHeroCards[Random.Range(0, availableHeroCards.Length)];
            amountText.text = randomHero.cardName;
        }
        else
        {
            amountText.text = amount.ToString();
        }
    }

    private Sprite GetRewardIcon(RewardType type)
    {
        switch (type)
        {
            case RewardType.Gold:
                return rewardGoldIcon;
            case RewardType.Energy:
                return rewardEnergyIcon;
            case RewardType.HeroUpgradeToken:
                return rewardTokenIcon;
            case RewardType.RandomHeroCard:
                return rewardHeroCardIcon;
            default:
                return rewardGoldIcon;
        }
    }

    public void TogglePanel(GameObject panel)
    {
        if (panel.activeSelf)
        {
            panel.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack)
                .OnComplete(() => panel.SetActive(false));
        }
        else
        {
            panel.SetActive(true);
            panel.transform.localScale = Vector3.zero;
            panel.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
        }
    }
}