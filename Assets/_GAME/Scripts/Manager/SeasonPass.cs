using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeasonPass : MonoBehaviour
{
    [SerializeField] private ChestManager chestManager;
    [SerializeField] private RewardedAdController rewardedAdController;
    [Header("Pass")]
    public PassSegment[] freeSeasonPass;
    public PassSegment[] goldenSeasonPass;

    [Header("Elements")]
    public GameObject freePassPrefabs;
    public GameObject goldenPassPrefabs;

    public Transform freePassTransform;
    public Transform goldenPassTransform;

    public Slider trophySlider;

    [Header("Reward Icons")]
    [SerializeField] private Sprite goldImage;
    [SerializeField] private Sprite energyImage;
    [SerializeField] private Sprite heroUpgradeTokenImage;
    [SerializeField] private Sprite woodenChestImage;
    [SerializeField] private Sprite silverChestImage;
    [SerializeField] private Sprite goldenChestImage;
    [SerializeField] private Sprite epicChestImage;
    [SerializeField] private Sprite legendaryChestImage;

    [Header("Backgrounds")]
    [SerializeField] private Sprite freeRewardBackground;
    [SerializeField] private Sprite goldRewardBackground;

    [Header("Trophy Slider Milestones")]
    [SerializeField]
    private TrophyMilestone[] trophyMilestones = new TrophyMilestone[]
    {
        new TrophyMilestone { minTrophy = 0, maxTrophy = 4, sliderValue = 0 },
        new TrophyMilestone { minTrophy = 5, maxTrophy = 10, sliderValue = 35 },
        new TrophyMilestone { minTrophy = 11, maxTrophy = 15, sliderValue = 75 },
        new TrophyMilestone { minTrophy = 16, maxTrophy = 20, sliderValue = 110 },
        new TrophyMilestone { minTrophy = 21, maxTrophy = 30, sliderValue = 145 },
        new TrophyMilestone { minTrophy = 31, maxTrophy = 40, sliderValue = 180 },
        new TrophyMilestone { minTrophy = 41, maxTrophy = 50, sliderValue = 215 },
        new TrophyMilestone { minTrophy = 51, maxTrophy = 60, sliderValue = 250 },
        new TrophyMilestone { minTrophy = 61, maxTrophy = 70, sliderValue = 285 },
        new TrophyMilestone { minTrophy = 71, maxTrophy = 100, sliderValue = 320 },
        new TrophyMilestone { minTrophy = 101, maxTrophy = 150, sliderValue = 355 },
        new TrophyMilestone { minTrophy = 151, maxTrophy = 200, sliderValue = 390 },
        new TrophyMilestone { minTrophy = 201, maxTrophy = 250, sliderValue = 425 },
        new TrophyMilestone { minTrophy = 251, maxTrophy = 300, sliderValue = 460 },
        new TrophyMilestone { minTrophy = 301, maxTrophy = int.MaxValue, sliderValue = 500 }
    };

    private void Start()
    {
        InitializeTrophySlider();
        InitializePassRewards(freeSeasonPass, freePassTransform, freeRewardBackground, true);
        InitializePassRewards(goldenSeasonPass, goldenPassTransform, goldRewardBackground, false);
    }

    private void InitializeTrophySlider()
    {
        trophySlider.maxValue = 500;
        int myTrophy = PlayerPrefs.GetInt("XP", 0);

        foreach (var milestone in trophyMilestones)
        {
            if (myTrophy >= milestone.minTrophy && myTrophy <= milestone.maxTrophy)
            {
                trophySlider.value = milestone.sliderValue;
                break;
            }
        }
    }

    private void InitializePassRewards(PassSegment[] passSegments, Transform parent, Sprite background, bool isFreePass)
    {
        int myTrophy = PlayerPrefs.GetInt("XP", 0);
        string passKey = isFreePass ? "FreePassReward" : "GoldenPassReward";

        for (int i = 0; i < passSegments.Length; i++)
        {
            int currentIndex = i;
            bool isRewardClaimed = PlayerPrefs.HasKey(passKey + currentIndex);

            Sprite rewardSprite = GetRewardSprite(passSegments[currentIndex].rewardType);

            GameObject rewardObject = Instantiate(freePassPrefabs, parent);
            FreePassRewardPrefabs rewardComponent = rewardObject.GetComponent<FreePassRewardPrefabs>();

            rewardComponent.Config(background, rewardSprite, passSegments[currentIndex].rewardAmount, isRewardClaimed);

            if (!isFreePass)
            {
                rewardComponent.AdIcon().SetActive(true);
            }

            bool hasEnoughTrophies = myTrophy >= passSegments[currentIndex].requireTrophy;

            if (hasEnoughTrophies && !isRewardClaimed)
            {
                rewardComponent.GetClaimButton().onClick.AddListener(() =>
                {
                    if (isFreePass)
                    {
                        ClaimReward(passSegments[currentIndex].rewardType,
                                  currentIndex,
                                  passSegments[currentIndex].rewardAmount,
                                  rewardObject,
                                  passKey);
                    }
                    else
                    {
                        ClaimGoldenReward(passSegments[currentIndex].rewardType,
                                        currentIndex,
                                        passSegments[currentIndex].rewardAmount,
                                        rewardObject,
                                        passKey);
                    }
                });
            }
            else if (!hasEnoughTrophies)
            {
                rewardComponent.GetClaimButton().interactable = false;
            }
        }
    }

    private Sprite GetRewardSprite(PassReward rewardType)
    {
        switch (rewardType)
        {
            case PassReward.Gold:
                return goldImage;
            case PassReward.Energy:
                return energyImage;
            case PassReward.HeroUpgradeToken:
                return heroUpgradeTokenImage;
            case PassReward.WooodenChest:
                return woodenChestImage;
            case PassReward.SilverChest:
                return silverChestImage;
            case PassReward.GoldenChest:
                return goldenChestImage;
            case PassReward.EpicChest:
                return epicChestImage;
            case PassReward.LegendaryChest:
                return legendaryChestImage;
            default:
                return goldImage;
        }
    }

    private void ClaimReward(PassReward type, int index, int amount, GameObject prefabs, string passKey)
    {
        if (PlayerPrefs.HasKey(passKey + index))
        {
            PopUpController.instance.OpenPopUp("You've already received this reward.");
            return;
        }

        GiveReward(type, amount);

        PlayerPrefs.SetInt(passKey + index, 1);
        PlayerPrefs.Save();

        PopUpController.instance.OpenPopUp("YOU GOT THE AWARD!");
        prefabs.GetComponent<FreePassRewardPrefabs>().CheckIcon().SetActive(true);
    }

    private void ClaimGoldenReward(PassReward type, int index, int amount, GameObject prefabs, string passKey)
    {
        rewardedAdController.ShowRewardedAd();

        if (PlayerPrefs.HasKey(passKey + index))
        {
            PopUpController.instance.OpenPopUp("You've already received this reward.");
            return;
        }

        GiveReward(type, amount);

        PlayerPrefs.SetInt(passKey + index, 1);
        PlayerPrefs.Save();

        PopUpController.instance.OpenPopUp("YOU GOT THE AWARD!");
        prefabs.GetComponent<FreePassRewardPrefabs>().CheckIcon().SetActive(true);
    }

    private void GiveReward(PassReward type, int amount)
    {
        switch (type)
        {
            case PassReward.Gold:
                DataManager.instance.AddGold(amount);
                break;

            case PassReward.Energy:
                DataManager.instance.AddEnergy(amount);
                break;

            case PassReward.WooodenChest:
                chestManager.WoodenChestFree();
                break;

            case PassReward.SilverChest:
                chestManager.SilverChestFree();
                break;

            case PassReward.GoldenChest:
                chestManager.GoldenChestFree();
                break;

            case PassReward.EpicChest:
                chestManager.EpicChestFree();
                break;

            case PassReward.LegendaryChest:
                chestManager.LegendaryChestFree();
                break;
            case PassReward.HeroUpgradeToken:
                DataManager.instance.AddHeroToken(amount);
                break;
        }
    }
}

[System.Serializable]
public struct PassSegment
{
    public PassReward rewardType;
    public int rewardAmount;
    public int requireTrophy;
}

[System.Serializable]
public struct TrophyMilestone
{
    public int minTrophy;
    public int maxTrophy;
    public float sliderValue;
}

[System.Serializable]
public enum PassReward
{
    Gold = 0,
    Energy = 1,
    WooodenChest = 2,
    SilverChest = 3,
    GoldenChest = 4,
    EpicChest = 5,
    LegendaryChest = 6,
    HeroUpgradeToken=7
}