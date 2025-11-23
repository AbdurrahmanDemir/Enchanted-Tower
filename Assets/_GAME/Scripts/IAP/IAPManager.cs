using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using System;
using Unity.VisualScripting;
using Product = UnityEngine.Purchasing.Product;
using UnityEditor;

#pragma warning disable CS0618
public class IAPManager : MonoBehaviour, IStoreListener
{
    [SerializeField] private GameObject startedPack1;
    [SerializeField] private GameObject startedPack2;
    [SerializeField] private GameObject starPack1;
    [SerializeField] private GameObject galaPack;

    IStoreController controller;
    [SerializeField] private ChestManager chestManager;

    public string[] product;



    [Obsolete]
    public void Start()
    {
        IAPStart();

        if (!PlayerPrefs.HasKey("startedpack1"))
            startedPack1.SetActive(true);
        else
        {
            startedPack1.SetActive(false);
            //starPack1.SetActive(true);
        }

        //if (!PlayerPrefs.HasKey("startedpack2"))
        //    startedPack2.SetActive(true);
        //else
        //{
        //    startedPack2.SetActive(false);
        //    starPack1.SetActive(true);
        //}

        //if (!PlayerPrefs.HasKey("galaPack"))
        //    galaPack.SetActive(true);
        //else
        //    galaPack.SetActive(false);
    }

