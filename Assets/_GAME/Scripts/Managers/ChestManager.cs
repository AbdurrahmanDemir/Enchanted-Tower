using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;


public class ChestManager : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private GameObject rewardPopUp;
    [SerializeField] private GameObject rewardPopUpShop;
    [SerializeField] private GameObject rewardContainerPrefab;
    [SerializeField] private Transform rewardContainersParent;
    [SerializeField] private Transform rewardContainersParentShop;
    [SerializeField] private Sprite rewardGoldIcon;
    [SerializeField] private Sprite rewardEnergyIcon;
    [Header("Wooden Chest Settings")]
    [SerializeField] private int[] woodenChestGoldPossibilities;
    [SerializeField] private int[] woodenChestEnergyPossibilities;
    [Header("silverChest Settings")]
    [SerializeField] private int[] silverChestGoldPossibilities;
    [SerializeField] private int[] silverChestEnergyPossibilities;
    [Header("Legendary Settings")]
    [SerializeField] private int[] legendaryBoxGoldPossibilities;
    [SerializeField] private int[] legendaryBoxEnergyPossibilities;




    public void WoodenChest()
    {

        rewardContainersParent.Clear();

        int randomType = Random.Range(0, 2);
        int randomGoldNumber = Random.Range(0, woodenChestGoldPossibilities.Length);
        int randomEnergyNumber = Random.Range(0, woodenChestEnergyPossibilities.Length);
        TogglePanel(rewardPopUp);

        switch (randomType)
        {
            case 0:
                GameObject containerInstance = Instantiate(rewardContainerPrefab, rewardContainersParent);
                Image targetImage = containerInstance.transform.GetChild(0).GetComponent<Image>();
                targetImage.sprite = rewardGoldIcon;
                containerInstance.GetComponentInChildren<TextMeshProUGUI>().text = woodenChestGoldPossibilities[randomGoldNumber].ToString();

                DataManager.instance.AddGold(woodenChestGoldPossibilities[randomGoldNumber]);
                break;
            case 1:
                GameObject containerInstance2 = Instantiate(rewardContainerPrefab, rewardContainersParent);
                Image targetImage2 = containerInstance2.transform.GetChild(0).GetComponent<Image>();
                targetImage2.sprite = rewardEnergyIcon;

                containerInstance2.GetComponentInChildren<Image>().sprite = rewardEnergyIcon;
                containerInstance2.GetComponentInChildren<TextMeshProUGUI>().text = woodenChestEnergyPossibilities[randomEnergyNumber].ToString();

                DataManager.instance.AddEnergy(woodenChestEnergyPossibilities[randomEnergyNumber]);
                break;
        }

        GameObject button = EventSystem.current.currentSelectedGameObject;
        button.SetActive(false);


    }
    public void WoodenChestBuy()
    {
        if (DataManager.instance.TryPurchaseGold(100))
        {
            rewardContainersParent.Clear();

            int randomType = Random.Range(0, 2);
            int randomGoldNumber = Random.Range(0, woodenChestGoldPossibilities.Length);
            int randomEnergyNumber = Random.Range(0, woodenChestEnergyPossibilities.Length);
            TogglePanel(rewardPopUp);

            switch (randomType)
            {
                case 0:
                    GameObject containerInstance = Instantiate(rewardContainerPrefab, rewardContainersParent);
                    Image targetImage = containerInstance.transform.GetChild(0).GetComponent<Image>();
                    targetImage.sprite = rewardGoldIcon;
                    containerInstance.GetComponentInChildren<TextMeshProUGUI>().text = woodenChestGoldPossibilities[randomGoldNumber].ToString();

                    DataManager.instance.AddGold(woodenChestGoldPossibilities[randomGoldNumber]);
                    break;
                case 1:
                    GameObject containerInstance2 = Instantiate(rewardContainerPrefab, rewardContainersParent);
                    Image targetImage2 = containerInstance2.transform.GetChild(0).GetComponent<Image>();
                    targetImage2.sprite = rewardEnergyIcon;

                    containerInstance2.GetComponentInChildren<Image>().sprite = rewardEnergyIcon;
                    containerInstance2.GetComponentInChildren<TextMeshProUGUI>().text = woodenChestEnergyPossibilities[randomEnergyNumber].ToString();

                    DataManager.instance.AddEnergy(woodenChestEnergyPossibilities[randomEnergyNumber]);
                    break;
            }

            GameObject button = EventSystem.current.currentSelectedGameObject;
            button.SetActive(false);
        }




    }


    public void SilverChest()
    {
        //Bridge.advertisement.ShowRewarded();

        rewardContainersParent.Clear();

        int randomType = Random.Range(0, 2);
        int randomGoldNumber = Random.Range(0, silverChestGoldPossibilities.Length);
        int randomEnergyNumber = Random.Range(0, silverChestEnergyPossibilities.Length);
        TogglePanel(rewardPopUp);

        switch (randomType)
        {
            case 0:
                GameObject containerInstance = Instantiate(rewardContainerPrefab, rewardContainersParent);

                Image targetImage = containerInstance.transform.GetChild(0).GetComponent<Image>();
                targetImage.sprite = rewardGoldIcon;
                containerInstance.GetComponentInChildren<TextMeshProUGUI>().text = silverChestGoldPossibilities[randomGoldNumber].ToString();

                DataManager.instance.AddGold(silverChestGoldPossibilities[randomGoldNumber]);
                break;
            case 1:
                GameObject containerInstance2 = Instantiate(rewardContainerPrefab, rewardContainersParent);
                Image targetImage2 = containerInstance2.transform.GetChild(0).GetComponent<Image>();
                targetImage2.sprite = rewardEnergyIcon;
                containerInstance2.GetComponentInChildren<TextMeshProUGUI>().text = silverChestEnergyPossibilities[randomEnergyNumber].ToString();


                DataManager.instance.AddEnergy(silverChestEnergyPossibilities[randomEnergyNumber]);
                break;
        }

        GameObject button = EventSystem.current.currentSelectedGameObject;
        button.SetActive(false);


    }
    public void SilverChestBuy()
    {
        if (DataManager.instance.TryPurchaseGold(250))
        {
            rewardContainersParent.Clear();

            int randomType = Random.Range(0, 2);
            int randomGoldNumber = Random.Range(0, silverChestGoldPossibilities.Length);
            int randomEnergyNumber = Random.Range(0, silverChestEnergyPossibilities.Length);
            TogglePanel(rewardPopUp);

            switch (randomType)
            {
                case 0:
                    GameObject containerInstance = Instantiate(rewardContainerPrefab, rewardContainersParent);

                    Image targetImage = containerInstance.transform.GetChild(0).GetComponent<Image>();
                    targetImage.sprite = rewardGoldIcon;
                    containerInstance.GetComponentInChildren<TextMeshProUGUI>().text = silverChestGoldPossibilities[randomGoldNumber].ToString();

                    DataManager.instance.AddGold(silverChestGoldPossibilities[randomGoldNumber]);
                    break;
                case 1:
                    GameObject containerInstance2 = Instantiate(rewardContainerPrefab, rewardContainersParent);
                    Image targetImage2 = containerInstance2.transform.GetChild(0).GetComponent<Image>();
                    targetImage2.sprite = rewardEnergyIcon;
                    containerInstance2.GetComponentInChildren<TextMeshProUGUI>().text = silverChestEnergyPossibilities[randomEnergyNumber].ToString();


                    DataManager.instance.AddEnergy(silverChestEnergyPossibilities[randomEnergyNumber]);
                    break;
            }

            GameObject button = EventSystem.current.currentSelectedGameObject;
            button.SetActive(false);
        }





    }

    public void LegendaryBox()
    {

        rewardContainersParentShop.Clear();

        int randomType = Random.Range(0, 2);
        int randomGoldNumber = Random.Range(0, legendaryBoxGoldPossibilities.Length);
        int randomEnergyNumber = Random.Range(0, legendaryBoxEnergyPossibilities.Length);
        TogglePanel(rewardPopUpShop);

        switch (randomType)
        {
            case 0:
                GameObject containerInstance = Instantiate(rewardContainerPrefab, rewardContainersParentShop);


                Image targetImage = containerInstance.transform.GetChild(0).GetComponent<Image>();
                targetImage.sprite = rewardGoldIcon;
                containerInstance.GetComponentInChildren<TextMeshProUGUI>().text = legendaryBoxGoldPossibilities[randomGoldNumber].ToString();

                DataManager.instance.AddGold(legendaryBoxGoldPossibilities[randomGoldNumber]);
                break;
            case 1:
                GameObject containerInstance2 = Instantiate(rewardContainerPrefab, rewardContainersParentShop);
                Image targetImage2 = containerInstance2.transform.GetChild(0).GetComponent<Image>();
                targetImage2.sprite = rewardEnergyIcon;
                containerInstance2.GetComponentInChildren<TextMeshProUGUI>().text = legendaryBoxEnergyPossibilities[randomEnergyNumber].ToString();


                DataManager.instance.AddEnergy(legendaryBoxEnergyPossibilities[randomEnergyNumber]);
                break;
        }

        GameObject button = EventSystem.current.currentSelectedGameObject;
        button.SetActive(false);


    }

    public void TogglePanel(GameObject panel)
    {
        if (panel.activeSelf)
        {
            panel.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() => panel.SetActive(false));
        }
        else
        {
            panel.SetActive(true);
            panel.transform.localScale = Vector3.zero;
            panel.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
        }
    }

    //public void CardboardBoxPass()
    //{
    //    rewardContainersParent.Clear();

    //    int randomType = Random.Range(0, 2);
    //    int randomGoldNumber = Random.Range(0, cardboardGoldPossibilities.Length);
    //    int randomEnergyNumber = Random.Range(0, cardboardEnergyPossibilities.Length);
    //    MenuManager.instance.TogglePanel(rewardPopUp);

    //    switch (randomType)
    //    {
    //        case 0:
    //            UIRewardContainer containerInstance = Instantiate(rewardContainerPrefab, rewardContainersParent);
    //            containerInstance.Configure(rewardGoldIcon, cardboardGoldPossibilities[randomGoldNumber].ToString());
    //            DataManager.instance.AddGold(cardboardGoldPossibilities[randomGoldNumber]);
    //            break;
    //        case 1:
    //            UIRewardContainer containerInstance2 = Instantiate(rewardContainerPrefab, rewardContainersParent);
    //            containerInstance2.Configure(rewardEnergyIcon, cardboardEnergyPossibilities[randomEnergyNumber].ToString());
    //            DataManager.instance.AddEnergy(cardboardEnergyPossibilities[randomEnergyNumber]);
    //            break;
    //    }

    //}
    //public void GoldBoxPass()
    //{
    //    rewardContainersParent.Clear();

    //    int randomType = Random.Range(0, 2);
    //    int randomGoldNumber = Random.Range(0, goldBoxGoldPossibilities.Length);
    //    int randomEnergyNumber = Random.Range(0, goldBoxEnergyPossibilities.Length);
    //    MenuManager.instance.TogglePanel(rewardPopUp);

    //    switch (randomType)
    //    {
    //        case 0:
    //            UIRewardContainer containerInstance = Instantiate(rewardContainerPrefab, rewardContainersParent);
    //            containerInstance.Configure(rewardGoldIcon, goldBoxGoldPossibilities[randomGoldNumber].ToString());
    //            DataManager.instance.AddGold(goldBoxGoldPossibilities[randomGoldNumber]);
    //            break;
    //        case 1:
    //            UIRewardContainer containerInstance2 = Instantiate(rewardContainerPrefab, rewardContainersParent);
    //            containerInstance2.Configure(rewardEnergyIcon, goldBoxEnergyPossibilities[randomEnergyNumber].ToString());
    //            DataManager.instance.AddEnergy(goldBoxEnergyPossibilities[randomEnergyNumber]);
    //            break;
    //    }



    //}


}
