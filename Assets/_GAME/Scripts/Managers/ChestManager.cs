using System.Collections;
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
    [SerializeField] private ChestConfig woodenChestConfig;
    [SerializeField] private ChestConfig silverChestConfig;
    [SerializeField] private ChestConfig goldenChestConfig;
    [SerializeField] private ChestConfig epicChestConfig;
    [SerializeField] private ChestConfig legendaryChestConfig;

    [Header("Hero Cards Pool")]
    [SerializeField] private MenuHeroCardSO[] availableHeroCards;

    private MenuHeroCardSO lastEarnedHero = null;

    public void WoodenChest() => OpenGoldChest(woodenChestConfig, false, rewardContainersParent, rewardPopUp);
    public void WoodenChestBuy() => OpenGoldChest(woodenChestConfig, true, rewardContainersParent, rewardPopUp);
    public void SilverChest() => OpenGoldChest(silverChestConfig, false, rewardContainersParent, rewardPopUp);
    public void SilverChestBuy() => OpenGoldChest(silverChestConfig, true, rewardContainersParent, rewardPopUp);

    public void GoldenChest() => OpenGemChest(goldenChestConfig, true, rewardContainersParentShop, rewardPopUpShop);
    public void EpicChest() => OpenGemChest(epicChestConfig, true, rewardContainersParentShop, rewardPopUpShop);
    public void LegendaryBox() => OpenGemChest(legendaryChestConfig, true, rewardContainersParentShop, rewardPopUpShop);

    public void GoldenChestFree() => OpenChest(goldenChestConfig, true, rewardContainersParentShop, rewardPopUpShop);
    public void EpicChestFree() => OpenChest(epicChestConfig, true, rewardContainersParentShop, rewardPopUpShop);
    public void LegendaryChestFree() => OpenChest(legendaryChestConfig, true, rewardContainersParentShop, rewardPopUpShop);

    private void OpenGoldChest(ChestConfig config, bool requiresPurchase, Transform containerParent, GameObject popUp)
    {
        if (requiresPurchase && !DataManager.instance.TryPurchaseGold(config.price)) return;

        containerParent.Clear();
        List<(RewardType type, int amount)> rewards = GenerateRewards(config);

        foreach (var r in rewards)
            GiveReward(r.type, r.amount);

        TogglePanel(popUp);
        StartCoroutine(ShowRewardsSequentially(rewards, containerParent));

        GameObject button = EventSystem.current.currentSelectedGameObject;
        if (button != null) button.SetActive(false);
    }

    private void OpenGemChest(ChestConfig config, bool requiresPurchase, Transform containerParent, GameObject popUp)
    {
        if (requiresPurchase && !DataManager.instance.TryPurchaseEnergy(config.price)) return;

        containerParent.Clear();
        List<(RewardType type, int amount)> rewards = GenerateRewards(config);

        foreach (var r in rewards)
            GiveReward(r.type, r.amount);

        TogglePanel(popUp);
        StartCoroutine(ShowRewardsSequentially(rewards, containerParent));
    }

    private void OpenChest(ChestConfig config, bool requiresPurchase, Transform containerParent, GameObject popUp)
    {
        containerParent.Clear();
        List<(RewardType type, int amount)> rewards = GenerateRewards(config);

        foreach (var r in rewards)
            GiveReward(r.type, r.amount);

        TogglePanel(popUp);
        StartCoroutine(ShowRewardsSequentially(rewards, containerParent));
    }

    private List<(RewardType type, int amount)> GenerateRewards(ChestConfig config)
    {
        List<(RewardType type, int amount)> list = new List<(RewardType, int)>();

        foreach (var rd in config.possibleRewards)
        {
            if (Random.Range(0f, 100f) <= rd.dropChance)
            {
                int amount = Random.Range(rd.minAmount, rd.maxAmount + 1);

                if (rd.rewardType == RewardType.RandomHeroCard)
                {
                    for (int i = 0; i < amount; i++)
                        list.Add((rd.rewardType, 1));
                }
                else list.Add((rd.rewardType, amount));
            }
        }

        if (list.Count == 0)
        {
            var f = config.possibleRewards[0];
            int amount = Random.Range(f.minAmount, f.maxAmount + 1);
            list.Add((f.rewardType, amount));
        }

        return list;
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
                List<MenuHeroCardSO> locked = new List<MenuHeroCardSO>();
                foreach (var hero in availableHeroCards)
                    if (!hero.IsPurchased()) locked.Add(hero);

                if (locked.Count > 0)
                {
                    MenuHeroCardSO chosen = locked[Random.Range(0, locked.Count)];
                    PlayerPrefs.SetInt($"{chosen.cardName}_Purchased", 1);
                    PlayerPrefs.Save();
                    lastEarnedHero = chosen;
                }
                else
                {
                    DataManager.instance.AddHeroToken(50);
                    lastEarnedHero = null;
                }
                break;
        }
    }

    private IEnumerator ShowRewardsSequentially(List<(RewardType type, int amount)> rewards, Transform parent)
    {
        foreach (var reward in rewards)
        {
            GameObject obj = CreateRewardUI(reward.type, reward.amount, parent);
            obj.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(0.25f);
        }
    }

    private GameObject CreateRewardUI(RewardType type, int amount, Transform parent)
    {
        GameObject obj = Instantiate(rewardContainerPrefab, parent);
        obj.transform.localScale = Vector3.zero;

        Image img = obj.transform.GetChild(0).GetComponent<Image>();
        TextMeshProUGUI txt = obj.GetComponentInChildren<TextMeshProUGUI>();
        img.sprite = GetRewardIcon(type);

        if (type == RewardType.RandomHeroCard)
        {
            if (lastEarnedHero != null)
            {
                img.sprite = lastEarnedHero.heroIcon;
                txt.text = lastEarnedHero.cardName;
            }
            else
            {
                img.sprite = rewardTokenIcon;
                txt.text = "50";
            }
        }
        else txt.text = amount.ToString();

        return obj;
    }

    private Sprite GetRewardIcon(RewardType type)
    {
        switch (type)
        {
            case RewardType.Gold: return rewardGoldIcon;
            case RewardType.Energy: return rewardEnergyIcon;
            case RewardType.HeroUpgradeToken: return rewardTokenIcon;
            case RewardType.RandomHeroCard: return rewardHeroCardIcon;
            default: return rewardGoldIcon;
        }
    }

    public void TogglePanel(GameObject panel)
    {
        if (panel.activeSelf)
        {
            panel.transform.DOScale(0f, 0.2f).SetEase(Ease.InBack).OnComplete(() => panel.SetActive(false));
        }
        else
        {
            panel.SetActive(true);
            panel.transform.localScale = Vector3.zero;
            panel.transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
        }
    }
}


