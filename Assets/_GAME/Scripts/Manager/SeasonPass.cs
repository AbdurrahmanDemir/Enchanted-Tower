using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeasonPass : MonoBehaviour
{
    [SerializeField] private ChestManager chestManager;

    [Header("Pass")]
    public PassSegment[] freeSeasonPass;
    public PassSegment[] goldenSeasonPass;

    [Header("Elements")]
    public GameObject freePassPrefabs;
    public GameObject goldenPassPrefabs;

    public Transform freePassTransform;
    public Transform goldenPassTransform;

    public Slider trophySlider;

    [Header("Other Elements")]
    [SerializeField] private Sprite goldImage;
    [SerializeField] private Sprite energyImage;
    [SerializeField] private Sprite bronzeBoxImage;
    [SerializeField] private Sprite goldBoxImage;
    [SerializeField] private Sprite freeRewardBackground;
    [SerializeField] private Sprite goldRewardBackground;

    Sprite rewardSprite;
    bool rewardState;
    private void Start()
    {
        trophySlider.maxValue = 500;
        int myTrophy = PlayerPrefs.GetInt("XP", 0);

        if (myTrophy >= 5 && myTrophy <= 10)
            trophySlider.value = 35;
        else if (myTrophy >= 11 && myTrophy <= 15)
            trophySlider.value = 75;
        else if (myTrophy >= 16 && myTrophy <= 20)
            trophySlider.value = 110;
        else if (myTrophy >= 21 && myTrophy <= 30)
            trophySlider.value = 145;
        else if (myTrophy >= 31 && myTrophy <= 40)
            trophySlider.value = 180;
        else if (myTrophy >= 41 && myTrophy <= 50)
            trophySlider.value = 215;
        else if (myTrophy >= 51 && myTrophy <= 60)
            trophySlider.value = 250;
        else if (myTrophy >= 71 && myTrophy <= 100)
            trophySlider.value = 285;
        else if (myTrophy >= 101 && myTrophy <= 150)
            trophySlider.value = 320;
        else if (myTrophy >= 151 && myTrophy <= 200)
            trophySlider.value = 355;
        else if (myTrophy >= 201 && myTrophy <= 250)
            trophySlider.value = 390;
        else if (myTrophy >= 251 && myTrophy <= 300)
            trophySlider.value = 425;
        else if (myTrophy >= 301 && myTrophy <= 500)
            trophySlider.value = 500;



        for (int i = 0; i < freeSeasonPass.Length; i++)
        {
            int currentIndex = i;

            if (PlayerPrefs.HasKey("FreePassReward" + currentIndex))
                rewardState = true;
            else
                rewardState = false;

            switch (freeSeasonPass[currentIndex].rewardType)
            {
                case PassReward.Gold:
                    rewardSprite = goldImage;
                    break;
                case PassReward.Energy:
                    rewardSprite = energyImage;
                    break;
                case PassReward.BronzeBox:
                    rewardSprite = bronzeBoxImage;
                    break;
                case PassReward.GoldenBox:
                    rewardSprite = goldBoxImage;
                    break;
                default:
                    break;
            }

            GameObject freeReward = Instantiate(freePassPrefabs, freePassTransform);
            freeReward.GetComponent<FreePassRewardPrefabs>().Config(freeRewardBackground, rewardSprite, freeSeasonPass[currentIndex].rewardAmount, rewardState);

            if (myTrophy >= freeSeasonPass[currentIndex].requireTrophy)
            {
                freeReward.GetComponent<FreePassRewardPrefabs>().GetClaimButton().onClick
                .AddListener(() => FreeRewardClaim(freeSeasonPass[currentIndex].rewardType,
                                                   currentIndex,
                                                   freeSeasonPass[currentIndex].rewardAmount,
                                                   freeReward));

            }
            else
            {
                PopUpController.instance.OpenPopUp("You don't have enough trophies.");
            }


        }

        for (int i = 0; i < goldenSeasonPass.Length; i++)
        {
            int currentIndex = i;

            if (PlayerPrefs.HasKey("GoldenPassReward" + currentIndex))
                rewardState = true;
            else
                rewardState = false;

            switch (goldenSeasonPass[currentIndex].rewardType)
            {
                case PassReward.Gold:
                    rewardSprite = goldImage;
                    break;
                case PassReward.Energy:
                    rewardSprite = energyImage;
                    break;
                case PassReward.BronzeBox:
                    rewardSprite = bronzeBoxImage;
                    break;
                case PassReward.GoldenBox:
                    rewardSprite = goldBoxImage;
                    break;
                default:
                    break;
            }

            GameObject goldenReward = Instantiate(freePassPrefabs, goldenPassTransform);
            goldenReward.GetComponent<FreePassRewardPrefabs>().Config(goldRewardBackground, rewardSprite, goldenSeasonPass[currentIndex].rewardAmount, rewardState);
            goldenReward.GetComponent<FreePassRewardPrefabs>().AdIcon().SetActive(true);

            if (myTrophy >= freeSeasonPass[currentIndex].requireTrophy)
            {
                goldenReward.GetComponent<FreePassRewardPrefabs>().GetClaimButton().onClick
                .AddListener(() => GoldenRewardClaim(goldenSeasonPass[currentIndex].rewardType,
                                                     currentIndex,
                                                     goldenSeasonPass[currentIndex].rewardAmount,
                                                     goldenReward));



            }
            else
            {
                PopUpController.instance.OpenPopUp("You don't have enough trophies.");
            }


        }
    }

    public void FreeRewardClaim(PassReward type, int index, int amount, GameObject prefabs)
    {

        if (!PlayerPrefs.HasKey("FreePassReward" + index))
        {
            switch (type)
            {
                case PassReward.Gold:
                    DataManager.instance.AddGold(amount);
                    break;
                case PassReward.Energy:
                    DataManager.instance.AddEnergy(amount);
                    break;
                case PassReward.BronzeBox:
                    chestManager.SilverChest();
                    break;
                case PassReward.GoldenBox:
                    chestManager.SilverChest();
                    break;
                default:
                    break;
            }
            PlayerPrefs.SetInt("FreePassReward" + index, 1);
            PopUpController.instance.OpenPopUp("YOU GOT THE AWARD!");
            prefabs.GetComponent<FreePassRewardPrefabs>().CheckIcon().SetActive(true);
        }
        else
        {
            PopUpController.instance.OpenPopUp("You've already received this reward.");
        }

    }

    public void GoldenRewardClaim(PassReward type, int index, int amount, GameObject prefabs)
    {

        if (!PlayerPrefs.HasKey("GoldenPassReward" + index))
        {

                switch (type)
                {
                    case PassReward.Gold:
                        DataManager.instance.AddGold(amount);
                        break;
                    case PassReward.Energy:
                        DataManager.instance.AddEnergy(amount);
                        break;
                    case PassReward.BronzeBox:
                        chestManager.SilverChest();
                        break;
                    case PassReward.GoldenBox:
                        chestManager.SilverChest();
                        break;
                    default:
                        break;
                }
                PlayerPrefs.SetInt("GoldenPassReward" + index, 1);
            PopUpController.instance.OpenPopUp("YOU GOT THE AWARD!");
                prefabs.GetComponent<FreePassRewardPrefabs>().CheckIcon().SetActive(true);


        }
        else
        {
            PopUpController.instance.OpenPopUp("You've already received this reward.");
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
public enum PassReward
{
    Gold = 0,
    Energy = 1,
    BronzeBox = 2,
    GoldenBox = 3
}