    [Obsolete]
    public void IAPStart()
    {
        var module = StandardPurchasingModule.Instance();
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(module);

        foreach (string item in product)
        {
            builder.AddProduct(item, ProductType.Consumable);
        }

        UnityPurchasing.Initialize(this, builder);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        this.controller = controller;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("Failed initiliaze");
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log("Purchuase failed");
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        if (string.Equals(e.purchasedProduct.definition.id, product[0], StringComparison.Ordinal)) //1000 altýn paketi
        {    
            DataManager.instance.AddGold(1000);
            return PurchaseProcessingResult.Complete;
        }
        else if (string.Equals(e.purchasedProduct.definition.id, product[1], StringComparison.Ordinal)) //6250 altýn paketi
        {
            DataManager.instance.AddGold(6250);
            return PurchaseProcessingResult.Complete;
        }
        else if (string.Equals(e.purchasedProduct.definition.id, product[2], StringComparison.Ordinal))//15000 altýn paketi
        {
            DataManager.instance.AddGold(15000);
            return PurchaseProcessingResult.Complete;
        }
        else if (string.Equals(e.purchasedProduct.definition.id, product[3], StringComparison.Ordinal))//31250 altýn paketi
        {
            DataManager.instance.AddGold(31250);
            return PurchaseProcessingResult.Complete;
        }
        else if (string.Equals(e.purchasedProduct.definition.id, product[4], StringComparison.Ordinal))//81250 altýn paketi
        {
            DataManager.instance.AddGold(81250);
            return PurchaseProcessingResult.Complete;
        }
        else if (string.Equals(e.purchasedProduct.definition.id, product[4], StringComparison.Ordinal))//175000 altýn paketi
        {
            DataManager.instance.AddGold(175000);
            return PurchaseProcessingResult.Complete;
        }
        else if (string.Equals(e.purchasedProduct.definition.id, product[5], StringComparison.Ordinal)) //started pack
        {
            DataManager.instance.AddGold(1000);
            DataManager.instance.AddEnergy(50);
            chestManager.EpicChestFree();
            startedPack1.SetActive(false);
            PlayerPrefs.SetInt("startedpack1", 1);

            return PurchaseProcessingResult.Complete;
        }
        else if (string.Equals(e.purchasedProduct.definition.id, product[6], StringComparison.Ordinal)) //legendary pack
        {
            DataManager.instance.AddEnergy(300);
            DataManager.instance.AddHeroToken(3000);
            chestManager.LegendaryChestFree();

            //startedPack2.SetActive(false);

            return PurchaseProcessingResult.Complete;
        }
        else if (string.Equals(e.purchasedProduct.definition.id, product[7], StringComparison.Ordinal)) //started pack
        {
            //ShopManager.instance.shopPlayerBuy(7);
            //ShopManager.instance.shopPlayerBuy(19);
            //ShopManager.instance.shopPlayerBuy(20);
            //PlayerPrefs.SetInt("galaPack", 1);
            galaPack.SetActive(false);

            return PurchaseProcessingResult.Complete;
        }
        else if (string.Equals(e.purchasedProduct.definition.id, product[8], StringComparison.Ordinal))//100000 altýn paketi
        {
            DataManager.instance.AddGold(500000);

            return PurchaseProcessingResult.Complete;
        }
        else if (string.Equals(e.purchasedProduct.definition.id, product[9], StringComparison.Ordinal))//premieum paketi
        {
            DataManager.instance.AddGold(150);

            return PurchaseProcessingResult.Complete;
        }
        else if (string.Equals(e.purchasedProduct.definition.id, product[10], StringComparison.Ordinal))//premieum paketi
        {
            DataManager.instance.AddGold(500);
            DataManager.instance.AddGold(1000000);


            return PurchaseProcessingResult.Complete;
        }
        //else if (string.Equals(e.purchasedProduct.definition.id, product[6], StringComparison.Ordinal))//8000 altýn paketi
        //{
        //    PlayerPrefs.SetInt("gold", PlayerPrefs.GetInt("gold") + 8000);
        //    dataManager.goldText.text = PlayerPrefs.GetInt("gold").ToString();

        //    return PurchaseProcessingResult.Complete;
        //}
        //else if (string.Equals(e.purchasedProduct.definition.id, product[7], StringComparison.Ordinal)) //started pack 2
        //{
        //    PlayerPrefs.SetInt("gold", PlayerPrefs.GetInt("gold") + 3000);
        //    PlayerPrefs.SetInt("energy", PlayerPrefs.GetInt("energy") + 500);
        //    dataManager.goldText.text = PlayerPrefs.GetInt("gold").ToString();
        //    dataManager.energyText.text = PlayerPrefs.GetInt("energy").ToString();
        //    StartCoroutine(dataManager.popUpCreat("GELISIM PAKETINI ALDINIZ! KEYFINI CIKARTIN"));

        //    return PurchaseProcessingResult.Complete;
        //}


        //else if (string.Equals(e.purchasedProduct.definition.id, product[5], StringComparison.Ordinal)) //noadspaketi
        //{
        //    PlayerPrefs.SetInt("gold", PlayerPrefs.GetInt("gold") + 500);
        //    dataManager.goldText.text = PlayerPrefs.GetInt("gold").ToString();

        //    //if (PlayerPrefs.HasKey("Noads") == true)
        //    //{
        //    //    PlayerPrefs.SetInt("Noads", 1);
        //    //}

        //    PlayerPrefs.SetInt("noads", 1);
        //    dataManager.NoAdsRemove();
        //    StartCoroutine(dataManager.popUpCreat("REKLAMSIZ OYUN DENEYIMI! KEYFINI CIKART!"));

        //    return PurchaseProcessingResult.Complete;
        //}
        //else if (string.Equals(e.purchasedProduct.definition.id, product[6], StringComparison.Ordinal))
        //{
        //    if (PlayerPrefs.HasKey("Noads") == true)
        //    {
        //        PlayerPrefs.SetInt("Noads", 1);
        //    }
        //    return PurchaseProcessingResult.Complete;
        //}



        else
        {
            return PurchaseProcessingResult.Pending;
        }

    }

    public void IAPButton(string id)
    {
        Product product = controller.products.WithID(id);
        if (product != null && product.availableToPurchase)
        {
            controller.InitiatePurchase(product);
            Debug.Log("Buying");
        }
        else
            Debug.Log("Not Buying");
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        throw new NotImplementedException();
    }
}